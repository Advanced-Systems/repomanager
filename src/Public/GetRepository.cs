﻿using System.IO;
using System.Linq;
using System.Collections.Generic;

using System.Management.Automation;

namespace RepoManager
{
    [Alias("repo")]
    [OutputType(typeof(Repository), typeof(List<Repository>))]
    [Cmdlet(VerbsCommon.Get, "Repository")]
    public class GetRepositoryCommand : PSCmdlet
    {
        #region parameters

        [ValidateNotNullOrEmpty()]
        [ArgumentCompleter(typeof(RepositoryArgumentCompleter))]
        [Parameter(ParameterSetName = "Repository", Position = 0, Mandatory = true, ValueFromPipeline = true, HelpMessage = "Repository name")]
        public List<string> Repository { get; set; }

        [ArgumentCompleter(typeof(ContainerArgumentCompleter))]
        [Parameter(ParameterSetName = "Repository", HelpMessage = "Path to container")]
        public string Container { get; set; }

        [Parameter(ParameterSetName = "All")]
        public SwitchParameter All { get; set; }

        #endregion

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
        }

        protected override void ProcessRecord()
        {
            if (All.IsPresent)
            {
                foreach (var name in Directory.GetDirectories(Container).Select(System.IO.Path.GetFileName))
                {
                    var repository = new Repository(name, Container);
                    WriteObject(repository);
                }
            }
            else
            {
                foreach (var name in Repository)
                {
                    try
                    {
                        var repository = new Repository(name, Container);
                        WriteObject(repository);
                    }
                    catch (DirectoryNotFoundException exception)
                    {
                        var repoPath = System.IO.Path.Combine(Container, name);
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
