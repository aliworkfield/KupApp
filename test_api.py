import requests
import json

# Test the API endpoints
print("Testing API endpoints...")

# Test login
print("Testing login...")
login_data = {
    'username': 'admin',
    'password': 'admin123'
}

try:
    login_response = requests.post('http://localhost:8001/auth/token', data=login_data)
    print(f"Login status code: {login_response.status_code}")
    
    if login_response.status_code == 200:
        token_data = login_response.json()
        token = token_data['accessToken']
        print(f"Login successful! Token: {token[:20]}...")
        
        # Test getting users (requires admin role)
        print("Testing get users...")
        headers = {'Authorization': f'Bearer {token}'}
        users_response = requests.get('http://localhost:8001/users', headers=headers)
        print(f"Get users status code: {users_response.status_code}")
        
        if users_response.status_code == 200:
            users_data = users_response.json()
            print(f"Found {len(users_data)} users")
            for user in users_data:
                print(f"  - {user['username']} ({user['email']})")
        else:
            print(f"Failed to get users: {users_response.status_code}")
            print(users_response.text)
        
        # Test getting coupons (requires admin role)
        print("Testing get coupons...")
        coupons_response = requests.get('http://localhost:8001/coupons', headers=headers)
        print(f"Get coupons status code: {coupons_response.status_code}")
        
        if coupons_response.status_code == 200:
            coupons_data = coupons_response.json()
            print(f"Found {len(coupons_data)} coupons")
            for coupon in coupons_data:
                print(f"  - {coupon['code']}: {coupon['description']}")
        else:
            print(f"Failed to get coupons: {coupons_response.status_code}")
            print(coupons_response.text)
    else:
        print(f"Login failed: {login_response.status_code}")
        print(login_response.text)
        
except Exception as e:
    print(f"Error: {e}")