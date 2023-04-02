using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace RepoManager
{
    internal sealed class Repository
    {
        #region Public Properties

        public string Name { get; }

        public string Container { get; }

        public string Directory { get; }

        public string Path { get; }

        public long Size { get; }

        public Remote Remote { get; }

        public string DefaultBranch { get; }

        public string CurrentBranch { get; }

        //public List<string> Language { get; }

        //public string License { get; }

        public int FileCount { get; }

        public int TotalFileCount { get; }

        public List<Author> Authors { get; }

        public int CommitCount { get; }

        public int NewCommitCount { get; }

        public Commit LastCommit { get; }

        #endregion

        public Repository(string name, string container)
        {
            Name = name;
            Container = container;
            Directory = System.IO.Path.Combine(container, name);
            Path = System.IO.Path.Combine(container, name, ".git");

            var directoryInfo = new DirectoryInfo(Directory);
            var files = directoryInfo.EnumerateFiles("*", SearchOption.AllDirectories);

            Size = files.Sum(f => f.Length);
            Remote = Git.GetRemote(Path);
            DefaultBranch = Git.GetDefaultBranch(Path);
            CurrentBranch = Git.GetCurrentBranch(Path);
            // TODO: Language
            // TODO: License
            FileCount = Git.GetTrackedFiles(Path, CurrentBranch).Count();
            TotalFileCount = files.Count();
            Authors = Git.GetAuthors(Path).ToList();
            CommitCount = Git.GetCommitCount(Path, DefaultBranch);
            NewCommitCount = Git.GetCommitCount(Path, CurrentBranch) - CommitCount;
            LastCommit = Git.GetLastCommit(Path);
        }
    }
}
