using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RepoManager
{
    #region type definitions for the Git and Repository classes

    internal enum Scope
    {
        Local,
        Global,
        System
    }

    public struct Remote
    {
        public string Push { get; set; }

        public string Fetch { get; set; }

        public override string ToString() => Fetch.Equals(Push) ? Push : $"{Push} ({Fetch})";
    }

    public struct Author
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public override string ToString() => $"{Name} ({Email})";
    }

    public struct Commit
    {
        public Author Author { get; set; }

        public string Message { get; set; }

        public DateTime DateTime { get; set; }

        public string Hash { get; set; }

        public override string ToString() => $"{Author.Name}@{DateTime.ToShortDateString()}: {Message}";
    }

    #endregion

    #region serializable types for the configuration file

    public enum Provider
    {
        GitHub,
        GitLab,
        BitBucket
    }

    public enum Protocol
    {
        HTTPS,
        SSH
    }

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

    [Serializable]
    internal sealed class Configuration
    {
        public List<RepositoryContainer> Container { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Protocol Protocol { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Provider Provider { get; set; }
    }

    #endregion
}
