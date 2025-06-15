# PowerShell script to generate code coverage report and open it in the browser

# Step 0: Resolve the absolute path to the project root (parent of the current script folder)
$root = Resolve-Path "$PSScriptRoot\.."

# Define the paths to the test project, coverage output folder, and HTML report folder
$projectPath = Join-Path $root "tests\TodoApp.Tests\TodoApp.Tests.csproj"
$coverageFolder = Join-Path $root "tests\TestResults"
$reportFolder = Join-Path $coverageFolder "Report"

# Step 1: Run tests with code coverage collection enabled
Write-Host "=== Step 1: Running tests with code coverage ==="
dotnet test $projectPath --collect:"XPlat Code Coverage" --results-directory $coverageFolder

# Step 2: Look for the generated coverage report file (Cobertura format)
Write-Host "`n=== Step 2: Searching for coverage.cobertura.xml ==="
$foundCoverage = Get-ChildItem -Path $coverageFolder -Recurse -Filter "coverage.cobertura.xml" -ErrorAction SilentlyContinue | Select-Object -First 1

# Exit if the coverage file was not found
if (-not $foundCoverage) {
    Write-Host "`n[ERROR] Coverage report not found. Make sure tests ran correctly."
    Start-Sleep -Seconds 5
    exit 1
}

$coverageReport = $foundCoverage.FullName
Write-Host "Found coverage report: $coverageReport"

# Step 3: Ensure ReportGenerator tool is installed
Write-Host "`n=== Step 3: Checking for ReportGenerator ==="
$rgExists = Get-Command "reportgenerator" -ErrorAction SilentlyContinue
if (-not $rgExists) {
    Write-Host "ReportGenerator not found. Installing..."
    dotnet tool install --global dotnet-reportgenerator-globaltool

    # Add the tool path to the current session's PATH
    $dotnetToolsPath = "$env:USERPROFILE\.dotnet\tools"
    $env:PATH += ";$dotnetToolsPath"
    Write-Host "ReportGenerator installed to: $dotnetToolsPath"
    Start-Sleep -Seconds 2
} else {
    Write-Host "ReportGenerator already installed."
}

# Step 4: Generate the HTML coverage report
Write-Host "`n=== Step 4: Generating HTML report ==="
reportgenerator -reports:$coverageReport -targetdir:$reportFolder -reporttypes:Html

# Step 5: Open the report in the default browser if it was successfully created
$indexPath = Join-Path $reportFolder "index.html"
if (Test-Path $indexPath) {
    Write-Host "`n=== Step 5: Opening report in browser ==="
    Start-Process $indexPath
    Start-Sleep -Seconds 2
} else {
    Write-Host "[ERROR] HTML report not generated."
    Start-Sleep -Seconds 5
    exit 1
}

# Done
Write-Host "`nAll done."
Start-Sleep -Seconds 3
