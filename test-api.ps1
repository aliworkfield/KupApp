# PowerShell script to test the API endpoints
Write-Host "Testing API endpoints..."

# Test health endpoint
try {
    $healthResponse = Invoke-WebRequest -Uri "http://localhost:8001/health" -Method GET -ErrorAction Stop
    Write-Host "Health endpoint: $($healthResponse.StatusCode) - $($healthResponse.Content)"
} catch {
    Write-Host "Health endpoint: Failed - $($_.Exception.Message)"
}

# Test users endpoint (this will likely fail without auth)
try {
    $usersResponse = Invoke-WebRequest -Uri "http://localhost:8001/users" -Method GET -ErrorAction Stop
    Write-Host "Users endpoint: $($usersResponse.StatusCode)"
} catch {
    Write-Host "Users endpoint: Failed - $($_.Exception.Message)"
}

# Test coupons endpoint (this will likely fail without auth)
try {
    $couponsResponse = Invoke-WebRequest -Uri "http://localhost:8001/coupons" -Method GET -ErrorAction Stop
    Write-Host "Coupons endpoint: $($couponsResponse.StatusCode)"
} catch {
    Write-Host "Coupons endpoint: Failed - $($_.Exception.Message)"
}

Write-Host "Test completed."