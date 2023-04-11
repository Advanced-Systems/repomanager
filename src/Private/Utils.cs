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
        public static void DeleteDirectoryRecursively(string path)
        {
            var directory = new DirectoryInfo(path) { Attributes = FileAttributes.Normal };
            foreach (var file in directory.GetFileSystemInfos("*", SearchOption.AllDirectories)) file.Attributes = FileAttributes.Normal;
            Directory.Delete(path, recursive: true);
        }

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
                    foreach (var repo in bitbucket) repositoryNames.Add(repo["name"].ToString());
                    break;
                case Provider.GitHub:
                default:
                    response = Task.Run(() => client.GetStringAsync($"https://api.github.com/users/{username}/repos")).Result;
                    var github = JArray.Parse(response);
                    foreach (var repo in github) repositoryNames.Add(repo["name"].ToString());
                    break;
            }

            return repositoryNames;
        }
    }
}
