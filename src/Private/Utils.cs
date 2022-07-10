using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net.Http;

using Newtonsoft.Json.Linq;

namespace RepoManager
{
    internal static class Utils
    {
        public static string RemoveLineBreaks(this string @string) => @string.Replace("\r\n", string.Empty).Replace("\r", string.Empty).Replace("\n", string.Empty);

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> @this, Action<T> action)
        {
            _ = action ?? throw new ArgumentNullException(nameof(action));

            foreach (T item in @this)
            {
                action(item);
            }

            return @this;
        }

        public static void DeleteDirectoryRecursively(string path)
        {
            var directory = new DirectoryInfo(path) { Attributes = FileAttributes.Normal };
            directory.GetFileSystemInfos("*", SearchOption.AllDirectories).ForEach(file => file.Attributes = FileAttributes.Normal);
            Directory.Delete(path, recursive: true);
        }

        public static DateTime ConvertToDateTime(double unixTimeStamp) => new DateTime(1970, 1, 1, 0, 0, 0).AddSeconds(unixTimeStamp);

        public static List<string> GetAllRepositoryNames(string username, Provider provider = Provider.GitHub)
        {
            var repositoryNames = new List<string>();
            string response = string.Empty;
            var client = new HttpClient();
            client.DefaultRequestHeaders.UserAgent.TryParseAdd(ConfigurationManager.ModuleName);

            switch (provider)
            {
                case Provider.GitLab:
                    throw new NotImplementedException();
                case Provider.BitBucket:
                    response = Task.Run(() => client.GetStringAsync($"https://bitbucket.org/api/2.0/repositories/{username}")).Result;
                    var bitbucket = JObject.Parse(response)["values"];
                    bitbucket.ForEach(repo => repositoryNames.Add(repo["name"].ToString()));
                    break;
                default:
                    response = Task.Run(() => client.GetStringAsync($"https://api.github.com/users/{username}/repos")).Result;
                    var github = JArray.Parse(response);
                    github.ForEach(repo => repositoryNames.Add(repo["name"].ToString()));
                    break;
            }

            return repositoryNames;
        }
    }
}
