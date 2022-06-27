function Set-Configuration {
    $ConfigPath = Get-ConfigurationPath

    if (-not (Test-Path -Path $ConfigPath))
    {
        $ReposDirectory = Join-Path -Path $([System.Environment]::GetFolderPath("Desktop")) -ChildPath "repos"
        $Path = @([RepoManagerContainer]::new($ReposDirectory, $true))
        $Protocol = "SSH"

        $DefaultSettings = [RepoManagerConfiguration]::new($Path, $Protocol)

        ConvertTo-Json -InputObject $DefaultSettings | Out-File -FilePath $ConfigPath
    }
}