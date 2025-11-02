# Test the API endpoints

Write-Host "Testing API endpoints..." -ForegroundColor Green

# Test login
Write-Host "Testing login..." -ForegroundColor Yellow
try {
    $loginResponse = Invoke-WebRequest -Uri "http://localhost:8001/auth/token" -Method POST -ContentType "application/x-www-form-urlencoded" -Body "username=admin&password=admin123"
    Write-Host "Login successful!" -ForegroundColor Green
    $tokenData = $loginResponse.Content | ConvertFrom-Json
    $token = $tokenData.accessToken
    Write-Host "Token: $token" -ForegroundColor Cyan
    
    # Test getting users (requires admin role)
    Write-Host "Testing get users..." -ForegroundColor Yellow
    $usersResponse = Invoke-WebRequest -Uri "http://localhost:8001/users" -Method GET -Headers @{Authorization = "Bearer $token"}
    Write-Host "Get users successful!" -ForegroundColor Green
    $usersData = $usersResponse.Content | ConvertFrom-Json
    Write-Host "Found $($usersData.Count) users:" -ForegroundColor Cyan
    $usersData | Format-Table
    
    # Test getting coupons (requires admin role)
    Write-Host "Testing get coupons..." -ForegroundColor Yellow
    $couponsResponse = Invoke-WebRequest -Uri "http://localhost:8001/coupons" -Method GET -Headers @{Authorization = "Bearer $token"}
    Write-Host "Get coupons successful!" -ForegroundColor Green
    $couponsData = $couponsResponse.Content | ConvertFrom-Json
    Write-Host "Found $($couponsData.Count) coupons:" -ForegroundColor Cyan
    $couponsData | Format-Table
}
catch {
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $response = $_.Exception.Response
        Write-Host "Status Code: $($response.StatusCode)" -ForegroundColor Red
        Write-Host "Status Description: $($response.StatusDescription)" -ForegroundColor Red
    }
}