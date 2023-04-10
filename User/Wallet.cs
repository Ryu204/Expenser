
using Expenser.Utility;

namespace Expenser.User
{
    /// <summary>
    /// Used to categorised user's data 
    /// into main parts
    /// </summary>
    public class Wallet
    {
        public uint Value { get; private set; }

        public Wallet(uint value)
        {
            Value = value;
        }   

        public static bool IsWalletName(string name)
        {
            return GrammarChecker.IsAllLetter(name);
        }
    }
}
