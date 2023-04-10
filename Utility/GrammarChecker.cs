
namespace Expenser.Utility
{
    static public class GrammarChecker
    {
        public readonly static string UserFileSuffix = ".data";

        public static bool IsAllLetter(string input)
        {
            foreach (char c in input)
                if (!char.IsLetter(c))
                    return false;
            return true;
        }

        public static bool TryGetFileName(string filename, out string name)
        {
            name = string.Empty;
            if (string.IsNullOrWhiteSpace(filename) || !filename.EndsWith(UserFileSuffix))
                return false;
            if (filename.Length == UserFileSuffix.Length)
                return false;
            if (!IsAllLetter(filename[..(filename.Length - UserFileSuffix.Length)]))
                return false;
            name = filename[..(filename.Length - UserFileSuffix.Length)];
            return true;
        }
    }
}
