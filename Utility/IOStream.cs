using System;
using Expenser.Core;

namespace Expenser.Utility
{
    /// <summary>
    /// Parse input and format output
    /// </summary>
    public static class IOStream
    {
        // Indicate the result of latest call to ParseCommand()
        public enum InputState
        { 
            OKAY, EMPTY, INVALID_ACTION, INVALID_FLAG
        }
        private static readonly string[] errorMessages = {
            "okay (should not be printed)",
            "Empty line",
            "Invalid action format (Must only consist of alphabetical characters)",
            "Invalid flag format (Example for flag: \"--short\",\"--force\")"
        };
            
        public static InputState State { get; private set; }
        public static string Message { get { return $"Input error: {errorMessages[(int)State]}."; } }

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
            if (newline)
                Console.WriteLine(message);
            else
                Console.Write(message);
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

        public static void PromptInput()
        {
            Console.Write(">>> ");
        }

        public static Command ParseCommand()
        {
            PromptInput();

            string[] input = GetInputString();

            if (input.Length == 0)
            {
                State = InputState.EMPTY;
                return new Command();
            }

            string action = input[0];
            if (!CommandParser.IsAction(action))
            {
                State = InputState.INVALID_ACTION;
                return new Command();
            }
            action = CommandParser.FormatAction(action);

            HashSet<string> flags = new();
            List<string> value = new();
            for(int i = 1; i < input.Length; ++i)
            {
                if (input[i].StartsWith(CommandParser.FlagSignature))
                {
                    if (!CommandParser.IsFlag(input[i]))
                    {
                        State = InputState.INVALID_FLAG;
                        return new Command();
                    }
                    flags.Add(CommandParser.FormatFlag(input[i]));
                }
                else
                    value.Add(input[i]);
            }

            State = InputState.OKAY;
            return new Command(action, flags, value.ToArray());
        }

        private static string[] GetInputString()
        {
            string? res = Console.ReadLine();
            if (res == null)
                return Array.Empty<string>();

            HashSet<char> whitespaces = new();
            foreach (char c in res)
                if (char.IsWhiteSpace(c))
                    whitespaces.Add(c);

            return res.Split(whitespaces.ToArray(), StringSplitOptions.RemoveEmptyEntries);
        }

        public static bool IsAllLetter(string input)
        {
            foreach (char c in input)
                if (!char.IsLetter(c))
                    return false;
            return true;
        }

        public static bool TryGetFileName(string filename, ref string name)
        {
            string suffix = ".data";
            if (string.IsNullOrWhiteSpace(filename) || !filename.EndsWith(suffix))
                return false;
            if (filename.Length <= suffix.Length)
                return false;
            if (!IsAllLetter(filename[..(filename.Length - suffix.Length)]))
                return false;
            name = filename[..(filename.Length - suffix.Length)];
            return true;
        }
    }
}
