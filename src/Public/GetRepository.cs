using System.IO;
using System.Linq;
using System.Collections.Generic;

using System.Management.Automation;

namespace RepoManager
{
    [Alias("grepo")]
    [OutputType(typeof(Repository), typeof(List<Repository>))]
    [Cmdlet(VerbsCommon.Get, "Repository")]
    public class GetRepositoryCommand : PSCmdlet
    {
        [Parameter(ParameterSetName = "All")]
        public SwitchParameter All { get; set; }

        [ValidateNotNullOrEmpty()]
        [ArgumentCompleter(typeof(NameArgumentCompleter))]
        [Parameter(Position = 0, Mandatory = true, ValueFromPipeline = true, ParameterSetName = "Name", HelpMessage = "Repository name")]
        public List<string> Name { get; set; }

        [ArgumentCompleter(typeof(PathArgumentCompleter))]
        [Parameter(ParameterSetName = "Name", HelpMessage = "Path to repository container")]
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
            if (All.IsPresent)
            {
                foreach (var name in Directory.GetDirectories(Path).Select(System.IO.Path.GetFileName))
                {
                    var repository = new Repository(name, container: Path);
                    WriteObject(repository);
                }
            }
            else
            {
                foreach (var name in Name)
                {
                    try
                    {
                        var repository = new Repository(name, container: Path);
                        WriteObject(repository);
                    }
                    catch (DirectoryNotFoundException exception)
                    {
                        var repoPath = System.IO.Path.Combine(Path, name);
                        WriteError(new ErrorRecord(exception, "Directory not found", ErrorCategory.ObjectNotFound, repoPath));
                        continue;
                    }
                }
            }
        }

        protected override void EndProcessing()
        {

        }
    }
}
