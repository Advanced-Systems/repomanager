using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

using Octokit;

namespace RepoManager
{
    internal static class Utils
    {
        public static string RemoveLineBreaks(this string @string)
        {
            return @string.Replace("\r\n", string.Empty).Replace("\r", string.Empty).Replace("\n", string.Empty);
        }

        public static IEnumerable<T> ForEach<T>(this IEnumerable<T> enumeration, Action<T> action)
        {
            foreach (T item in enumeration)
            {
                action(item);
            }

            return enumeration;
        }

        public static void DeleteDirectoryRecursively(string path)
        {
            foreach (string file in Directory.EnumerateFiles(path, "*.*", SearchOption.AllDirectories))
            {
                File.SetAttributes(file, FileAttributes.Normal);
                File.Delete(file);
            }

            foreach (string directory in Directory.EnumerateDirectories(path))
            {
                var directoryInfo = new DirectoryInfo(directory);
                directoryInfo.Attributes |= FileAttributes.Normal;
                Directory.Delete(directory, recursive: true);
            }

            Directory.Delete(path);
        }

        public static async Task<IEnumerable<string>> GetAllRepositoryNames(string username)
        {
            var github = new GitHubClient(new Octokit.ProductHeaderValue(ConfigurationManager.ModuleName));
            var repositories = await github.Repository.GetAllForUser(username);
            return repositories.Select(repo => repo.Name);
        }
    }
}
