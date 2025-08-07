Set-Location -Path (Join-Path (Join-Path $PSScriptRoot "..") "H1Assist")

Write-Host "Starting Tailwind CSS in watch mode..." -ForegroundColor Yellow

& npm run css:debug

Read-Host "Press Enter to close this window..."