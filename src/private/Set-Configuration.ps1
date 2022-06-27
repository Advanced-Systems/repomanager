function Set-Configuration {
    $ConfigPath = Get-ConfigurationPath

    if (-not (Test-Path -Path $ConfigPath))
    {
        $ReposDirectory = Join-Path -Path $([System.Environment]::GetFolderPath("Desktop")) -ChildPath "repos"
        New-Item -ItemType Directory -Path $ReposDirectory -Force | Out-Null
        $Container = @([RepoManagerContainer]::new($ReposDirectory, $true))
        $Protocol = "SSH"

        $DefaultSettings = [RepoManagerConfiguration]::new($Container, $Protocol)

        ConvertTo-Json -InputObject $DefaultSettings | Out-File -FilePath $ConfigPath
    }
}
