class RepoManagerConfiguration {
    [ValidateNotNullOrEmpty()][RepoManagerContainer[]] $Container
    [ValidateNotNullOrEmpty()][string] $Protocol

    RepoManagerConfiguration([RepoManagerContainer[]] $Container, [string] $Protocol) {
        $this.Container = $Container
        $this.Protocol = $Protocol
    }
}

