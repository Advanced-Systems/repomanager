class RepoManagerContainer {
    [ValidateNotNullOrEmpty()][string] $Name
    [ValidateNotNullOrEmpty()][string] $Path
    [ValidateNotNullOrEmpty()][bool] $IsDefault

    RepoManagerContainer([string] $Path, [bool] $IsDefault) {
        $this.Path = $Path
        $this.Name = Split-Path -Path $Path -Leaf
        $this.IsDefault = $IsDefault
    }
}
