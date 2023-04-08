using System;
using System.Collections;
using System.Linq;
using System.Runtime.CompilerServices;

using CliWrap.Builders;

namespace RepoManager
{
    public static class ExtensionMethods
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string ReplaceLineEndings(this string @string, string newValue) => @string.Replace("\r\n", string.Empty).Replace("\r", string.Empty).Replace("\n", string.Empty);

        public static ArgumentsBuilder AddOption(this ArgumentsBuilder args, bool condition, string option)
        {
            if (condition) args.Add(option);
            return args;
        }
    }
}
