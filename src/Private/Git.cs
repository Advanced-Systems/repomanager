using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using CliWrap;
using CliWrap.Buffered;

namespace RepoManager
{
    internal sealed class Git
    {
        #region configuration

        public static async Task<string> GetConfigAsync(string key, Scope scope)
        {
            var configTask = await Cli.Wrap("git")
                .WithArguments(new string[] { "config", $"--{scope.ToString().ToLower()}", key })
                .ExecuteBufferedAsync();

            return configTask.StandardOutput.RemoveLineBreaks();
        }

        public static string GetConfig(string key, Scope scope) => GetConfigAsync(key, scope).GetAwaiter().GetResult();

        public static async Task<Remote> GetRemoteAsync(string workingDirectory)
        {
            var remoteTask = await Cli.Wrap("git")
                .WithWorkingDirectory(workingDirectory)
                .WithArguments(new string[] { "remote", "--verbose" })
                .ExecuteBufferedAsync();

            var standardOutput = remoteTask.StandardOutput
                .Split(new char[] { '\t', ' ' })
                .Where(line => !line.Contains("origin") && !line.Contains('('));

            return new Remote
            {
                Fetch = standardOutput.First().ToString(),
                Push = standardOutput.Last().ToString()
            };
        }

        public static Remote GetRemote(string workingDirectory) => GetRemoteAsync(workingDirectory).GetAwaiter().GetResult();

        #endregion

        #region repository

        public static async Task FetchAsync(string workingDirectory, bool all = false)
        {
            var fetchTask = await Cli.Wrap("git")
                .WithWorkingDirectory(workingDirectory)
                .WithArguments(new string[] { "fetch", all ? "--all" : string.Empty })
                .ExecuteAsync();
        }

        public static void Fetch(string workingDirectory, bool all = false) => FetchAsync(workingDirectory, all).GetAwaiter().GetResult();

        public static async Task CloneRepositoryAsync(string uri, string path, Action<string> verboseAction = null, Action<string> warningAction = null)
        {
            if (!Directory.Exists(path) || !Directory.EnumerateFileSystemEntries(path).Any())
            {
                if (verboseAction != null)
                {
                    verboseAction($"Cloning '{uri}' into '{path}' . . .");
                }

                var cloneTask = await Cli.Wrap("git")
                    .WithArguments(new string[] { "clone", uri, path, "--quiet" })
                    .ExecuteAsync();
            }
            else
            {
                if (warningAction != null)
                {
                    warningAction($"Destination path '{path}' already exists and is not an empty directory");
                }
            }
        }

        public static void CloneRepository(string uri, string path, Action<string> verboseAction = null, Action<string> warningAction = null) =>
            CloneRepositoryAsync(uri, path, verboseAction, warningAction).GetAwaiter().GetResult();

        public static async Task<IEnumerable<string>> GetTrackedFilesAsync(string workingDirectory, string branch)
        {
            var command = await Cli.Wrap("git")
                .WithWorkingDirectory(workingDirectory)
                .WithArguments(new string[] { "ls-tree", "-r", branch, "--name-only" })
                .ExecuteBufferedAsync();

            return command.StandardOutput.Split(Environment.NewLine.ToArray()).SkipLast(1);
        }

        public static IEnumerable<string> GetTrackedFiles(string workingDirectory, string branch) => GetTrackedFilesAsync(workingDirectory, branch).GetAwaiter().GetResult();

        #endregion

        #region branches

        public static async Task<IEnumerable<string>> GetLocalBranchesAsync(string workingDirectory)
        {
            var branchTask = await Cli.Wrap("git")
                .WithWorkingDirectory(workingDirectory)
                .WithArguments(new string[] { "branch", "--format=%(refname:short)" })
                .ExecuteBufferedAsync();

            return branchTask.StandardOutput.Split(Environment.NewLine.ToArray(), StringSplitOptions.RemoveEmptyEntries);
        }

        public static IEnumerable<string> GetLocalBranches(string workingDirectory) => GetLocalBranchesAsync(workingDirectory).GetAwaiter().GetResult();

        public static async Task<string> GetDefaultBranchAsync(string workingDirectory)
        {
            var branchTask = await Cli.Wrap("git")
                .WithWorkingDirectory(workingDirectory)
                .WithArguments(new string[] { "remote", "show", "origin" })
                .ExecuteBufferedAsync();

            var standardOutput = branchTask.StandardOutput.Split(Environment.NewLine.ToArray());

            return standardOutput
                .Where(line => line.Contains("HEAD branch"))
                .First()
                .Split(":")
                .Last()
                .Trim()
                .RemoveLineBreaks();
        }

        public static string GetDefaultBranch(string workingDirectory) => GetDefaultBranchAsync(workingDirectory).GetAwaiter().GetResult();

        public static async Task<string> GetCurrentBranchAsync(string workingDirectory)
        {
            var command = await Cli.Wrap("git")
                .WithWorkingDirectory(workingDirectory)
                .WithArguments(new string[] { "branch", "--show-current" })
                .ExecuteBufferedAsync();

            return command.StandardOutput.RemoveLineBreaks();
        }

        public static string GetCurrentBranch(string workingDirectory) => GetCurrentBranchAsync(workingDirectory).GetAwaiter().GetResult();

        public static async Task<IEnumerable<string>> GetRemoteBranchesAsync(string workingDirectory, bool fetchAll = true)
        {
            if (fetchAll)
            {
                await FetchAsync(workingDirectory, all: true);
            }

            var branchTask = await Cli.Wrap("git")
                .WithWorkingDirectory(workingDirectory)
                .WithArguments(new string[] { "branch", "--remote", "--format=%(refname:lstrip=3)" })
                .ExecuteBufferedAsync();

            var branchFilter = new List<string> {
                "HEAD",
                "master",
                "main"
            };

            var standardOutput = branchTask.StandardOutput.Split(Environment.NewLine.ToCharArray());
            return standardOutput.Where(branch => !string.IsNullOrEmpty(branch) && !branchFilter.Contains(branch));
        }

        public static IEnumerable<string> GetRemoteBranches(string workingDirectory, bool fetchAll = true) =>
            GetRemoteBranchesAsync(workingDirectory, fetchAll).GetAwaiter().GetResult();

        public static async Task TrackBranchAsync(string workingDirectory, string branch)
        {
            var trackTask = await Cli.Wrap("git")
                .WithWorkingDirectory(workingDirectory)
                .WithArguments(new string[] { "branch", "--track", branch, $"origin/{branch}" })
                .ExecuteAsync();
        }

        public static void TrackBranch(string workingDirectory, string branch) => TrackBranchAsync(workingDirectory, branch).GetAwaiter().GetResult();

        public static void TrackAllBranches(string workingDirectory, Action<string> verboseAction = null, Action<string> warningAction = null)
        {
            var remoteBranches = GetRemoteBranches(workingDirectory, fetchAll: true);
            var localBranches = GetLocalBranches(workingDirectory);

            foreach (string remoteBranch in remoteBranches)
            {
                if (!localBranches.Contains(remoteBranch))
                {
                    if (verboseAction != null)
                    {
                        verboseAction($"Branch '{remoteBranch}' set up to track 'origin/{remoteBranch}'");
                    }

                    TrackBranch(workingDirectory, remoteBranch);
                }
                else
                {
                    if (warningAction != null)
                    {
                        warningAction($"A branch named '{remoteBranch}' already exists");
                    }
                }
            }
        }

        #endregion

        #region commits

        public static async Task<Commit> GetLastCommitAsync(string workingDirectory)
        {
            var commitTask = await Cli.Wrap("git")
                .WithWorkingDirectory(workingDirectory)
                .WithArguments(new string[] { "log", "-n", "1", "--format='%s%n%H%n%an%n%ae%n%at'" })
                .ExecuteBufferedAsync();

            var standardOutput = commitTask.StandardOutput.Replace("'", "").Split(Environment.NewLine.ToArray()).ToList();

            return new Commit
            {
                Message = standardOutput[0],
                Hash = standardOutput[1],
                Author = new Author { Name = standardOutput[2], Email = standardOutput[3] },
                DateTime = Utils.ConvertToDateTime(unixTimeStamp: Convert.ToDouble(standardOutput[4]))
            };
        }

        public static Commit GetLastCommit(string workingDirectory) => GetLastCommitAsync(workingDirectory).GetAwaiter().GetResult();

        public static async Task<int> GetCommitCountAsync(string workingDirectory, string branch)
        {
            var countTask = await Cli.Wrap("git")
                .WithWorkingDirectory(workingDirectory)
                .WithArguments(new string[] { "rev-list", "--count", branch })
                .ExecuteBufferedAsync();

            return Convert.ToInt32(countTask.StandardOutput);
        }

        public static int GetCommitCount(string workingDirectory, string branch) => GetCommitCountAsync(workingDirectory, branch).GetAwaiter().GetResult();

        #endregion

        #region logs

        public static async Task<IEnumerable<Author>> GetAuthorsAsync(string workingDirectory)
        {
            var authorTask = await Cli.Wrap("git")
                .WithWorkingDirectory(workingDirectory)
                .WithArguments(new string[] { "log", "--format='%an,%ae'" })
                .ExecuteBufferedAsync();

            var standardOutput = authorTask.StandardOutput.Split(Environment.NewLine.ToArray()).Distinct();

            return standardOutput
                .Where(line => !string.IsNullOrEmpty(line))
                .Select(line =>
                {
                    string[] log = line.Replace("'", "").Split(',');
                    return new Author { Name = log[0], Email = log[1] };
                });
        }

        public static IEnumerable<Author> GetAuthors(string workingDirectory) => GetAuthorsAsync(workingDirectory).GetAwaiter().GetResult();

        #endregion
    }
}
