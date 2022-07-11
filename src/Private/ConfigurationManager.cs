using System;
using System.IO;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace RepoManager
{
    internal sealed class ConfigurationManager
    {
        public const string ModuleName = "RepoManager";

        public string ConfigurationDirectory
        {
            get
            {
                string configurationDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ModuleName);

                if (!Directory.Exists(configurationDirectory))
                {
                    Directory.CreateDirectory(configurationDirectory);
                }

                return configurationDirectory;
            }
        }

        public string ConfigurationPath => Path.Combine(ConfigurationDirectory, "config.json");

        public JsonSerializerSettings JsonSettings
        {
            get
            {
                return new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore,
                    Formatting = Formatting.Indented
                };
            }
        }

        public Configuration Configuration
        {
            get
            {
                return JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(ConfigurationPath), JsonSettings);
            }
        }

        public ConfigurationManager()
        {
            string configurationPath = ConfigurationPath;

            if (!File.Exists(configurationPath))
            {
                var configuration = new Configuration
                {
                    Container = new List<RepositoryContainer> { new RepositoryContainer() },
                    Protocol = Protocol.HTTPS,
                    Provider = Provider.GitHub
                };

                string data = JsonConvert.SerializeObject(configuration, JsonSettings);
                File.WriteAllText(configurationPath, data);
            }
        }
    }
}
