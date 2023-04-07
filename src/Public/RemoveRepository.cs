using System.IO;
using System.Linq;
using System.Collections.Generic;

using System.Management.Automation;

namespace RepoManager
{
    [Alias("rmrepo")]
    [Cmdlet(VerbsCommon.Remove, "Repository", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
    public class RemoveRepositoryCommand : PSCmdlet
    {
        [ValidateNotNullOrEmpty()]
        [ArgumentCompleter(typeof(RepositoryArgumentCompleter))]
        [Parameter(Position = 0, Mandatory = true, ValueFromPipeline = true, HelpMessage = "Repository to remove")]
        public List<string> Name { get; set; }

        [ArgumentCompleter(typeof(ContainerArgumentCompleter))]
        [Parameter(Position = 1, HelpMessage = "Path to repository container")]
        public string Path { get; set; }

        private Configuration Configuration { get; set; }

        protected override void BeginProcessing()
        {
            var configurationManager = new ConfigurationManager();
            Configuration = configurationManager.Configuration;

            Path = MyInvocation.BoundParameters.ContainsKey("Path")
                 ? System.IO.Path.GetFullPath(Path)
                 : Configuration.Container
                    .Where(repo => repo.IsDefault)
                    .Select(repo => repo.Path)
                    .First();
        }

        protected override void ProcessRecord()
        {
            foreach (string n in Name)
            {
                string repoPath = System.IO.Path.Combine(Path, n);

                try
                {
                    if (ShouldProcess(repoPath, $"Delete repository '{n}'"))
                    {
                        WriteVerbose($"Deleting '{repoPath}' . . .");
                        Utils.DeleteDirectoryRecursively(repoPath);
                    }
                }
                catch (DirectoryNotFoundException exception)
                {
                    WriteError(new ErrorRecord(exception, "Directory not found", ErrorCategory.ObjectNotFound, repoPath));
                    continue;
                }
            }
        }

        protected override void EndProcessing()
        {

        }
    }
}
