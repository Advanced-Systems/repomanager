using System;

namespace RepoManager
{
    public struct Commit
    {
        public Author Author { get; set; }

        public string Message { get; set; }

        public DateTime DateTime { get; set; }

        public string Hash { get; set; }

        public override string ToString() => $"{Author.Name}@{DateTime.ToShortDateString()}: {Message}";
    }
}
