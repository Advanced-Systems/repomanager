using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using System.Management.Automation;

namespace RepoManager
{
    [Alias("clone")]
    [Cmdlet(VerbsData.Import, "Repository", DefaultParameterSetName = "Container", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.Low)]
    public class ImportRepositoryCommand : PSCmdlet
    {
        #region parameters

        [ValidateNotNullOrEmpty()]
        [Parameter(ParameterSetName = "Uri", Position = 0, Mandatory = true, ValueFromPipeline = true, HelpMessage = "Repository URI")]
        public List<string> Uri { get; set; }

        [ArgumentCompleter(typeof(ContainerArgumentCompleter))]
        [Parameter(ParameterSetName = "Container", HelpMessage = "Path to container")]
        public string Container { get; set; }

        [Parameter(ParameterSetName = "Container", Mandatory = true, HelpMessage = "Repository name")]
        public string Repository { get; set; }

        [Parameter(ParameterSetName = "All")]
        [Parameter(ParameterSetName = "Container")]
        [Parameter(HelpMessage = "Git user name")]
        public string User { get; set; }

        [Parameter(ParameterSetName = "All")]
        [Parameter(ParameterSetName = "Container")]
        [Parameter(HelpMessage = "Hosting service")]
        public Provider Provider { get; set; }

        [Parameter(ParameterSetName = "All")]
        [Parameter(ParameterSetName = "Container")]
        [Parameter(HelpMessage = "Network protocol")]
        public Protocol Protocol { get; set; }

        [Parameter(ParameterSetName = "All", Mandatory = true)]
        public SwitchParameter All { get; set; }

        [Parameter()]
        public SwitchParameter TrackAllBranches { get; set; }

        [Parameter()]
        public SwitchParameter Silent { get; set; }

        #endregion

        private string TopLevelDomain { get; set; }

        private string Hostname { get; set; }

        private Configuration Configuration { get; set; }

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

            User = (MyInvocation.BoundParameters.ContainsKey("Repository") || MyInvocation.BoundParameters.ContainsKey("All")) && !MyInvocation.BoundParameters.ContainsKey("User")
                 ? Git.GetConfig("user.name", Scope.Global)
                 : User;

            Protocol = MyInvocation.BoundParameters.ContainsKey("Protocol")
                     ? Protocol
                     : Configuration.Protocol;

            Provider = MyInvocation.BoundParameters.ContainsKey("Provider")
                     ? Provider
                     : Configuration.Provider;

            TopLevelDomain = Provider is Provider.BitBucket ? "org" : "com";

            Hostname = Protocol is Protocol.HTTPS
                     ? $"https://{Provider.ToString().ToLower()}.{TopLevelDomain}/"
                     : $"git@{Provider.ToString().ToLower()}.{TopLevelDomain}:";
        }

        protected override void ProcessRecord()
        {
            if (All.IsPresent)
            {
                var repoNames = Utils.GetAllRepositoryNames(User, Provider);
                int count = repoNames.Count();
                int activityId = 0;

                foreach (string repoName in repoNames)
                {
                    string uri = $"{Hostname}{User}/{repoName}.git";
                    string repoPath = System.IO.Path.Combine(Container, repoName);
                    int percent = Convert.ToInt32(Math.Round(activityId * 100F / count));

                    var progress = new ProgressRecord(activityId, uri, $"Cloning {repoName} to '{Container}' . . .");
                    progress.StatusDescription = $"{percent}%";
                    progress.PercentComplete = percent;

                    WriteProgress(progress);

                    if (ShouldProcess(uri, $"Clone repository {repoName} to '{Container}'"))
                    {
                        Git.CloneRepository(uri, repoPath, Silent.IsPresent, WriteWarning);

                        if (TrackAllBranches.IsPresent)
                        {
                            Git.TrackAllBranches(System.IO.Path.Combine(repoPath, ".git"));
                        }
                    }

                    activityId++;
                }
            }
            else if (MyInvocation.BoundParameters.ContainsKey("Repository"))
            {
                string uri = $"{Hostname}{User}/{Repository}.git";
                string repoPath = System.IO.Path.Combine(Container, Repository);

                if (ShouldProcess(uri, $"Clone repository {Repository} to '{Container}'"))
                {
                    Git.CloneRepository(uri, repoPath, Silent.IsPresent, WriteWarning);

                    if (TrackAllBranches.IsPresent)
                    {
                        Git.TrackAllBranches(repoPath, Silent.IsPresent);
                    }
                }
            }
            else
            {
                foreach (string u in Uri)
                {
                    string repoName = u.Split('/').Last().Split('.').First();
                    string repoPath = System.IO.Path.Combine(Container, repoName);
                    string gitPath = System.IO.Path.Combine(repoPath, ".git");

                    if (ShouldProcess(u, $"Clone repository {repoName} to '{Container}'"))
                    {
                        Git.CloneRepository(u, repoPath, Silent.IsPresent, WriteWarning);

                        if (TrackAllBranches.IsPresent && Directory.Exists(gitPath))
                        {
                            Git.TrackAllBranches(gitPath, Silent.IsPresent);
                        }
                    }
                }
            }
        }

        protected override void EndProcessing()
        {

        }
    }
}
