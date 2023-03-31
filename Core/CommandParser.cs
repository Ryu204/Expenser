/*
    A string Str is:
    1. An action <-> Str consists of >= 1 letter
                     All characters of Str is letter
    2. A flag <-> Str = "--" + An Action
*/
 using System.Diagnostics;

namespace Expenser.Core
{
    static public class CommandParser
    {
        public static readonly char FlagChar = '-';

        private static bool IsAllLetter(string input)
        {
            foreach (char c in input) 
                if (!char.IsLetter(c))
                    return false;
            return true;
        }

        public static bool IsAction(string input)
        {
            return IsAllLetter(input) && input.Length > 0;
        }

        public static bool IsFlag(string input)
        {
            if (input.Length <= 1 || !input.StartsWith(FlagChar))
                return false;
            if (!IsAllLetter(input[1..]))
                return false;
            return true;
        }

        public static string TrimFlag(string flag)
        {
            Debug.Assert(IsFlag(flag));
            return flag[1..];
        }
    }
}
