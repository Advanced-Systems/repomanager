using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using System.Management.Automation;
using System.Threading.Tasks;

namespace RepoManager
{
    [Alias("clone")]
    [Cmdlet(VerbsData.Import, "Repository")]
    public class ImportRepositoryCommand : PSCmdlet
    {
        [ValidateNotNullOrEmpty()]
        [Parameter(Position = 0, Mandatory = true, ValueFromPipeline = true, ParameterSetName = "Uri", HelpMessage = "Repository URI")]
        public List<string> Uri { get; set; }

        [Parameter(HelpMessage = "Path to repository container")]
        public string Path { get; set; }

        [Parameter()]
        public SwitchParameter TrackAllBranches { get; set; }

        [ValidateNotNullOrEmpty()]
        [Parameter(Mandatory = true, ParameterSetName = "All")]
        public SwitchParameter All { get; set; }

        [Parameter(ParameterSetName = "User", HelpMessage = "GitHub user name")]
        [Parameter(ParameterSetName = "All", HelpMessage = "GitHub user name")]
        public string User { get; set; }

        [ValidateNotNullOrEmpty()]
        [Parameter(Mandatory = true, ParameterSetName = "User", HelpMessage = "Repository name")]
        public string Name { get; set; }

        [ValidateSet("SSH", "HTTPS")]
        [Parameter(ParameterSetName = "User", HelpMessage = "Clone protocol")]
        [Parameter(ParameterSetName = "All", HelpMessage = "Clone protocol")]
        public string Protocol { get; set; }

        private string Hostname { get; set; }

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

            User = !MyInvocation.BoundParameters.ContainsKey("Uri") && !MyInvocation.BoundParameters.ContainsKey("User")
                 ? Git.GetConfig("user.name", Scope.Global)
                 : User;

            Protocol = MyInvocation.BoundParameters.ContainsKey("Protocol")
                     ? Protocol
                     : Configuration.Protocol.ToString();

            Hostname = Protocol.Equals("SSH") ? "git@github.com:" : "https://github.com/";
        }

        protected override void ProcessRecord()
        {
            if (All.IsPresent)
            {
                var repoNames = Task.Run(() => Utils.GetAllRepositoryNames(User)).Result;
                int count = repoNames.Count();
                int activityId = 0;

                foreach (string repoName in repoNames)
                {
                    string uri = $"{Hostname}{User}/{repoName}.git";
                    string repoPath = System.IO.Path.Combine(Path, repoName);
                    int percent = Convert.ToInt32(Math.Round(activityId * 100F / count));

                    var progress = new ProgressRecord(activityId, uri, $"Cloning {repoName} to '{Path}' . . .");
                    progress.PercentComplete = percent;
                    progress.StatusDescription = $"{percent}%";

                    WriteProgress(progress);

                    if (!Directory.Exists(repoPath) || !Directory.EnumerateFileSystemEntries(repoPath).Any())
                    {
                        Git.CloneRepository(uri, repoPath);
                    }
                    else
                    {
                        WriteWarning($"Destination path '{repoPath}' already exists and is not an empty directory");
                    }

                    if (TrackAllBranches.IsPresent)
                    {
                        Git.TrackAllBranches(System.IO.Path.Combine(repoPath, ".git"), WriteVerbose, WriteWarning);
                    }

                    // TODO: return repository object
                    activityId++;
                }
            }
            else if (!string.IsNullOrEmpty(User))
            {
                // TODO: return repository object
                string uri = $"{Hostname}/{User}/{Name}.git";
                string repoPath = System.IO.Path.Combine(Path, Name);

                WriteVerbose($"Cloning '{Name}' into '{repoPath}' . . .");
                Git.CloneRepository(uri, repoPath);

                if (TrackAllBranches.IsPresent)
                {
                    Git.TrackAllBranches(repoPath, WriteVerbose, WriteWarning);
                }
            }
            else
            {
                foreach (string u in Uri)
                {
                    string repoName = u.Split('/').Last().Split('.').First();
                    string repoPath = System.IO.Path.Combine(Path, repoName);
                    string gitPath = System.IO.Path.Combine(repoPath, ".git");

                    if (!Directory.Exists(repoPath) || !Directory.EnumerateFileSystemEntries(repoPath).Any())
                    {
                        WriteVerbose($"Cloning '{u}' into '{repoPath}' . . .");
                        Git.CloneRepository(u, repoPath);

                        if (TrackAllBranches.IsPresent && Directory.Exists(gitPath))
                        {
                            Git.TrackAllBranches(gitPath, WriteVerbose, WriteWarning);
                        }

                        // TODO: return repository object
                    }
                    else
                    {
                        WriteWarning($"Destination path '{repoPath}' already exists and is not an empty directory");
                    }
                }
            }
        }

        protected override void EndProcessing()
        {

        }
    }
}
