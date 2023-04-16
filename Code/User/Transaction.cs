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

        public enum Component
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
            NEWWALLET = Component.USERNAME | Component.WALLETNAME | (Component.VALID << 3),
            RMWALLET = Component.USERNAME | Component.WALLETNAME | (Component.VALID << 4)
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
        public DateTime Time { get; set; }
        public string Message { get; set; }

        public Transaction(Type type)
        {
            this.type = type;
            username = "None";
            walletName = "None";
            transactionValue = 0;
            Time = DateTime.Now;
            Message = "None";
        }

        public static class Parser
        {
            public static char Separator = '~';
            public static bool WriteToStream(StreamWriter writer, Transaction tran)
            {
                try
                {
                    writer.WriteLine($"{tran.type}{Separator}{tran.username}{Separator}{tran.walletName}"
                        + $"{Separator}{tran.transactionValue}{Separator}{tran.Time}{Separator}{tran.Message}");
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }

            public static bool TryParseFromStream(StreamReader reader, ref Transaction trans)
            {
                string[] words = IOStream.GetInputAsArray(reader, Separator);
                if (words.Length != 6)
                    return false;
                if (!(Account.IsUsername(words[1]) && Wallet.IsWalletName(words[2])))
                    return false;

                string username = words[1], walletName = words[2];
                if (Enum.TryParse(words[0], out Type type) && uint.TryParse(words[3], out uint transactionValue)
                    && DateTime.TryParse(words[4], out DateTime time))
                {
                    trans = new(type)
                    {
                        username = username,
                        walletName = walletName,
                        transactionValue = transactionValue,
                        Time = time,
                        Message = string.IsNullOrWhiteSpace(words[5]) ? "None" : words[5]
                    };
                    return true;
                }
                else
                    return false;
            }
        }
    }
}