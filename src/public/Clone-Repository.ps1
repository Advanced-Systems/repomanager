function Clone-Repository {
    [Alias("crepo")]
    [CmdletBinding()]
    param(
        [Parameter(Position = 0, Mandatory, ValueFromPipeline)]
        [string[]] $Uri,

        [Parameter()]
        [string] $Path,

        [Parameter()]
        [switch] $AllBranches
    )

    begin {
        $ConfigPath = Get-ConfigurationPath
        $ConfigFile = Get-Content -Path $ConfigPath -Raw | ConvertFrom-Json
        $Path = if ($Path) { $Path } else { $ConfigFile.Container | Where-Object IsDefault | Select-Object -Expand Path -First 1 }
    }
    process {
        foreach ($u in $Uri) {
            $RepoName = $(Split-Path -Path $u -Leaf).Split(".")[0]
            $RepoPath = Join-Path -Path $Path -ChildPath $RepoName

            if (-not (Test-Path -Path $RepoPath)) {
                git clone $Uri $RepoPath

                if ($AllBranches.IsPresent) {
                    Get-AllBranches -Path $RepoPath
                }
            }
            else {
                Write-Warning "Skipping clone because '${RepoName}' already exists in '${Path}'"
            }

            # Get-Repository -Path $RepoPath
        }
    }
    end {
        # done
    }
}
