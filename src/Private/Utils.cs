using System.Diagnostics;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RepoManager
{
    internal class Utils
    {
        public static List<string> RunCommand(string workingDirectory, string program, params string[] arguments)
        {
            var tsc = new TaskCompletionSource<int>();
            List<string> standardOutput = new List<string>();

            var startInfo = new ProcessStartInfo
            {
                WorkingDirectory = workingDirectory,
                FileName = program,
                Arguments = string.Join(" ", arguments),
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            };

            using (var process = new Process { StartInfo = startInfo, EnableRaisingEvents = true })
            {
                process.Start();

                process.Exited += (sender, args) =>
                {
                    tsc.SetResult(process.ExitCode);
                    process.Dispose();
                };

                process.OutputDataReceived += (sender, line) =>
                {
                    if (line.Data != null)
                    {
                        standardOutput.Add(line.Data);
                    }
                };

                process.BeginOutputReadLine();
                process.WaitForExit();
            }

            return standardOutput;
        }
    }
}
