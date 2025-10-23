# PowerShell script to initialize the database
Write-Host "Initializing database..."
dotnet run --project InitDb/InitDb.csproj
Write-Host "Database initialization completed."