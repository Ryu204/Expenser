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

        public static void Output(string message, bool newline = true, ConsoleColor color = ConsoleColor.Green)
        {
            Console.ForegroundColor = color;
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

        public static void OutputOther(string message, bool newline = true)
        {
            Console.ForegroundColor = Colors[(int)ColorCode.OTHER];
            Console.Write(message);
            if (newline)
                Console.WriteLine();
            Console.ForegroundColor = Colors[(int)ColorCode.DEFAULT];
        }

        private static void PromptInput()
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("************************************************************************************\n>>> ");
            Console.ForegroundColor = Colors[(int)ColorCode.DEFAULT];
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

        public static string[] GetInputAsArray(StreamReader reader, char separator = ' ')
        {
            string? res = reader.ReadLine();
            if (res == null)
                return Array.Empty<string>();

            HashSet<char> whitespaces = new();
            foreach (char c in res)
                if (char.IsWhiteSpace(c))
                    whitespaces.Add(c);
            if  (separator == ' ')
                return res.Split(whitespaces.ToArray(), StringSplitOptions.RemoveEmptyEntries);
            else
                return res.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        }

        public static bool YesNoPrompt(string message)
        {
            OutputOther($"{message} [y/n] ", false);
            string? inp = Console.ReadLine();
            if (inp == null)
                return false;
            inp = inp.Trim();
            if (inp.ToLower() != "y")
                return false;
            return true;
        }
    }
}
