using System;
using System.Diagnostics;
using Expenser.Utility;
using static Expenser.Utility.IOStream;

namespace Expenser.Core
{
    /// <summary>
    /// Parse input string by given rules:
    ///     * First word is action
    ///     * Flags start with "--"
    ///     * Others are values
    /// </summary>
    static public class CommandParser
    {
        public static readonly string FlagSignature = "--";
        private enum MessageID
        { GOOD, EMPTY, INVALID_ACTION, INVALID_FLAG }
        private static readonly string[] Messages =
        {
            string.Empty,
            "Empty line",
            "Invalid action format (Must only consist of alphabetical characters)",
            "Invalid flag format (Example for flag: \"--short\",\"--force\")"
        };

        public static bool TryParse(string[] splittedInput, ref Command command, out string message)
        {
            if (splittedInput.Length == 0)
            {
                message = Messages[(int)MessageID.EMPTY];
                return false;
            }

            if (!IsAction(splittedInput[0]))
            {
                message = Messages[(int)MessageID.EMPTY];
                return false;
            }
            
            string action = FormatAction(splittedInput[0]);
            HashSet<string> flags = new();
            List<string> value = new();

            for (int i = 1; i < splittedInput.Length; ++i)
            {
                if (splittedInput[i].StartsWith(FlagSignature))
                {
                    if (!IsFlag(splittedInput[i]))
                    {
                        message = Messages[(int)MessageID.INVALID_FLAG];
                        return false;
                    }
                    flags.Add(FormatFlag(splittedInput[i]));
                }
                else
                    value.Add(splittedInput[i]);
            }

            message = Messages[(int)MessageID.GOOD];
            command = new(action, flags, value.ToArray());
            return true;
        }

        // A string is an action if it has only and at least one letter
        public static bool IsAction(string input)
        {
            return GrammarChecker.IsAllLetter(input) && input.Length > 0;
        }

        // A string s is a flag if s = FlagSignature + an action
        public static bool IsFlag(string input)
        {
            if (!input.StartsWith(FlagSignature))
                return false;
            if (!IsAction(input[FlagSignature.Length..]))
                return false;
            return true;
        }

        public static string FormatAction(string action)
        {
            Debug.Assert(IsAction(action));
            return action.ToLower();
        }

        public static string FormatFlag(string flag)
        {
            Debug.Assert(IsFlag(flag));
            return flag[FlagSignature.Length..];
        }
    }
}
