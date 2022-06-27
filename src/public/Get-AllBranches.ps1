function Get-AllBranches {
    param(
        [Parameter()]
        [string] $Path = $PWD.Path
    )

    begin {
        $GitPath = Join-Path -Path $Path -ChildPath ".git"
        Push-Location -Path $Path
    }

    process {
        if (Test-Path -Path $GitPath) {
            git fetch --all

            foreach ($Branch in $(git branch -r | Select-String -Pattern "origin/master|origin/main|origin/HEAD" -NotMatch)) {
                $Branch = ($Branch -split '/', 2).Trim()[1]

                if (-not $(git show-ref "refs/heads/${Branch}")) {
                    git branch --track $Branch "origin/${Branch}"
                }
            }
        }
        else {
            Write-Error "Not a Git repository" -Category ObjectNotFound -ErrorAction Stop
        }
    }
    end {
        Pop-Location
    }
}
