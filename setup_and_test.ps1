# Setup and test SQL Server LocalDB for Coupon Management App

Write-Host "Setting up SQL Server LocalDB for Coupon Management App..." -ForegroundColor Green

# Check if LocalDB is installed
try {
    $localDBInfo = sqlcmd -S "(localdb)\MSSQLLocalDB" -Q "SELECT @@VERSION" 2>$null
    Write-Host "SQL Server LocalDB is available." -ForegroundColor Green
} catch {
    Write-Host "SQL Server LocalDB is not available. Please install SQL Server Express with LocalDB." -ForegroundColor Red
    exit 1
}

# Create and populate the database using SQL script
Write-Host "Creating and populating CouponTestDb database using SQL script..." -ForegroundColor Yellow

# Execute the SQL script
sqlcmd -S "(localdb)\MSSQLLocalDB" -i "create_sqlserver_db.sql"

if ($LASTEXITCODE -eq 0) {
    Write-Host "Database creation completed successfully!" -ForegroundColor Green
} else {
    Write-Host "Database creation failed!" -ForegroundColor Red
    exit 1
}

# Build and run the test connection script
Write-Host "Building and running test connection script..." -ForegroundColor Yellow

# Build the test connection project
dotnet build test_db_connection.csproj

if ($LASTEXITCODE -eq 0) {
    Write-Host "Test connection project built successfully!" -ForegroundColor Green
} else {
    Write-Host "Failed to build test connection project!" -ForegroundColor Red
    exit 1
}

# Copy appsettings.json to the output directory
Copy-Item "dotnet-backend\CouponApp\appsettings.json" -Destination "bin\Debug\net8.0\" -Force

# Run the test connection script
Write-Host "Running database connection test..." -ForegroundColor Yellow
dotnet run --project test_db_connection.csproj

Write-Host "Setup and test complete!" -ForegroundColor Green