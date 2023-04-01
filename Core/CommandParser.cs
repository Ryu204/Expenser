using System.Diagnostics;
using Expenser.Utility;

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

        public static bool IsAction(string input)
        {
            return IOStream.IsAllLetter(input) && input.Length > 0;
        }

        public static bool IsFlag(string input)
        {
            if (input.Length <= FlagSignature.Length || !input.StartsWith(FlagSignature))
                return false;
            if (!IOStream.IsAllLetter(input[FlagSignature.Length..]))
                return false;
            return true;
        }

        public static string FormatFlag(string flag)
        {
            Debug.Assert(IsFlag(flag));
            return flag[FlagSignature.Length..];
        }

        public static string FormatAction(string action)
        {
            return action.ToLower();
        }
    }
}
