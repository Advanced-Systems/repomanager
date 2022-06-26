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
        $ConfigPath = Get-Configuration
        $ConfigFile = Get-Content -Path $ConfigPath -Raw | ConvertFrom-Json
        New-Item -ItemType Directory -Path $Path -Force | Out-Null
    }
    process {
        $RepositoryPath = [RepositoryContainer]::new($Path, $AsDefault.IsPresent)

        if ($AsDefault) {
            $ConfigFile.Path | ForEach-Object { $_.IsDefault=$false }
            Write-Verbose -Message "Set '$($RepositoryPath.Name)' as default repository container"
        }

        $ConfigFile.Path = $ConfigFile.Path.Where({ $_.Name -ne $RepositoryPath.Name })
        $ConfigFile.Path += $RepositoryPath
        $ConfigFile | ConvertTo-Json | Out-File -FilePath $ConfigPath
    }
    end {
        Write-Output $ConfigFile
    }
}
