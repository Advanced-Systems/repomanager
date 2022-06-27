function Register-RepositoryPath {
    <#
        .SYNOPSIS
        Register a new repository path in this module's configuration file. These paths can be used as a destination path for local repository clones.
    #>
    [CmdletBinding()]
    param(
        [Parameter(Position = 0, Mandatory)]
        [string] $Path,

        [Parameter()]
        [switch] $AsDefault
    )
    begin {
        Set-Configuration
        $ConfigPath = Get-ConfigurationPath
        $ConfigFile = Get-Content -Path $ConfigPath -Raw | ConvertFrom-Json
        New-Item -ItemType Directory -Path $Path -Force | Out-Null
    }
    process {
        $RepoManagerContainer = [RepoManagerContainer]::new($Path, $AsDefault.IsPresent)

        if ($AsDefault) {
            $ConfigFile.Path | ForEach-Object { $_.IsDefault = $false }
            Write-Verbose -Message "Set '$($RepoManagerContainer.Name)' as default repository container"
        }

        $ConfigFile.Path = $ConfigFile.Path.Where({ $_.Name -ne $RepoManagerContainer.Name -and $_ -ne $RepoManagerContainer.ToString() })
        $ConfigFile.Path.Add($RepoManagerContainer)
        $ConfigFile | ConvertTo-Json | Out-File -FilePath $ConfigPath
    }
    end {
        Write-Verbose $ConfigFile
    }
}
