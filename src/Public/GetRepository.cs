using System;
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
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "Name", HelpMessage = "Repository name")]
        public List<string> Name { get; set; }

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
                Name = System.IO.Directory.GetDirectories(Path).ToList();
                Name = Name.Select(p => System.IO.Path.GetFileName(p)).ToList();

                foreach (var name in Name)
                {
                    var repository = new Repository(name, container: Path);
                    WriteObject(repository);
                }
            }
            else
            {
                var repository = new Repository(Name.First(), container: Path);
                WriteObject(repository);
            }
        }

        protected override void EndProcessing()
        {
            // TODO
        }
    }
}
