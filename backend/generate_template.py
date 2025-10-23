import pandas as pd
from datetime import datetime, timedelta

def generate_coupon_template():
    # Create sample data for the template
    data = {
        'code': ['WELCOME10', 'SAVE20', 'HOLIDAY25'],
        'description': ['Welcome discount for new users', 'Special savings coupon', 'Holiday discount'],
        'discount_amount': [10, 20, 25],
        'discount_type': ['percentage', 'fixed', 'percentage'],
        'expiration_date': [
            datetime.now() + timedelta(days=30),
            datetime.now() + timedelta(days=60),
            datetime.now() + timedelta(days=90)
        ]
    }
    
    df = pd.DataFrame(data)
    df.to_excel('coupon_template.xlsx', index=False)
    print("Coupon template generated: coupon_template.xlsx")

if __name__ == "__main__":
    generate_coupon_template()