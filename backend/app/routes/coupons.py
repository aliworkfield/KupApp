from fastapi import APIRouter, Depends, HTTPException, status, UploadFile, File
from sqlalchemy.orm import Session
from sqlalchemy import and_
from typing import List
import pandas as pd
from io import StringIO
from app.database.database import get_db
from app.services.coupon import (
    get_coupon, get_coupons, create_coupon, update_coupon, delete_coupon,
    assign_coupon_to_user, get_user_coupons, get_unused_user_coupons,
    get_manager_coupons, get_unassigned_coupons, mark_coupon_as_used
)
from app.schemas.coupon import Coupon, CouponCreate, CouponUpdate, CouponAssignmentCreate
from app.models.user import User, UserRole
from app.models.coupon import CouponAssignment, Coupon as CouponModel
from app.services.auth import authenticate_user_role
from app.routes.users import get_current_user

router = APIRouter()

# Dependency to check if user is admin
def check_admin_role(current_user_role: UserRole = Depends()):
    if not authenticate_user_role(UserRole.ADMIN, current_user_role):
        raise HTTPException(
            status_code=status.HTTP_403_FORBIDDEN,
            detail="Admin access required"
        )

# Dependency to check if user is manager or admin
def check_manager_role(current_user_role: UserRole = Depends()):
    if not authenticate_user_role(UserRole.MANAGER, current_user_role):
        raise HTTPException(
            status_code=status.HTTP_403_FORBIDDEN,
            detail="Manager or Admin access required"
        )

# Admin can do everything
@router.post("/", response_model=Coupon)
def create_new_coupon(coupon: CouponCreate, db: Session = Depends(get_db), current_user_role: UserRole = Depends(check_admin_role), current_user = Depends(get_current_user)):
    # Validate input
    if not coupon.code or not coupon.discount_type:
        raise HTTPException(status_code=400, detail="Code and discount type are required")
    
    # Check if coupon code already exists
    existing_coupon = db.query(CouponModel).filter(CouponModel.code == coupon.code).first()
    if existing_coupon:
        raise HTTPException(status_code=400, detail="Coupon code already exists")
    
    db_coupon = create_coupon(db, coupon, current_user.id)
    if not db_coupon:
        raise HTTPException(status_code=500, detail="Coupon could not be created")
    return db_coupon

# Managers and Admins can create coupons
@router.post("/upload-excel", response_model=List[Coupon])
async def upload_coupons_from_excel(file: UploadFile = File(...), db: Session = Depends(get_db), current_user_role: UserRole = Depends(check_manager_role), current_user = Depends(get_current_user)):
    # Validate file
    if not file:
        raise HTTPException(status_code=400, detail="No file provided")
        
    if not file.filename or not file.filename.endswith(('.xlsx', '.xls')):
        raise HTTPException(status_code=400, detail="Only Excel files are allowed")
    
    contents = await file.read()
    if not contents:
        raise HTTPException(status_code=400, detail="File is empty")
    
    # Parse Excel file
    try:
        df = pd.read_excel(contents)
    except Exception as e:
        raise HTTPException(status_code=400, detail=f"Error reading Excel file: {str(e)}")
    
    # Validate required columns
    required_columns = ['code', 'discount_amount', 'discount_type']
    for col in required_columns:
        if col not in df.columns:
            raise HTTPException(status_code=400, detail=f"Missing required column: {col}")
    
    coupons = []
    errors = []
    
    for idx, row in df.iterrows():
        # Convert index to string and then to int for row number
        row_num = int(str(idx)) + 1
        
        # Skip rows with missing required data
        # Simple check for None or empty values
        if (row['code'] is None or row['code'] == '' or 
            row['discount_amount'] is None or 
            row['discount_type'] is None or row['discount_type'] == ''):
            errors.append(f"Row {row_num}: Missing required data")
            continue
            
        # Check if coupon code already exists
        existing_coupon = db.query(CouponModel).filter(CouponModel.code == str(row['code'])).first()
        if existing_coupon:
            errors.append(f"Row {row_num}: Coupon code {row['code']} already exists")
            continue
            
        coupon_data = CouponCreate(
            code=str(row['code']),
            description=str(row.get('description', '')),
            discount_amount=int(row['discount_amount']),
            discount_type=str(row['discount_type']),
            expiration_date=row.get('expiration_date', None)
        )
        db_coupon = create_coupon(db, coupon_data, current_user.id)
        coupons.append(db_coupon)
    
    if errors:
        return HTTPException(status_code=400, detail=f"Some coupons could not be created: {', '.join(errors)}")
    
    return coupons

# Managers and Admins can assign coupons to users
@router.post("/assign", response_model=dict)
def assign_coupon(assignment: CouponAssignmentCreate, db: Session = Depends(get_db), current_user_role: UserRole = Depends(check_manager_role)):
    # Validate input
    if not assignment.coupon_id or not assignment.user_id:
        raise HTTPException(status_code=400, detail="Coupon ID and User ID are required")
    
    # Check if coupon exists
    coupon = db.query(CouponModel).filter(CouponModel.id == assignment.coupon_id).first()
    if not coupon:
        raise HTTPException(status_code=404, detail="Coupon not found")
    
    # Check if user exists
    user = db.query(User).filter(User.id == assignment.user_id).first()
    if not user:
        raise HTTPException(status_code=404, detail="User not found")
    
    # Check if assignment already exists
    existing_assignment = db.query(CouponAssignment) \
        .filter(CouponAssignment.coupon_id == assignment.coupon_id) \
        .filter(CouponAssignment.user_id == assignment.user_id) \
        .first()
    if existing_assignment:
        raise HTTPException(status_code=400, detail="Coupon already assigned to this user")
    
    db_assignment = assign_coupon_to_user(db, assignment)
    if not db_assignment:
        raise HTTPException(status_code=500, detail="Coupon assignment failed")
    return {"message": "Coupon assigned successfully"}

# Users can see their assigned coupons
@router.get("/my-coupons", response_model=List[Coupon])
def read_user_coupons(db: Session = Depends(get_db), current_user = Depends(get_current_user)):
    try:
        assignments = get_user_coupons(db, current_user.id)
        return [assignment.coupon for assignment in assignments]
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Error fetching coupons: {str(e)}")

# Users can see their unused coupons
@router.get("/my-unused-coupons", response_model=List[Coupon])
def read_user_unused_coupons(db: Session = Depends(get_db), current_user = Depends(get_current_user)):
    try:
        assignments = get_unused_user_coupons(db, current_user.id)
        return [assignment.coupon for assignment in assignments]
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Error fetching unused coupons: {str(e)}")

# Managers can see coupons they created
@router.get("/my-created", response_model=List[Coupon])
def read_manager_coupons(db: Session = Depends(get_db), current_user = Depends(get_current_user), current_user_role: UserRole = Depends(check_manager_role)):
    try:
        coupons = get_manager_coupons(db, current_user.id)
        return coupons
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Error fetching created coupons: {str(e)}")

# Managers and Admins can see unassigned coupons
@router.get("/unassigned", response_model=List[Coupon])
def read_unassigned_coupons(db: Session = Depends(get_db), current_user_role: UserRole = Depends(check_manager_role)):
    try:
        coupons = get_unassigned_coupons(db)
        return coupons
    except Exception as e:
        raise HTTPException(status_code=500, detail=f"Error fetching unassigned coupons: {str(e)}")

# Mark coupon as used
@router.post("/use/{assignment_id}", response_model=dict)
def use_coupon(assignment_id: int, db: Session = Depends(get_db)):
    # Validate input
    if not assignment_id:
        raise HTTPException(status_code=400, detail="Assignment ID is required")
    
    result = mark_coupon_as_used(db, assignment_id)
    if not result:
        raise HTTPException(status_code=404, detail="Coupon assignment not found")
    return {"message": "Coupon marked as used"}