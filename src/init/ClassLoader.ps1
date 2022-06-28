Push-Location $(Get-Item $PSScriptRoot).Parent

Get-ChildItem -Path ".\classes" -Filter "*.ps1" | ForEach-Object { Invoke-Expression $_.FullName }

Pop-Location
