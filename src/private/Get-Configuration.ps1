function Get-ConfigurationPath {
    Join-Path -Path $(Get-SavePath) -ChildPath "config.json"
}
