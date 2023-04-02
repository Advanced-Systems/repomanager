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
            var configCommand = await Cli.Wrap("git")
                .WithArguments(new string[] { "config", $"--{scope.ToString().ToLower()}", key })
                .ExecuteBufferedAsync();

            return configCommand.StandardOutput.ReplaceLineEndings(string.Empty);
        }

        public static string GetConfig(string key, Scope scope) => GetConfigAsync(key, scope).GetAwaiter().GetResult();

        public static async Task<Remote> GetRemoteAsync(string workingDirectory)
        {
            var remoteCommand = await Cli.Wrap("git")
                .WithWorkingDirectory(workingDirectory)
                .WithArguments(new string[] { "remote", "--verbose" })
                .ExecuteBufferedAsync();

            var result = remoteCommand.StandardOutput
                .Split(new char[] { '\t', ' ' })
                .Where(line => !line.Contains("origin") && !line.Contains('('));

            return new Remote
            {
                Fetch = result.First().ToString(),
                Push = result.Last().ToString()
            };
        }

        public static Remote GetRemote(string workingDirectory) => GetRemoteAsync(workingDirectory).GetAwaiter().GetResult();

        #endregion

        #region repository

        public static async Task FetchAsync(string workingDirectory, bool all = false)
        {
            var fetchCommand = await Cli.Wrap("git")
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

                var cloneCommand = await Cli.Wrap("git")
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
            var listCommand = await Cli.Wrap("git")
                .WithWorkingDirectory(workingDirectory)
                .WithArguments(new string[] { "ls-tree", "-r", branch, "--name-only" })
                .ExecuteBufferedAsync();

            return listCommand.StandardOutput.Split(Environment.NewLine.ToArray()).SkipLast(1);
        }

        public static IEnumerable<string> GetTrackedFiles(string workingDirectory, string branch) => GetTrackedFilesAsync(workingDirectory, branch).GetAwaiter().GetResult();

        #endregion

        #region branches

        public static async Task<IEnumerable<string>> GetLocalBranchesAsync(string workingDirectory)
        {
            var branchCommand = await Cli.Wrap("git")
                .WithWorkingDirectory(workingDirectory)
                .WithArguments(new string[] { "branch", "--format=%(refname:short)" })
                .ExecuteBufferedAsync();

            return branchCommand.StandardOutput.Split(Environment.NewLine.ToArray(), StringSplitOptions.RemoveEmptyEntries);
        }

        public static IEnumerable<string> GetLocalBranches(string workingDirectory) => GetLocalBranchesAsync(workingDirectory).GetAwaiter().GetResult();

        public static async Task<string> GetDefaultBranchAsync(string workingDirectory)
        {
            var branchCommand = await Cli.Wrap("git")
                .WithWorkingDirectory(workingDirectory)
                .WithArguments(new string[] { "config", "--get", "init.defaultBranch" })
                .ExecuteBufferedAsync();

            return branchCommand.StandardOutput.ReplaceLineEndings(string.Empty);
        }

        public static string GetDefaultBranch(string workingDirectory) => GetDefaultBranchAsync(workingDirectory).GetAwaiter().GetResult();

        public static async Task<string> GetCurrentBranchAsync(string workingDirectory)
        {
            var branchCommand = await Cli.Wrap("git")
                .WithWorkingDirectory(workingDirectory)
                .WithArguments(new string[] { "branch", "--show-current" })
                .ExecuteBufferedAsync();

            return branchCommand.StandardOutput.ReplaceLineEndings(string.Empty);
        }

        public static string GetCurrentBranch(string workingDirectory) => GetCurrentBranchAsync(workingDirectory).GetAwaiter().GetResult();

        public static async Task<IEnumerable<string>> GetRemoteBranchesAsync(string workingDirectory, bool fetchAll = true)
        {
            if (fetchAll)
            {
                await FetchAsync(workingDirectory, all: true);
            }

            var branchCommand = await Cli.Wrap("git")
                .WithWorkingDirectory(workingDirectory)
                .WithArguments(new string[] { "branch", "--remote", "--format=%(refname:lstrip=3)" })
                .ExecuteBufferedAsync();

            var branchFilter = new List<string> {
                "HEAD",
                "master",
                "main"
            };

            var result = branchCommand.StandardOutput.Split(Environment.NewLine.ToCharArray());
            return result.Where(branch => !string.IsNullOrEmpty(branch) && !branchFilter.Contains(branch));
        }

        public static IEnumerable<string> GetRemoteBranches(string workingDirectory, bool fetchAll = true) =>
            GetRemoteBranchesAsync(workingDirectory, fetchAll).GetAwaiter().GetResult();

        public static async Task TrackBranchAsync(string workingDirectory, string branch)
        {
            var branchCommand = await Cli.Wrap("git")
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
            var logCommand = await Cli.Wrap("git")
                .WithWorkingDirectory(workingDirectory)
                .WithArguments(new string[] { "log", "-n", "1", "--format='%s%n%H%n%an%n%ae%n%at'" })
                .ExecuteBufferedAsync();

            var result = logCommand.StandardOutput.Replace("'", "").Split(Environment.NewLine.ToArray()).ToList();

            return new Commit
            {
                Message = result[0],
                Hash = result[1],
                Author = new Author { Name = result[2], Email = result[3] },
                DateTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(result[4])).DateTime
            };
        }

        public static Commit GetLastCommit(string workingDirectory) => GetLastCommitAsync(workingDirectory).GetAwaiter().GetResult();

        public static async Task<int> GetCommitCountAsync(string workingDirectory, string branch)
        {
            var revCommand = await Cli.Wrap("git")
                .WithWorkingDirectory(workingDirectory)
                .WithArguments(new string[] { "rev-list", "--count", branch })
                .ExecuteBufferedAsync();

            return Convert.ToInt32(revCommand.StandardOutput);
        }

        public static int GetCommitCount(string workingDirectory, string branch) => GetCommitCountAsync(workingDirectory, branch).GetAwaiter().GetResult();

        #endregion

        #region logs

        public static async Task<IEnumerable<Author>> GetAuthorsAsync(string workingDirectory)
        {
            var logCommand = await Cli.Wrap("git")
                .WithWorkingDirectory(workingDirectory)
                .WithArguments(new string[] { "log", "--format='%an,%ae'" })
                .ExecuteBufferedAsync();

            var result = logCommand.StandardOutput.Split(Environment.NewLine.ToArray()).Distinct();

            return result
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
