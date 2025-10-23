from app.database.database import SessionLocal, engine, Base
from app.models.user import User, UserRole
from app.models.coupon import Coupon, CouponAssignment
from app.services.auth import get_password_hash
from datetime import datetime, timedelta

# Create all tables
Base.metadata.create_all(bind=engine)

def init_db():
    db = SessionLocal()
    
    # Check if admin user exists
    admin_user = db.query(User).filter(User.username == "admin").first()
    if not admin_user:
        # Create admin user
        admin_user = User(
            username="admin",
            email="admin@example.com",
            hashed_password=get_password_hash("admin123"),
            role=UserRole.ADMIN
        )
        db.add(admin_user)
        
        # Create manager user
        manager_user = User(
            username="manager",
            email="manager@example.com",
            hashed_password=get_password_hash("manager123"),
            role=UserRole.MANAGER
        )
        db.add(manager_user)
        
        # Create regular user
        regular_user = User(
            username="user",
            email="user@example.com",
            hashed_password=get_password_hash("user123"),
            role=UserRole.USER
        )
        db.add(regular_user)
        
        # Commit users
        db.commit()
        db.refresh(admin_user)
        db.refresh(manager_user)
        db.refresh(regular_user)
        
        # Create sample coupons
        coupon1 = Coupon(
            code="WELCOME10",
            description="Welcome discount",
            discount_amount=10,
            discount_type="percentage",
            expiration_date=datetime.utcnow() + timedelta(days=30),
            created_by=admin_user.id
        )
        db.add(coupon1)
        
        coupon2 = Coupon(
            code="SAVE20",
            description="Special savings",
            discount_amount=20,
            discount_type="fixed",
            expiration_date=datetime.utcnow() + timedelta(days=60),
            created_by=manager_user.id
        )
        db.add(coupon2)
        
        # Commit coupons
        db.commit()
        db.refresh(coupon1)
        db.refresh(coupon2)
        
        # Assign coupons to users
        assignment1 = CouponAssignment(
            coupon_id=coupon1.id,
            user_id=regular_user.id
        )
        db.add(assignment1)
        
        assignment2 = CouponAssignment(
            coupon_id=coupon2.id,
            user_id=regular_user.id
        )
        db.add(assignment2)
        
        db.commit()
        print("Database initialized with sample data")
        print("Users created:")
        print("- Admin: admin / admin123")
        print("- Manager: manager / manager123")
        print("- User: user / user123")
    else:
        print("Database already initialized")
    
    db.close()

if __name__ == "__main__":
    init_db()