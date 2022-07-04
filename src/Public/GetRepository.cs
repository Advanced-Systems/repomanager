using System;
using System.Linq;
using System.Collections.Generic;

using System.Management.Automation;

namespace RepoManager
{
    [Alias("grepo")]
    [Cmdlet(VerbsCommon.Get, "Repository")]
    public class GetRepositoryCommand : PSCmdlet
    {
        [ValidateNotNullOrEmpty()]
        [Parameter(Mandatory = true, ValueFromPipeline = true, ParameterSetName = "Name", HelpMessage = "Repository name")]
        public List<string> Name { get; set; }

        [Parameter(ParameterSetName = "Name", HelpMessage = "Path to repository container")]
        public string Path { get; set; }

        [Parameter(ParameterSetName = "All")]
        public SwitchParameter All { get; set; }

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
                foreach (var name in Name)
                {
                    WriteObject($"This is all!, {name}");
                }
            }
            else
            {
                WriteObject($"Name is {string.Join(Environment.NewLine, Name)}");
            }
        }

        protected override void EndProcessing()
        {
            // TODO
        }
    }
}
