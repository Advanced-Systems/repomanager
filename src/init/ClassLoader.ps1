Push-Location $(Get-Item $PSScriptRoot).Parent.Parent

Get-ChildItem -Path ".\src\classes" -Filter "*.ps1" | ForEach-Object { Invoke-Expression $_.FullName }

Pop-Location
