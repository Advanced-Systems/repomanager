$Arguments = @{
    FilePath = "pwsh"
    WindowStyle = "Maximized"
    ArgumentList = @(
        "-NoProfile -NoLogo -NoExit"
        "-Command &{ cd .\src && dotnet publish && Import-Module .\bin\Debug\netstandard2.1\publish\RepoManager.dll -Force }"
    )
}

Start-Process @Arguments
