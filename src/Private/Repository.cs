using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace RepoManager
{
    internal sealed class Repository
    {
        public string Name { get; set; }

        public string Container { get; set; }

        public string Path { get; set; }

        public string GitPath { get; set; }

        public long Size { get; set; }

        public string Remote { get; set; }

        public string DefaultBranch { get; set; }

        public string ActiveBranch { get; set; }

        public List<string> Language { get; set; }

        public string License { get; set; }

        public int FileCount { get; set; }

        public int TotalFileCount { get; set; }

        public List<Author> Authors { get; set; }

        public int CommitCount { get; set; }

        public int NewCommitCount { get; set; }

        public Commit LastCommit { get; set; }

        private DirectoryInfo DirectoryInfo { get; set; }

        private IEnumerable<FileInfo> Files { get; set; }

        public Repository(string name, string container)
        {
            Name = name;
            Container = container;
            Path = System.IO.Path.Combine(container, name);
            GitPath = System.IO.Path.Combine(container, name, ".git");
            DirectoryInfo = new DirectoryInfo(Path);
            Files = DirectoryInfo.EnumerateFiles("*", SearchOption.AllDirectories);
            Size = Files.Sum(file => file.Length);
            DefaultBranch = Git.GetDefaultBranch(GitPath);
            ActiveBranch = Git.GetActiveBranch(GitPath);
            // FileCount
            TotalFileCount = Files.Count();
            Authors = Git.GetAuthors(GitPath).ToList();
            CommitCount = Git.GetCommitCount(GitPath, DefaultBranch);
            NewCommitCount = Git.GetCommitCount(GitPath, ActiveBranch) - CommitCount;
            LastCommit = Git.GetLastCommit(GitPath);
        }
    }
}
