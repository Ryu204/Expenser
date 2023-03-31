using System;

namespace Expenser.Core
{
    public readonly struct Command
    {
        public string Action { get; }
        public HashSet<string> Flags { get; }
        public string[] Value { get; }

        public Command()
        {
            Action = string.Empty;
            Flags = new();
            Value = Array.Empty<string>();
        }

        public Command(string action, HashSet<string> flags, string[] value)
        {
            Action = action;
            Flags = flags;
            Value = value;
        }
    }
}
