using Expenser.Core;

namespace Expenser.Utility
{
    public class IOStream
    {
        static readonly string EMPTY_LINE = "ios: empty line";
        static readonly string INVALID_ACTION = "ios:: invalid action";
        static readonly string INVALID_FLAG = "ios:: invalid flag";

        public enum InputState
        { 
            OKAY, EMPTY, INVALID_ACTION, INVALID_FLAG
        }

        private static List<string> messages = new() {
            "okay (should not be printed)",
            "empty line",
            "invalid action",
            "invalid flag"
        };
            
        public InputState State { get; private set; }
        public string Message { get { return "ios: " + messages[(int)State]; } }

        public void Output(string message)
        {
            Console.WriteLine(message);
        }

        // Get input string from console by
        // erasing uneeded whitespaces
        private static string[] GetInputString()
        {
            string? res = Console.ReadLine();
            if (res == null)
                return Array.Empty<string>();

            HashSet<char> whitespaces = new();
            foreach (char c in res)
                if (char.IsWhiteSpace(c))
                    whitespaces.Add(c);

            string[] trimmed = res.Split(whitespaces.ToArray(), StringSplitOptions.RemoveEmptyEntries);

            if (trimmed.Length == 0)
                return Array.Empty<string>();
            return trimmed;
        }

        public Command ParseCommand()
        {
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
            HashSet<string> flags = new();
            List<string> value = new();
            for(int i = 1; i < input.Length; ++i)
            {
                if (input[i].StartsWith(CommandParser.FlagChar))
                {
                    if (!CommandParser.IsFlag(input[i]))
                    {
                        State = InputState.INVALID_FLAG;
                        return new Command();
                    }
                    flags.Add(CommandParser.TrimFlag(input[i]));
                }
                else
                    value.Add(input[i]);
            }
            State = InputState.OKAY;
            return new Command(action, flags, value.ToArray());
        }
    }
}
