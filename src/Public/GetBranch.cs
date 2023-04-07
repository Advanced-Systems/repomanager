using System;
using System.IO;
using System.Linq;

using System.Management.Automation;

namespace RepoManager
{
    [Alias("track")]
    [Cmdlet(VerbsCommon.Get, "Branch")]
    public class GetBranchCommand : PSCmdlet
    {
        #region parameters

        [ArgumentCompleter(typeof(RepositoryArgumentCompleter))]
        [Parameter(ParameterSetName = "All")]
        [Parameter(ParameterSetName = "Branch")]
        [Parameter(ParameterSetName = "Container", Mandatory = true)]
        [Parameter(HelpMessage = "Repository name")]
        public string Repository { get; set; }

        [Parameter(ParameterSetName = "All")]
        [Parameter(ParameterSetName = "Branch")]
        [Parameter(ParameterSetName = "Path", Mandatory = true)]
        [Parameter(HelpMessage = "Path to repository")]
        public string Path { get; set; }

        [ArgumentCompleter(typeof(ContainerArgumentCompleter))]
        [Parameter(ParameterSetName = "All")]
        [Parameter(ParameterSetName = "Branch")]
        [Parameter(ParameterSetName = "Container", Mandatory = true)]
        [Parameter(HelpMessage = "Path to container")]
        public string Container { get; set; }

        [Parameter(ParameterSetName = "Branch", Mandatory = true, HelpMessage = "Name of remote branch")]
        public string Branch { get; set; }

        [Parameter(ParameterSetName = "All", Mandatory = true)]
        public SwitchParameter All { get; set; }

        [Parameter()]
        public SwitchParameter Silent { get; set; }

        #endregion

        private Configuration Configuration { get; set; }

        private string GitFolder { get; set; }

        private string CWD { get; set; }

        protected override void BeginProcessing()
        {
            var configurationManager = new ConfigurationManager();
            Configuration = configurationManager.Configuration;

            Container = MyInvocation.BoundParameters.ContainsKey("Path")
                 ? System.IO.Path.GetFullPath(Container)
                 : Configuration.Container
                    .Where(repo => repo.IsDefault)
                    .Select(repo => repo.Path)
                    .First();

            if (string.IsNullOrEmpty(Repository))
            {
                CWD = this.CurrentProviderLocation("FileSystem").ProviderPath;
                Repository = new DirectoryInfo(CWD).Name;
            }

            GitFolder = System.IO.Path.Combine(Container, Repository, ".git");
        }

        protected override void ProcessRecord()
        {
            try
            {
                switch(this.ParameterSetName)
                {
                    case "All":
                        Git.TrackAllBranches(GitFolder, Silent.IsPresent);
                        break;
                    case "Branch":
                        Git.TrackBranch(GitFolder, Branch, Silent.IsPresent);
                        break;
                }
            }
            catch (FileNotFoundException exception)
            {
                WriteError(new ErrorRecord(exception, "Not a Git Repository", ErrorCategory.ObjectNotFound, GitFolder));
            }
        }

        protected override void EndProcessing()
        {

        }
    }
}
