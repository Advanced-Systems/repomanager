using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using System.Management.Automation;

namespace RepoManager
{
    [Cmdlet(VerbsCommon.Get, "AllBranches")]
    public class GetAllBranchesCommand : PSCmdlet
    {
        [Parameter(HelpMessage = "Path to repository")]
        public string Path { get; set; }

        private string GitPath { get; set; }

        private string CurrentWorkingDirectory { get; set; }

        protected override void BeginProcessing()
        {
            CurrentWorkingDirectory = CurrentProviderLocation("FileSystem").ProviderPath;
            Path = MyInvocation.BoundParameters.ContainsKey("Path") ? System.IO.Path.GetFullPath(Path) : CurrentWorkingDirectory;
            GitPath = System.IO.Path.Combine(Path, ".git");
            Directory.SetCurrentDirectory(Path);
        }

        protected override void ProcessRecord()
        {
            try
            {
                Git.TrackAllBranches(GitPath, WriteVerbose, WriteWarning);
            }
            catch (FileNotFoundException exception)
            {
                WriteError(new ErrorRecord(exception, "Not a Git Repository", ErrorCategory.ObjectNotFound, GitPath));
            }
        }

        protected override void EndProcessing()
        {
            Directory.SetCurrentDirectory(CurrentWorkingDirectory);
        }
    }
}
