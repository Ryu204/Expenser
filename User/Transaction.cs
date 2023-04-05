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

        public Transaction(Type type)
        {
            this.type = type;
            username = "_";
            walletName = "_";
            transactionValue = 0;
        }
        public bool WriteToStream(StreamWriter writer)
        {
            try
            {
                writer.WriteLine($"{type},{username},{walletName},{transactionValue}");
            }
            catch(Exception)
            {
                return false;
            }
            return true;
        }

        public static bool ReadFromStream(StreamReader reader, ref Transaction trans)
        {
            string? line = reader.ReadLine();
            if (line == null)
                return false;
            string[] words = line.Split(',', StringSplitOptions.RemoveEmptyEntries);

            if (words.Length != 4)
                return false;

            string username = words[1], walletName = words[2];

            if (Enum.TryParse(words[0], out Type type) && uint.TryParse(words[3], out uint transactionValue))
            {
                Transaction res = new(type)
                {
                    Username = username,
                    WalletName = walletName,
                    transactionValue = transactionValue,
                };
                trans = res;
                return true;
            }
            else
                return false;
        }
            
        public enum Type
        {
            _NONE = 0,
            _USERNAME = 1 << 0,
            _WALLETNAME = 1 << 1,
            _VALUE = 1 << 2,
            
            _VALID = 1 << 10,

            ADD = _USERNAME | _WALLETNAME | _VALUE | (_VALID << 1),
            SUB = _USERNAME | _WALLETNAME | _VALUE | (_VALID << 2),
            NEWWALLET = _USERNAME | _WALLETNAME | _VALUE | (_VALID << 3)
        }

        public Type Operation
        {
            get
            {
                Debug.Assert(type > Type._VALID);
                return type;
            }
        }
        public string Username
        { 
            get
            {
                Debug.Assert((type & Type._USERNAME) != 0);
                return username;
            }
            set
            {
                Debug.Assert((type & Type._USERNAME) != 0);
                username = value;
            }
        }
        public string WalletName
        {
            get
            {
                Debug.Assert((type & Type._WALLETNAME) != 0);
                return walletName;
            }
            set
            {
                Debug.Assert((type & Type._WALLETNAME) != 0);
                walletName = value;
            }
        }
        public uint Value
        {
            get
            {
                Debug.Assert((type & Type._VALUE) != 0);
                return transactionValue;
            }
            set
            {
                Debug.Assert((type & Type._VALUE) != 0);
                transactionValue = value;
            }
        }
    }
}