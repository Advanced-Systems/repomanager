#
# Module manifest for module "RepoManager"
#
# Generated by: Stefan Greve
#
# Genereated on: 6/26/2022
#

@{
    RootModule = "RepoManager.psm1"
    ModuleVersion = "1.0.0"
    CompatiblePSEditions = @("Desktop")
    GUID = "94a06e96-47f7-4a11-ab81-99941502d13d"
    Author = "Stefan Greve"
    CompanyName = "Advanced Systems"
    Copyright = "(c) 2022 Advanced Systems. All rights reserved."
    Description = "PowerShell module to manager your Git respositories from the terminal."
    PowerShellVersion = "5.1"
    RequiredModules = @()
    FormatsToProcess = @()

    FunctionsToExport = @(
        "Get-AllBranches",
        "New-Repository",
        "Register-RepositoryContainer"
    )

    CmdletsToExport = @()

    VariablesToExport = @()

    AliasesToExport = @(
        "nrepo"
    )

    FileList = @(
        "RepoManager.psd1",
        "RepoManager.psm1",
        "classes\RepoManagerConfiguration.ps1",
        "classes\RepoManagerContainer.ps1",
        "public\New-Repository.ps1",
        "public\Register-RepositoryContainer.ps1"
    )

    PrivateData = @{
        PSData = @{
            Tags = @("PSEdition_Desktop", "Windows", "RepoManager", "Development")
            LicenseUri = "https://www.gnu.org/licenses/gpl-3.0.en.html"
            ReleaseNotes = "https://github.com/Advanced-Systems/repomanager/blob/master/CHANGELOG.md"
        }
    }
}

