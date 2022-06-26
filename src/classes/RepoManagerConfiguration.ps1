class RepoManagerConfiguration {
    [ValidateNotNullOrEmpty()][RepoManagerContainer[]] $Path
    [ValidateNotNullOrEmpty()][string] $Protocol

    RepoManagerConfiguration([RepoManagerContainer[]] $Path, [string] $Protocol) {
        $this.Path = $Path
        $this.Protocol = $Protocol
    }
}

