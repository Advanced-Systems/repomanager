using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace RepoManager
{
    internal class Repository
    {
        public string Name { get; set; }

        public string Container { get; set; }

        public string Path { get; set; }

        public string GitPath { get; set; }

        public int Size { get; set; }

        public string Remote { get; set; }

        public string DefaultBranch { get; set; }

        public string ActiveBranch { get; set; }

        public List<string> Language { get; set; }

        public string License { get; set; }

        public int FileCount { get; set; }

        public int TotalFileCount { get; set; }

        public List<string> Contributors { get; set; }

        public int CommitCount { get; set; }

        public int NewCommitCount { get; set; }

        public Commit LastCommit { get; set; }

        private IEnumerable<string> Files { get; set; }

        public Repository(string name, string container)
        {
            Name = name;
            Container = container;
            Path = System.IO.Path.Combine(container, name);
            GitPath = System.IO.Path.Combine(container, name, ".git");
            Files = Directory.EnumerateFiles(Path, "*.*", SearchOption.AllDirectories);
            // FileCount
            TotalFileCount = Files.Count();
            LastCommit = Git.GetLastCommit(GitPath);
        }
    }
}
