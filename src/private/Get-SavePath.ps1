function Get-SavePath {
    $SavePath = Join-Path -Path $([System.Environment]::GetFolderPath("ApplicationData")) -ChildPath "RepoManager"

    if (-not (Test-Path -Path $SavePath))
    {
        New-Item -ItemType Directory -Path $SavePath -Force | Out-Null
    }

    Write-Output $SavePath
}
