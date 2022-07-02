using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Management.Automation;

namespace RepoManager
{
    [Alias("clone")]
    [Cmdlet(VerbsData.Import, "Repository")]
    public class ImportRepositoryCommand : PSCmdlet
    {
        [Parameter(Position = 0, Mandatory = true, ValueFromPipeline = true, HelpMessage = "Repository URI")]
        public List<string> Uri { get; set; }

        [Parameter(HelpMessage = "Path of a repository container")]
        public string Path { get; set; }

        [Parameter()]
        public SwitchParameter TrackAllBranches { get; set; }

        private Configuration Configuration { get; set; }

        private bool Exists { get; set; } = false;

        protected override void BeginProcessing()
        {
            ConfigurationManager.Init();
            Configuration = ConfigurationManager.Configuration;

            Path = MyInvocation.BoundParameters.ContainsKey("Path")
                 ? System.IO.Path.GetFullPath(Path)
                 : Configuration.Container
                    .Where(repo => repo.IsDefault)
                    .Select(repo => repo.Path)
                    .First();
        }

        protected override void ProcessRecord()
        {
            foreach (string u in Uri)
            {
                string repoName = u.Split('/').Last().Split('.')[0];
                string repoPath = System.IO.Path.Combine(Path, repoName);
                string gitPath = System.IO.Path.Combine(repoPath, ".git");

                if (!Directory.Exists(repoPath) || Directory.GetFiles(repoPath).Count() == 0)
                {
                    WriteVerbose($"Cloning '{u}' into '{repoPath}' . . .");
                    Git.CloneRepository(u, repoPath);
                    Exists = true;

                    if (TrackAllBranches.IsPresent && Directory.Exists(gitPath))
                    {
                        Git.TrackAllBranches(gitPath, WriteVerbose, WriteWarning);
                    }
                }
                else
                {
                    WriteWarning($"Destination path '{repoPath}' already exists and is not an empty directory");
                }
            }
        }

        protected override void EndProcessing()
        {
            if (Exists)
            {
                // todo: invoke get-repository here
            }
        }
    }
}
