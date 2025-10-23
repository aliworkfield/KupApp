from sqlalchemy.orm import Session
from app.models.coupon import Coupon, CouponAssignment
from app.models.user import User
from app.schemas.coupon import CouponCreate, CouponUpdate, CouponAssignmentCreate
from typing import List, Optional
from datetime import datetime

def get_coupon(db: Session, coupon_id: int):
    return db.query(Coupon).filter(Coupon.id == coupon_id).first()

def get_coupon_by_code(db: Session, code: str):
    return db.query(Coupon).filter(Coupon.code == code).first()

def get_coupons(db: Session, skip: int = 0, limit: int = 100):
    return db.query(Coupon).offset(skip).limit(limit).all()

def get_active_coupons(db: Session):
    return db.query(Coupon).filter(Coupon.is_active == True).all()

def create_coupon(db: Session, coupon: CouponCreate, creator_id: int):
    db_coupon = Coupon(
        code=coupon.code,
        description=coupon.description,
        discount_amount=coupon.discount_amount,
        discount_type=coupon.discount_type,
        expiration_date=coupon.expiration_date,
        created_by=creator_id
    )
    db.add(db_coupon)
    db.commit()
    db.refresh(db_coupon)
    return db_coupon

def update_coupon(db: Session, coupon_id: int, coupon_update: CouponUpdate):
    db_coupon = db.query(Coupon).filter(Coupon.id == coupon_id).first()
    if not db_coupon:
        return None
    
    update_data = coupon_update.dict(exclude_unset=True)
    for key, value in update_data.items():
        setattr(db_coupon, key, value)
    
    db.commit()
    db.refresh(db_coupon)
    return db_coupon

def delete_coupon(db: Session, coupon_id: int):
    db_coupon = db.query(Coupon).filter(Coupon.id == coupon_id).first()
    if not db_coupon:
        return None
    
    db.delete(db_coupon)
    db.commit()
    return db_coupon

def assign_coupon_to_user(db: Session, assignment: CouponAssignmentCreate):
    db_assignment = CouponAssignment(
        coupon_id=assignment.coupon_id,
        user_id=assignment.user_id
    )
    db.add(db_assignment)
    db.commit()
    db.refresh(db_assignment)
    return db_assignment

def get_user_coupons(db: Session, user_id: int):
    return db.query(CouponAssignment)\
             .join(Coupon)\
             .filter(CouponAssignment.user_id == user_id)\
             .all()

def get_unused_user_coupons(db: Session, user_id: int):
    return db.query(CouponAssignment)\
             .join(Coupon)\
             .filter(CouponAssignment.user_id == user_id)\
             .filter(CouponAssignment.is_used == False)\
             .all()

def get_manager_coupons(db: Session, manager_id: int):
    return db.query(Coupon)\
             .filter(Coupon.created_by == manager_id)\
             .all()

def get_unassigned_coupons(db: Session):
    # Get coupons that have no assignments
    assigned_coupon_ids = db.query(CouponAssignment.coupon_id).distinct()
    return db.query(Coupon)\
             .filter(~Coupon.id.in_(assigned_coupon_ids))\
             .all()

def mark_coupon_as_used(db: Session, assignment_id: int):
    db_assignment = db.query(CouponAssignment)\
                      .filter(CouponAssignment.id == assignment_id)\
                      .first()
    if not db_assignment:
        return None
    
    db_assignment.is_used = True
    db_assignment.used_at = datetime.utcnow()
    db.commit()
    db.refresh(db_assignment)
    return db_assignment