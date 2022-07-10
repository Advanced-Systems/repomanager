using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using CliWrap;
using CliWrap.Buffered;

namespace RepoManager
{
    internal class Git
    {
        public static async Task<string> GetConfigAsync(string key, Scope scope)
        {
            var configTask = await Cli.Wrap("git")
                .WithArguments(new string[] { "config", $"--{scope.ToString().ToLower()}", key })
                .ExecuteBufferedAsync();

            return configTask.StandardOutput.RemoveLineBreaks();
        }

        public static string GetConfig(string key, Scope scope) => GetConfigAsync(key, scope).GetAwaiter().GetResult();

        public static async Task<string> GetDefaultBranchAsync(string workingDirectory)
        {
            var branchTask = await Cli.Wrap("git")
                .WithWorkingDirectory(workingDirectory)
                .WithArguments(new string[] { "remote", "show", "origin" })
                .ExecuteBufferedAsync();

            var standardOutput = branchTask.StandardOutput.Split(Environment.NewLine.ToArray()).ToList();

            return standardOutput
                .Where(line => line.Contains("HEAD branch"))
                .First()
                .Split(":")
                .Last()
                .Trim();
        }

        public static string GetDefaultBranch(string workingDirectory) => GetDefaultBranchAsync(workingDirectory).GetAwaiter().GetResult();

        public static async Task<string> GetActiveBranchAsync(string workingDirectory)
        {
            var branchTask = await Cli.Wrap("git")
                .WithWorkingDirectory(workingDirectory)
                .WithArguments(new string[] { "rev-parse", "--abbrev-ref", "HEAD" })
                .ExecuteBufferedAsync();

            return branchTask.StandardOutput;
        }

        public static string GetActiveBranch(string workingDirectory) => GetActiveBranchAsync(workingDirectory).GetAwaiter().GetResult();

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

        public static async Task FetchAsync(string workingDirectory, bool all = false)
        {
            var fetchTask = await Cli.Wrap("git")
                .WithWorkingDirectory(workingDirectory)
                .WithArguments(new string[] { "fetch", all ? "--all" : string.Empty })
                .ExecuteAsync();
        }

        public static void Fetch(string workingDirectory, bool all = false)
        {
            FetchAsync(workingDirectory, all).GetAwaiter().GetResult();
        }

        public static async Task<IEnumerable<string>> GetLocalBranchesAsync(string workingDirectory)
        {
            var branchTask = await Cli.Wrap("git")
                .WithWorkingDirectory(workingDirectory)
                .WithArguments(new string[] { "branch", "--format=%(refname:short)" })
                .ExecuteBufferedAsync();

            var standardOutput = branchTask.StandardOutput.Split(Environment.NewLine.ToArray()).ToList();
            return standardOutput.Where(b => !string.IsNullOrEmpty(b));
        }

        public static IEnumerable<string> GetLocalBranches(string workingDirectory)
        {
            return GetLocalBranchesAsync(workingDirectory).GetAwaiter().GetResult();
        }

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

            var standardOutput = branchTask.StandardOutput.Split(Environment.NewLine.ToCharArray()).ToList();
            return standardOutput.Where(branch => !string.IsNullOrEmpty(branch) && !branchFilter.Contains(branch));
        }

        public static IEnumerable<string> GetRemoteBranches(string workingDirectory, bool fetchAll = true)
        {
            return GetRemoteBranchesAsync(workingDirectory, fetchAll).GetAwaiter().GetResult();
        }

        public static async Task TrackBranchAsync(string workingDirectory, string branch)
        {
            var trackTask = await Cli.Wrap("git")
                .WithWorkingDirectory(workingDirectory)
                .WithArguments(new string[] { "branch", "--track", branch, $"origin/{branch}" })
                .ExecuteAsync();
        }

        public static void TrackBranch(string workingDirectory, string branch)
        {
            TrackBranchAsync(workingDirectory, branch).GetAwaiter().GetResult();
        }

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

        public static async Task CloneRepositoryAsync(string uri, string path)
        {
            var cloneTask = await Cli.Wrap("git")
                .WithArguments(new string[] { "clone", uri, path, "--quiet" })
                .ExecuteAsync();
        }

        public static void CloneRepository(string uri, string path)
        {
            CloneRepositoryAsync(uri, path).GetAwaiter().GetResult();
        }
    }
}
