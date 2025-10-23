from sqlalchemy import Column, Integer, String, Text, DateTime, Boolean, ForeignKey
from sqlalchemy.orm import relationship
from app.database.database import Base
from datetime import datetime

class Coupon(Base):
    __tablename__ = "coupons"

    id = Column(Integer, primary_key=True, index=True)
    code = Column(String, unique=True, index=True)
    description = Column(Text)
    discount_amount = Column(Integer)  # In cents or percentage points
    discount_type = Column(String)  # "percentage" or "fixed"
    expiration_date = Column(DateTime)
    is_active = Column(Boolean, default=True)
    created_at = Column(DateTime, default=datetime.utcnow)
    created_by = Column(Integer, ForeignKey("users.id"))
    
    # Relationships
    creator = relationship("User", back_populates="created_coupons")
    assignments = relationship("CouponAssignment", back_populates="coupon")

class CouponAssignment(Base):
    __tablename__ = "coupon_assignments"

    id = Column(Integer, primary_key=True, index=True)
    coupon_id = Column(Integer, ForeignKey("coupons.id"))
    user_id = Column(Integer, ForeignKey("users.id"))
    is_used = Column(Boolean, default=False)
    assigned_at = Column(DateTime, default=datetime.utcnow)
    used_at = Column(DateTime, nullable=True)
    
    # Relationships
    coupon = relationship("Coupon", back_populates="assignments")
    user = relationship("User", back_populates="assigned_coupons")