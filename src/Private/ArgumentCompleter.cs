using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using System.Management.Automation;
using System.Management.Automation.Language;

namespace RepoManager
{
    public class PathArgumentCompleter : IArgumentCompleter
    {
        IEnumerable<CompletionResult> IArgumentCompleter.CompleteArgument(string commandName, string parameterName, string wordToComplete, CommandAst commandAst, IDictionary fakeBoundParameters)
        {
            var configurationManager = new ConfigurationManager();
            var container = configurationManager.Configuration.Container;

            return container
                .Select(c => c.Path)
                .Where(new WildcardPattern($"{wordToComplete}*", WildcardOptions.IgnoreCase).IsMatch)
                .Select(match => new CompletionResult(match));
        }
    }

    public class NameArgumentCompleter : IArgumentCompleter
    {
        IEnumerable<CompletionResult> IArgumentCompleter.CompleteArgument(string commandName, string parameterName, string wordToComplete, CommandAst commandAst, IDictionary fakeBoundParameters)
        {
            var configurationManager = new ConfigurationManager();
            var container = configurationManager.Configuration.Container;
            string path = container.Where(repo => repo.IsDefault).Select(repo => repo.Path).First();

            var commandElements = commandAst.CommandElements.Select(ast => ast.Extent.Text);

            if (commandElements.Any(node => node.Contains("-Path")))
            {
                path = commandElements.SkipWhile(node => !node.Equals("-Path")).Skip(1).First();
            }

            var suggestedRepositories = Directory.GetDirectories(path).Select(System.IO.Path.GetFileName);

            return suggestedRepositories
                .Where(new WildcardPattern($"{wordToComplete}*", WildcardOptions.IgnoreCase).IsMatch)
                .Select(match => new CompletionResult(match));
        }
    }
}