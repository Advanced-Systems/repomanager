using System;

namespace RepoManager
{
    [Serializable]
    internal sealed class RepositoryContainer
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public bool IsDefault { get; set; }

        public RepositoryContainer()
        {
            Name = "repos";
            Path = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), Name);
            IsDefault = true;
        }

        public RepositoryContainer(string name, string path, bool isDefault)
        {
            Name = name;
            Path = path;
            IsDefault = isDefault;
        }
    }
}
