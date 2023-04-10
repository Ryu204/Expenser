using Expenser.Utility;
using System.Diagnostics;

namespace Expenser.User
{
    public struct Transaction
    {
        private readonly Type type;
        private string username;
        private string walletName;
        private uint transactionValue;

        private enum Component
        {
            NONE = 0,
            USERNAME = 1 << 0,
            WALLETNAME = 1 << 1,
            VALUE = 1 << 2,
            VALID = 1 << 10,
        }
        public enum Type
        {
            ADD = Component.USERNAME | Component.WALLETNAME | Component.VALUE | (Component.VALID << 1),
            SUB = Component.USERNAME | Component.WALLETNAME | Component.VALUE | (Component.VALID << 2),
            NEWWALLET = Component.USERNAME | Component.WALLETNAME | Component.VALUE | (Component.VALID << 3)
        }

        public Type Operation { get { return type; } }
        public string Username
        { 
            get
            {
                Debug.Assert(((uint)type & (uint)Component.USERNAME) != 0);
                return username;
            }
            set
            {
                Debug.Assert(((uint)type & (uint)Component.USERNAME) != 0);
                Debug.Assert(Account.IsUsername(value));
                username = value;
            }
        }
        public string WalletName
        {
            get
            {
                Debug.Assert(((uint)type & (uint)Component.WALLETNAME) != 0);
                return walletName;
            }
            set
            {
                Debug.Assert(((uint)type & (uint)Component.WALLETNAME) != 0);
                Debug.Assert(Wallet.IsWalletName(value));
                walletName = value;
            }
        }
        public uint Value
        {
            get
            {
                Debug.Assert(((uint)type & (uint)Component.VALUE) != 0);
                return transactionValue;
            }
            set
            {
                Debug.Assert(((uint)type & (uint)Component.VALUE) != 0);
                transactionValue = value;
            }
        }

        public Transaction(Type type)
        {
            this.type = type;
            username = "None";
            walletName = "None";
            transactionValue = 0;
        }

        public static class Parser
        {
            public static bool WriteToStream(StreamWriter writer, Transaction tran)
            {
                try
                {
                    writer.WriteLine($"{tran.type} {tran.username} {tran.walletName} {tran.transactionValue}");
                }
                catch (Exception)
                {
                    return false;
                }
                return true;
            }

            public static bool TryParseFromStream(StreamReader reader, ref Transaction trans)
            {
                string[] words = IOStream.GetInputAsArray(reader);
                if (words.Length != 4)
                    return false;
                if (!(Account.IsUsername(words[1]) && Wallet.IsWalletName(words[2])))
                    return false;

                string username = words[1], walletName = words[2];
                if (Enum.TryParse(words[0], out Type type) && uint.TryParse(words[3], out uint transactionValue))
                {
                    trans = new(type)
                    {
                        Username = username,
                        WalletName = walletName,
                        Value = transactionValue,
                    };
                    return true;
                }
                else
                    return false;
            }
        }
    }
}