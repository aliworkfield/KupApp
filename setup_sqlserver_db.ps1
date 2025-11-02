# Setup SQL Server LocalDB for Coupon Management App

Write-Host "Setting up SQL Server LocalDB for Coupon Management App..." -ForegroundColor Green

# Check if LocalDB is installed
try {
    $localDBInfo = sqlcmd -S "(localdb)\MSSQLLocalDB" -Q "SELECT @@VERSION" 2>$null
    Write-Host "SQL Server LocalDB is available." -ForegroundColor Green
} catch {
    Write-Host "SQL Server LocalDB is not available. Please install SQL Server Express with LocalDB." -ForegroundColor Red
    exit 1
}

# Create and populate the database
Write-Host "Creating and populating CouponTestDb database..." -ForegroundColor Yellow

# Execute the SQL script
sqlcmd -S "(localdb)\MSSQLLocalDB" -i "create_sqlserver_db.sql"

if ($LASTEXITCODE -eq 0) {
    Write-Host "Database setup completed successfully!" -ForegroundColor Green
} else {
    Write-Host "Database setup failed!" -ForegroundColor Red
    exit 1
}

Write-Host "Setup complete. You can now run your application." -ForegroundColor Green