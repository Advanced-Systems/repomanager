function Get-Repository {
    [Alias("grepo")]
    [OutputType([RepoManagerRepository])]
    [CmdletBinding()]
    param(
        [Parameter(Position = 0, Mandatory, ValueFromPipeline, ParameterSetName = "Name")]
        [string[]] $Name,

        [Parameter(ParameterSetName = "Name")]
        [string] $Path,

        [Parameter(ParameterSetName = "All")]
        [switch] $All
    )

    begin {
        $ConfigPath = Get-ConfigurationPath
        $ConfigFile = Get-Content -Path $ConfigPath -Raw | ConvertFrom-Json
        $Path = if ($Path) { $Path } else { $ConfigFile.Container | Where-Object IsDefault | Select-Object -Expand Path -First 1 }
    }
    process {
        if ($Name) {
            foreach ($n in $Name) {
                Write-Output $([RepoManagerRepository]::new($Name, $Path))
            }
        }
        else {

        }
    }
    end {

    }
}
