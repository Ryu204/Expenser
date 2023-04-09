using System;
using static Expenser.Utility.IOStream;

namespace Expenser.Core
{
    public class Command
    {
        public string Action { get; } = string.Empty;
        public HashSet<string> Flags { get; } = new();
        public string[] Value { get; } = Array.Empty<string>();

        public Command() { }
        public Command(string action, HashSet<string> flags, string[] value)
        {
            Action = action;
            Flags = flags;
            Value = value;
        }
    }
}
