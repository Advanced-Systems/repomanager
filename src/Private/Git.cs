using System;
using System.Collections.Generic;

namespace RepoManager
{
    internal class Git
    {
        public static List<string> GetRemoteBranches(string workingDirectory)
        {
            List<string> standardOutput = Utils.RunCommand(workingDirectory, "git", "branch", "--remote");

            // TODO: filter output

            return standardOutput;
        }
    }
}
