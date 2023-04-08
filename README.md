<p align="center">
  <a title="Project Logo">
    <img height="150" style="margin-top:15px" src="https://raw.githubusercontent.com/Advanced-Systems/vector-assets/master/advanced-systems-logo-annotated.svg">
  </a>
</p>

<h1 align="center">Advanced Systems RepoManager</h1>

## Basic Usage

Open the configuration file to customize repository container. The exact location
of the settings file varies between the two major platforms:

```powershell
$windows = $env:AppData\RepoManager\config.json
$unix = ~\.repomanager\config.json
```

## Remarks

Configure `ssh-agent` to run automatically (requires admin privileges):

```powershell
Start-Service ssh-agent -StartupType Automatic
```

If your private SSH key is not stored in one of the defaults locations (like
`~/.ssh/id_rsa`), you'll need to tell your SSH authentication agent where to find
it. To add your key to `ssh-agent`, run

```powershell
ssh-add $home\.ssh\<private_key>
```

## Cmdlets

- `Get-Branch`
- `Get-Repository`
- `Import-Repository`
- `Remove-Repository`
