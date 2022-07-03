using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace RepoManager
{
    internal class Repository
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public string GitPath { get; set; }

        private string Container { get; set; }

        // TODO: Size

        public string Remote { get; set; }

        public string DefaultBranch { get; set; }

        public string ActiveBranch { get; set; }

        // TODO: (Programming) Language(s)

        // TODO: License Name

        public int TotalFileCount { get; set; }

        public List<string> Contributors { get; set; }

        public int CommitCount { get; set; }

        public int NewCommitCount { get; set; }

        public string LastCommitMessage { get; set; }

        public string LastCommitHash { get; set; }

        public string LastCommitAuthor { get; set; }

        public DateTime LastCommitDate { get; set; }

        public Repository()
        {
            // TODO
        }
    }
}
