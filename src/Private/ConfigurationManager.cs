using System;
using System.IO;

using Newtonsoft.Json;

namespace RepoManager
{
    internal class ConfigurationManager
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
                string data = JsonConvert.SerializeObject(new Configuration(), JsonSettings);
                File.WriteAllText(configurationPath, data);
            }
        }
    }
}
