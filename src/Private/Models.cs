using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace RepoManager
{
    internal enum Protocol
    {
        HTTP,
        HTTPS,
        SSH
    }

    [Serializable]
    internal class RepositoryContainer
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
    internal class Configuration
    {
        public List<RepositoryContainer> Container { get; set; } = new List<RepositoryContainer>() { new RepositoryContainer() };

        [JsonConverter(typeof(StringEnumConverter))]
        public Protocol Protocol { get; set; } = Protocol.SSH;
    }
}
