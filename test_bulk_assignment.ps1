# Test bulk assignment of coupons to users

Write-Host "Testing bulk assignment of coupons to users..." -ForegroundColor Green

# Login as admin
Write-Host "Logging in as admin..." -ForegroundColor Yellow
try {
    $loginResponse = Invoke-WebRequest -Uri "http://localhost:8001/auth/token" -Method POST -ContentType "application/x-www-form-urlencoded" -Body "username=admin&password=admin123"
    Write-Host "Login successful!" -ForegroundColor Green
    $tokenData = $loginResponse.Content | ConvertFrom-Json
    $token = $tokenData.accessToken
    Write-Host "Token: $token" -ForegroundColor Cyan
    
    # Get all users
    Write-Host "Fetching all users..." -ForegroundColor Yellow
    $usersResponse = Invoke-WebRequest -Uri "http://localhost:8001/users" -Method GET -Headers @{Authorization = "Bearer $token"}
    $usersData = $usersResponse.Content | ConvertFrom-Json
    Write-Host "Found $($usersData.Count) users" -ForegroundColor Cyan
    
    # Get all coupons
    Write-Host "Fetching all coupons..." -ForegroundColor Yellow
    $couponsResponse = Invoke-WebRequest -Uri "http://localhost:8001/coupons" -Method GET -Headers @{Authorization = "Bearer $token"}
    $couponsData = $couponsResponse.Content | ConvertFrom-Json
    Write-Host "Found $($couponsData.Count) coupons" -ForegroundColor Cyan
    
    # Create bulk assignment data (coupon1 to user1, coupon2 to user2, etc.)
    $assignments = @()
    $count = [Math]::Min($couponsData.Count, $usersData.Count)
    
    for ($i = 0; $i -lt $count; $i++) {
        $assignment = @{
            couponId = $couponsData[$i].id
            userId = $usersData[$i].id
        }
        $assignments += $assignment
    }
    
    # Convert to JSON
    $assignmentJson = $assignments | ConvertTo-Json
    
    # Bulk assign coupons to users
    Write-Host "Bulk assigning coupons to users..." -ForegroundColor Yellow
    $bulkAssignResponse = Invoke-WebRequest -Uri "http://localhost:8001/coupons/assign-bulk" -Method POST -Headers @{Authorization = "Bearer $token"; "Content-Type" = "application/json"} -Body $assignmentJson
    Write-Host "Bulk assignment successful!" -ForegroundColor Green
    $assignmentResult = $bulkAssignResponse.Content | ConvertFrom-Json
    Write-Host "Assigned $($assignmentResult.Count) coupons" -ForegroundColor Cyan
    
    # Display the assignments
    Write-Host "Assignments:" -ForegroundColor Yellow
    foreach ($assignment in $assignmentResult) {
        Write-Host "Coupon ID: $($assignment.couponId) assigned to User ID: $($assignment.userId)" -ForegroundColor Cyan
    }
}
catch {
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        $response = $_.Exception.Response
        Write-Host "Status Code: $($response.StatusCode)" -ForegroundColor Red
        Write-Host "Status Description: $($response.StatusDescription)" -ForegroundColor Red
    }
}