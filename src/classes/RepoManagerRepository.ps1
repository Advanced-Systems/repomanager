class RepoManagerRepository {
    [ValidateNotNullOrEmpty()] [string] $Name
    [ValidateNotNullOrEmpty()] [string] $Path
    [ValidateNotNullOrEmpty()] [string] $Remote
    [ValidateNotNullOrEmpty()] [string] $DefaultBranch
    [ValidateNotNullOrEmpty()] [string] $ActiveBranch
    [ValidateNotNullOrEmpty()] [string[]] $Contributors
    [ValidateNotNullOrEmpty()] [int] $CommitCount
    [ValidateNotNullOrEmpty()] [string] $LastCommitMessage
    [ValidateNotNullOrEmpty()] [string] $LastCommitHash

    RepoManagerRepository([string] $Name, [string] $Path) {
        $this.Name = $Name
        $this.Path = $Path

        Push-Location $(Join-Path -Path $Path -ChildPath $Name)

        $this.Remote = git config --get remote.origin.url
        $this.DefaultBranch = $(git remote show origin | Select-String -Pattern 'HEAD branch' -SimpleMatch).Line.Split(":")[1].Trim()
        $this.ActiveBranch = git rev-parse --abbrev-ref HEAD
        $this.Contributors = git log --format="%aN" | Select-Object -Unique
        $this.CommitCount = git rev-list --count HEAD
        $this.LastCommitMessage = git log -n 1 --format="%s"
        $this.LastCommitHash = git log -n 1 --format="%H"

        Pop-Location
    }
}
