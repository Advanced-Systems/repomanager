using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace RepoManager
{
    internal sealed class ConfigurationManager
    {
        public const string ModuleName = "RepoManager";

        public void SetConsoleEncoding()
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            Console.OutputEncoding = this.Configuration.Language switch
            {
                Language.Japanese => Encoding.GetEncoding(65001),
                Language.Chinese => Encoding.GetEncoding(50227),
                Language.German => Encoding.GetEncoding(1252),
                _ => Encoding.Default
            };
        }

        public static string Folder
        {
            get
            {
                string moduleFolder = Environment.OSVersion.Platform switch
                {
                    PlatformID.Win32NT => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), ModuleName),
                    /* Unix */ _ => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), $".{ModuleName.ToLower()}"),
                };

                if (!Directory.Exists(moduleFolder)) Directory.CreateDirectory(moduleFolder);

                return moduleFolder;
            }
        }

        public string ConfigurationPath => System.IO.Path.Combine(Folder, "config.json");

        public JsonSerializerSettings ConfigurationSerializer
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
                return JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(ConfigurationPath), ConfigurationSerializer);
            }
        }

        public ConfigurationManager()
        {
            if (!File.Exists(ConfigurationPath))
            {
                var configuration = new Configuration
                {
                    Container = new List<RepositoryContainer> { new RepositoryContainer() },
                    Protocol = Protocol.HTTPS,
                    Provider = Provider.GitHub,
                    Language = Language.English,
                };

                string data = JsonConvert.SerializeObject(configuration, ConfigurationSerializer);
                File.WriteAllText(ConfigurationPath, data);
            }

            SetConsoleEncoding();
        }
    }
}
