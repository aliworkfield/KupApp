from pydantic import BaseModel
from typing import Optional, List
from datetime import datetime
from app.models.user import UserRole

class CouponBase(BaseModel):
    code: str
    description: Optional[str] = None
    discount_amount: int
    discount_type: str  # "percentage" or "fixed"
    expiration_date: Optional[datetime] = None

class CouponCreate(CouponBase):
    pass

class CouponUpdate(BaseModel):
    code: Optional[str] = None
    description: Optional[str] = None
    discount_amount: Optional[int] = None
    discount_type: Optional[str] = None
    expiration_date: Optional[datetime] = None
    is_active: Optional[bool] = None

class CouponInDB(CouponBase):
    id: int
    is_active: bool
    created_at: datetime
    created_by: int

    class Config:
        from_attributes = True

class Coupon(CouponBase):
    id: int
    is_active: bool
    created_at: datetime
    created_by: int

    class Config:
        from_attributes = True

class CouponAssignmentBase(BaseModel):
    coupon_id: int
    user_id: int

class CouponAssignmentCreate(CouponAssignmentBase):
    pass

class CouponAssignmentUpdate(BaseModel):
    is_used: Optional[bool] = None
    used_at: Optional[datetime] = None

class CouponAssignmentInDB(CouponAssignmentBase):
    id: int
    is_used: bool
    assigned_at: datetime
    used_at: Optional[datetime] = None

    class Config:
        from_attributes = True

class CouponAssignment(CouponAssignmentBase):
    id: int
    is_used: bool
    assigned_at: datetime
    used_at: Optional[datetime] = None
    coupon_code: str

    class Config:
        from_attributes = True