#
# Module manifest for module 'RepoManager'
#
# Generated by: Stefan Greve
#
# Generated on: 6/28/2022
#

@{
    RootModule = './bin/Debug/netstandard2.1/publish/RepoManager.dll'
    ModuleVersion = '1.0.0'
    # CompatiblePSEditions = @()
    GUID = '8e96f76d-185e-477b-bbd8-857b4c82f3ca'
    Author = 'Stefan Greve'
    CompanyName = 'Advanced Systems'
    Copyright = '(c) 2022 Advanced Systems. All rights reserved.'
    Description = 'PowerShell module to manager your Git respositories from the terminal'
    # PowerShellVersion = ''
    # PowerShellHostName = ''
    # PowerShellHostVersion = ''
    # DotNetFrameworkVersion = ''
    # CLRVersion = ''
    # ProcessorArchitecture = ''
    # RequiredModules = @()
    # RequiredAssemblies = @()
    # ScriptsToProcess = @()
    # TypesToProcess = @()
    # FormatsToProcess = @()
    # NestedModules = @()

    FunctionsToExport = '*'

    CmdletsToExport = @(
        'Import-Repository',
        'Get-Branch',
        'Get-Repository',
        'Remove-Repository'
    )

    AliasesToExport = @(
        'clone',    # Import-Repository
        'track',    # Get-Branch
        'grepo',    # Get-Repository
        'rrepo'     # Remove-Repository
    )

    # VariablesToExport = '*'

    # DscResourcesToExport = @()

    # ModuleList = @()

    # FileList = @()

    PrivateData = @{
        PSData = @{
            Tags = @('Git', 'Repository', 'Manager', 'Windows', 'Development', 'Productivity')
            LicenseUri = 'https://www.gnu.org/licenses/gpl-3.0.en.html'
            ProjectUri = 'https://github.com/Advanced-Systems/repomanager/'
            # IconUri = ''
            ReleaseNotes = 'https://github.com/Advanced-Systems/repomanager/blob/master/CHANGELOG.md'
        }
    }

    # HelpInfoURI = ''
    # DefaultCommandPrefix = ''
}

