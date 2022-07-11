using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using System.Management.Automation;

namespace RepoManager
{
    [Alias("track")]
    [Cmdlet(VerbsCommon.Get, "Branch")]
    public class GetBranchCommand : PSCmdlet
    {
        [ValidateNotNullOrEmpty()]
        [ArgumentCompleter(typeof(NameArgumentCompleter))]
        [Parameter(Position = 0, Mandatory = true, ParameterSetName = "All", HelpMessage = "Repository name")]
        [Parameter(Position = 0, Mandatory = true, ParameterSetName = "Name", HelpMessage = "Repository name")]
        public string Name { get; set; }

        [ValidateNotNullOrEmpty()]
        [Parameter(Position = 1, Mandatory = true, ParameterSetName = "Name", HelpMessage = "Name of remote branch")]
        public string Branch { get; set; }

        [ArgumentCompleter(typeof(PathArgumentCompleter))]
        [Parameter(Position = 2, HelpMessage = "Path to repository container")]
        public string Path { get; set; }

        [Parameter(ParameterSetName = "All")]
        public SwitchParameter All { get; set; }

        private Configuration Configuration { get; set; }

        private string GitPath { get; set; }

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

            GitPath = System.IO.Path.Combine(Path, Name, ".git");
        }

        protected override void ProcessRecord()
        {
            try
            {
                if (All.IsPresent)
                {
                    Git.TrackAllBranches(GitPath, WriteVerbose, WriteWarning);
                }
                else
                {
                    var localBranches = Git.GetLocalBranches(GitPath);

                    if (!localBranches.Contains(Branch))
                    {
                        WriteVerbose($"Branch '{Branch}' set up to track 'origin/{Branch}'");
                        Git.TrackBranch(GitPath, Branch);
                    }
                    else
                    {
                        WriteWarning($"A branch named '{Branch}' already exists");
                    }
                }
            }
            catch (FileNotFoundException exception)
            {
                WriteError(new ErrorRecord(exception, "Not a Git Repository", ErrorCategory.ObjectNotFound, GitPath));
            }
        }

        protected override void EndProcessing()
        {

        }
    }
}
