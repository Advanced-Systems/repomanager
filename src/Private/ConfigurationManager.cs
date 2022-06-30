using System;
using System.IO;
using Newtonsoft.Json;

namespace RepoManager
{
    internal static class ConfigurationManager
    {
        public static string ConfigurationDirectory
        {
            get
            {
                string configurationDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "RepoManager");

                if (!Directory.Exists(configurationDirectory))
                {
                    Directory.CreateDirectory(configurationDirectory);
                }

                return configurationDirectory;
            }
        }

        public static string ConfigurationPath => Path.Combine(ConfigurationDirectory, "config.json");

        public static JsonSerializerSettings JsonSettings => new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, Formatting = Formatting.Indented };

        public static Configuration Configuration => JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(ConfigurationPath), JsonSettings);

        public static void Init()
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
