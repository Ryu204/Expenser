
namespace Expenser.Utility
{
    static public class GrammarChecker
    {
        public static bool IsAllLetter(string input)
        {
            foreach (char c in input)
                if (!char.IsLetter(c))
                    return false;
            return true;
        }
    }
}
