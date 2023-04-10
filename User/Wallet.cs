
using Expenser.Utility;
using System.Diagnostics;
using System.Transactions;

namespace Expenser.User
{
    /// <summary>
    /// Used to categorised user's data 
    /// into main parts
    /// </summary>
    public class Wallet
    {
        public string Name { get; }
        public uint Value { get; private set; }
        public static readonly string DefaultName = "Other";

        public Wallet(string name, uint value)
        {
            Name = name;
            Value = value;
        }   

        public static bool IsWalletName(string name)
        {
            return GrammarChecker.IsAllLetter(name) && name.Length >= 2;
        }

        public bool AdjustValue(uint increment, bool add)
        {
            uint newValue;
            try
            {
                checked
                {
                    newValue = add ? Value + increment : Value - increment;
                }
            }
            catch (OverflowException)
            {
                if (add)
                {
                    if (Name != DefaultName)
                        IOStream.OutputError($"Addition value {increment} is too large to add to wallet {Name}.");
                    else
                        IOStream.OutputError($"Addition value {increment} is too large to add to spare amount.");
                }
                else
                {
                    if (Name != DefaultName)
                        IOStream.OutputError($"This wallet only have {Value} VND.");
                    else
                        IOStream.OutputError($"You only have {Value} VND spare money.");
                }
                return false;
            }

            Value = newValue;
            return true;
        }

        public static class Loader
        {
            public static bool WriteToStream(StreamWriter writer, Wallet wallet)
            {
                try
                {
                    writer.WriteLine($"{wallet.Name} {wallet.Value}");
                    return true;
                }
                catch(Exception)
                {
                    return false;
                }
            }
            public static bool TryParseFromStream(StreamReader reader, out Wallet wallet)
            {
                wallet = new(string.Empty, 0);
                string[] words = IOStream.GetInputAsArray(reader);
                if (words.Length != 2)
                    return false;
                if (!IsWalletName(words[0]))
                    return false;
                if (uint.TryParse(words[1], out uint value))
                {
                    wallet = new(words[0], value);
                    return true;
                }
                else
                    return false;
            }
        }
    }
}
