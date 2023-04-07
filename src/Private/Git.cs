using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

using CliWrap;
using CliWrap.Buffered;

namespace RepoManager
{
    internal sealed class Git
    {
        #region configuration

        public static string GetConfig(string key, Scope scope)
        {
            var configCommand = Cli.Wrap("git")
                .WithArguments(args => args
                    .Add("config")
                    .Add($"--{scope.ToString().ToLower()}")
                    .Add(key)
                )
                .ExecuteBufferedAsync()
                .GetAwaiter()
                .GetResult();

            return configCommand.StandardOutput.ReplaceLineEndings(string.Empty);
        }

        public static Remote GetRemote(string workingDirectory)
        {
            var remoteCommand = Cli.Wrap("git")
                .WithWorkingDirectory(workingDirectory)
                .WithArguments(args => args
                    .Add("remote")
                    .Add("--verbose")
                )
                .ExecuteBufferedAsync()
                .GetAwaiter()
                .GetResult();

            var remotes = remoteCommand.StandardOutput
                .Split(new char[] { '\t', ' ' })
                .Where(line => !line.Contains("origin") && !line.Contains('('));

            return new Remote
            {
                Fetch = remotes.First().ToString(),
                Push = remotes.Last().ToString()
            };
        }

        #endregion

        #region repository

        public static void Fetch(string workingDirectory, bool all = false)
        {
            var fetchCommand = Cli.Wrap("git")
                .WithWorkingDirectory(workingDirectory)
                .WithArguments(args => args
                    .Add("fetch")
                    .AddOption(all, "--all")
                )
                .ExecuteAsync()
                .GetAwaiter()
                .GetResult();
        }

        public static void CloneRepository(string uri, string path, bool silent = false, Action<string> warningAction = null)
        {
            if (!Directory.Exists(path) || !Directory.EnumerateFileSystemEntries(path).Any())
            {
                var cloneCommand = Cli.Wrap("git")
                    .WithArguments(args => args
                        .Add("clone")
                        .Add(uri)
                        .Add(path)
                        .AddOption(silent, "--quiet")
                    )
                    .ExecuteBufferedAsync()
                    .GetAwaiter()
                    .GetResult();
            }
            else
            {
                if (warningAction != null && !silent) warningAction($"Destination path '{path}' already exists and is not an empty directory");
            }
        }

        public static IEnumerable<string> GetTrackedFiles(string workingDirectory, string branch)
        {
            var listCommand = Cli.Wrap("git")
                .WithWorkingDirectory(workingDirectory)
                .WithArguments(args => args
                    .Add("ls-tree")
                    .Add("-r")
                    .Add(branch)
                    .Add("--name-only")
                )
                .ExecuteBufferedAsync()
                .GetAwaiter()
                .GetResult();

            return listCommand.StandardOutput.Split(Environment.NewLine.ToArray()).SkipLast(1);
        }

        #endregion

        #region branches

        public static IEnumerable<string> GetLocalBranches(string workingDirectory)
        {
            var branchCommand = Cli.Wrap("git")
                .WithWorkingDirectory(workingDirectory)
                .WithArguments(args => args
                    .Add("branch")
                    .Add("--format=%(refname:short)")
                )
                .ExecuteBufferedAsync()
                .GetAwaiter()
                .GetResult();

            return branchCommand.StandardOutput.Split(Environment.NewLine.ToArray(), StringSplitOptions.RemoveEmptyEntries);
        }

        public static string GetDefaultBranch(string workingDirectory)
        {
            var branchCommand = Cli.Wrap("git")
                .WithWorkingDirectory(workingDirectory)
                .WithArguments(args => args
                    .Add("config")
                    .Add("--get")
                    .Add("init.defaultBranch")
                )
                .ExecuteBufferedAsync()
                .GetAwaiter()
                .GetResult();

            return branchCommand.StandardOutput.ReplaceLineEndings(string.Empty);
        }

        public static string GetCurrentBranch(string workingDirectory)
        {
            var branchCommand = Cli.Wrap("git")
                .WithWorkingDirectory(workingDirectory)
                .WithArguments(args => args
                    .Add("branch")
                    .Add("--show-current")
                )
                .ExecuteBufferedAsync()
                .GetAwaiter()
                .GetResult();

            return branchCommand.StandardOutput.ReplaceLineEndings(string.Empty);
        }

        public static IEnumerable<string> GetRemoteBranches(string workingDirectory, bool fetchAll = true)
        {
            if (fetchAll) Fetch(workingDirectory, all: true);

            var branchCommand = Cli.Wrap("git")
                .WithWorkingDirectory(workingDirectory)
                .WithArguments(args => args
                    .Add("branch")
                    .Add("--remote")
                    .Add("--format=%(refname:lstrip=3)")
                )
                .ExecuteBufferedAsync()
                .GetAwaiter()
                .GetResult();

            var branchFilter = new List<string> {
                "HEAD",
                "master",
                "main"
            };

            var result = branchCommand.StandardOutput.Split(Environment.NewLine.ToCharArray());
            return result.Where(branch => !string.IsNullOrEmpty(branch) && !branchFilter.Contains(branch));
        }

        public static void TrackBranch(string workingDirectory, string branch, bool silent = false)
        {
            var branchCommand = Cli.Wrap("git")
                .WithWorkingDirectory(workingDirectory)
                .WithArguments(args => args
                    .Add("branch")
                    .Add("--track")
                    .Add(branch)
                    .Add($"origin/{branch}")
                    .AddOption(silent, "--quiet")
                )
                .ExecuteAsync()
                .GetAwaiter()
                .GetResult();
        }

        public static void TrackAllBranches(string workingDirectory, bool silent = false)
        {
            var remoteBranches = GetRemoteBranches(workingDirectory, fetchAll: true);
            var localBranches = GetLocalBranches(workingDirectory);

            foreach (string remoteBranch in remoteBranches)
            {
                if (localBranches.Contains(remoteBranch)) continue;

                TrackBranch(workingDirectory, remoteBranch, silent);
            }
        }

        #endregion

        #region commits

        public static Commit GetLastCommit(string workingDirectory)
        {
            var logCommand = Cli.Wrap("git")
                .WithWorkingDirectory(workingDirectory)
                .WithArguments(args => args
                    .Add("log")
                    .Add("-n")
                    .Add(1)
                    .Add("--format=%s%n%H%n%an%n%ae%n%at")
                )
                .ExecuteBufferedAsync()
                .GetAwaiter()
                .GetResult();

            var result = logCommand.StandardOutput.Split(Environment.NewLine.ToArray()).ToList();

            return new Commit
            {
                Message = result[0],
                Hash = result[1],
                Author = new Author { Name = result[2], Email = result[3] },
                DateTime = DateTimeOffset.FromUnixTimeSeconds(long.Parse(result[4])).DateTime
            };
        }

        public static int GetCommitCount(string workingDirectory, string branch)
        {
            var revCommand = Cli.Wrap("git")
                .WithWorkingDirectory(workingDirectory)
                .WithArguments(args => args
                    .Add("rev-list")
                    .Add("--count")
                    .Add(branch)
                )
                .ExecuteBufferedAsync()
                .GetAwaiter()
                .GetResult();

            return Convert.ToInt32(revCommand.StandardOutput);
        }

        #endregion

        #region logs

        public static IEnumerable<Author> GetAuthors(string workingDirectory)
        {
            var logCommand = Cli.Wrap("git")
                .WithWorkingDirectory(workingDirectory)
                .WithArguments(args => args
                    .Add("log")
                    .Add("--format=%an,%ae")
                )
                .ExecuteBufferedAsync()
                .GetAwaiter()
                .GetResult();

            return logCommand
                .StandardOutput
                .Split(Environment.NewLine.ToArray())
                .Distinct()
                .Where(line => !string.IsNullOrEmpty(line))
                .Select(line => {
                    var log = line.Split(',');
                    return new Author { Name = log[0], Email = log[1] };
                });
        }

        #endregion
    }
}
