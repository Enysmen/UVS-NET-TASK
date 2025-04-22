<#
.SYNOPSIS
  Boots Postgres in Docker and applies the schema from dbSchema.sql.

.DESCRIPTION
  This script always works relative to its folder (where dbSchema.sql is located).
  The first time you run PowerShell, it will ask for permission to execute (Unblock-File).
#>

# — We define the folder where the script itself is located
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition

# Full path to the schema file
$dbSchemaFile = Join-Path $scriptDir 'dbSchema.sql'

# PowerShell warning on first run (Unblock-File)
Write-Host "WARNING: You are about to run a script that configures Docker and your database."
Write-Host "If you trust this script, you may need to run 'Unblock-File setUpDatabase.ps1' to skip this prompt."
Read-Host 'Press ENTER to continue or CTRL+C to cancel'

# — Environment variables that your application expects
$Env:UvsTaskPassword       = 'guest'
$Env:UvsTaskDatabase       = 'uvsproject'
$Env:UvsTaskPort           = '7777'
$Env:UvsTaskSchemaLocation = $dbSchemaFile

# — Pull & run container
docker pull postgres
if (-not $?) { Write-Error 'Failed to pull postgres image'; exit 1 }

$container = docker run `
    -e "POSTGRES_PASSWORD=$Env:UvsTaskPassword" `
    -e "POSTGRES_DB=$Env:UvsTaskDatabase" `
    -p "$Env:UvsTaskPort`:5432" `
    -d postgres
if (-not $?) { Write-Error 'Failed to start postgres container'; exit 1 }

Write-Host 'Waiting for database to accept connections...' -NoNewline
Start-Sleep -Seconds 5  # simple timeout, in production a psql check loop is better

# — Copy the schema file into the container correctly:
Write-Host "`nCopying schema file into container..."
docker cp "$dbSchemaFile" "${container}:/dbSchema.sql"
if (-not $?) { Write-Error 'docker cp failed'; exit 1 }         # :contentReference[oaicite:2]{index=2}

# — Apply the schema via psql inside the container
Write-Host 'Applying database schema...'
docker exec -i $container psql -U postgres -d $Env:UvsTaskDatabase -f /dbSchema.sql
if (-not $?) { Write-Error 'Failed to apply schema'; exit 1 }  # :contentReference[oaicite:3]{index=3}

# — Summary information
Write-Host "`nThe database is ready to use" -ForegroundColor Green
Write-Host "Connection string: 'Server=localhost; User ID=postgres; Password=$Env:UvsTaskPassword;Port=$Env:UvsTaskPort;Database=$Env:UvsTaskDatabase;'"
Write-Host 'Schema applied:'; Get-Content $dbSchemaFile | ForEach-Object { "  $_" }

Write-Host "`nPress Ctrl+C to stop the database server and exit" -ForegroundColor Green
docker attach $container

# — On any exit (Ctrl+C) we will stop the container
trap {
    Write-Host "`nStopping PostgreSQL container..." -ForegroundColor Yellow
    docker stop $container | Out-Null
}