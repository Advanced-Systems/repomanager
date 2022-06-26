function Get-Configuration {
    $ConfigPath = Join-Path -Path $(Get-SavePath) -ChildPath "config.json"

    if (-not (Test-Path -Path $ConfigPath))
    {
        $DefaultSettings = [PSCustomObject]@{
            "Path" = @([RepositoryContainer]::new([System.Environment]::GetFolderPath("Desktop"), $true))
            "Protocol" = "SSH"
        }

        ConvertTo-Json $DefaultSettings | Out-File -FilePath $ConfigPath
    }

    Write-Output $ConfigPath
}
