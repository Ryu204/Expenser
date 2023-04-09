using System;
using Expenser.Core;

namespace Expenser.Utility
{
    /// <summary>
    /// Parse input and format output
    /// </summary>
    public static class IOStream
    {            
        private enum ColorCode
        {
            NORMAL, ERROR, OTHER, DEFAULT
        }
        private static readonly ConsoleColor[] Colors = {
            ConsoleColor.Green,
            ConsoleColor.Red,
            ConsoleColor.Yellow,
            ConsoleColor.White
        };

        public static void Output(string message, bool newline = true)
        {
            Console.ForegroundColor = Colors[(int)ColorCode.NORMAL];
            Console.Write(message);
            if (newline)
                Console.WriteLine();
            Console.ForegroundColor = Colors[(int)ColorCode.DEFAULT];
        }

        public static void OutputError(string message)
        {
            Console.ForegroundColor = Colors[(int)ColorCode.ERROR];
            Console.WriteLine(message);
            Console.ForegroundColor = Colors[(int)ColorCode.DEFAULT];
        }

        public static void OutputOther(string message)
        {
            Console.ForegroundColor = Colors[(int)ColorCode.OTHER];
            Console.WriteLine(message);
            Console.ForegroundColor = Colors[(int)ColorCode.DEFAULT];
        }

        private static void PromptInput()
        {
            Console.Write(">>> ");
        }

        public static string[] GetInputAsArray()
        {
            PromptInput();
            string? res = Console.ReadLine();
            if (res == null)
                return Array.Empty<string>();

            HashSet<char> whitespaces = new();
            foreach (char c in res)
                if (char.IsWhiteSpace(c))
                    whitespaces.Add(c);

            return res.Split(whitespaces.ToArray(), StringSplitOptions.RemoveEmptyEntries);
        }

        public static bool TryGetFileName(string filename, ref string name)
        {
            string suffix = ".data";
            if (string.IsNullOrWhiteSpace(filename) || !filename.EndsWith(suffix))
                return false;
            if (filename.Length <= suffix.Length)
                return false;
            if (!GrammarChecker.IsAllLetter(filename[..(filename.Length - suffix.Length)]))
                return false;
            name = filename[..(filename.Length - suffix.Length)];
            return true;
        }
    }
}
