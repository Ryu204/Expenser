using Expenser.Utility;

/// <summary>
/// Provide methods to interact with user.
/// For eg. saving data, updating status,
/// creating wallet,...
/// </summary>
namespace Expenser.User
{
    /// <summary>
    /// Create and edit user's login information
    /// and navigate to wallet
    /// </summary>
    public partial class Account
    {
        public readonly string Password = string.Empty;
        public readonly List<Transaction> Transactions = new();
        public readonly Dictionary<string, Wallet> Wallets = new();

        public string Username { get; private set; } = string.Empty;
        public ulong Value
        {
            get
            {
                ulong res = 0;
                foreach (var pair in Wallets)
                    res += pair.Value.Value;
                return res;
            }
        }

        public Account() { }
        public Account(string username, string password, List<Transaction> transactions, List<Wallet> wallets)
        {
            this.Password = password;
            this.Transactions = transactions;
            Username = username;

            bool defaulWalletExist = false;
            foreach (Wallet wallet in wallets)
            {
                Wallets.Add(wallet.Name, wallet);
                defaulWalletExist = defaulWalletExist || (wallet.Name == Wallet.DefaultName);
            }

            if (!defaulWalletExist)
                Wallets.Add(Wallet.DefaultName, new Wallet(Wallet.DefaultName, 0));
        }

        public static bool IsUsername(string username)
        {
            return username.Length >= 2 && GrammarChecker.IsAllLetter(username);
        }

        public static bool IsPassword(string password)
        {
            foreach (char c in password)
                if (!char.IsLetterOrDigit(c))
                    return false;
            if (password.Length <= 5)
                return false;
            else 
                return true;
        }

        public bool AdjustValue(uint increment, bool add, string walletName)
        {
            if (!Wallets.ContainsKey(walletName))
            {
                IOStream.OutputError($"User {Username} does not have a wallet with the name \"{walletName}\".");
                return false;
            }

            if (!Wallets[walletName].AdjustValue(increment, add))
                return false;

            Transaction.Type type = add ? Transaction.Type.ADD : Transaction.Type.SUB;
            Transaction newTrans = new(type)
            {
                Username = this.Username,
                WalletName = walletName,
                Value = increment,
                Time = DateTime.Now
            };
            Transactions.Add(newTrans);
            return true;
        }

        public bool AddWallet(string walletName)
        {
            if (Wallets.ContainsKey(walletName))
            {
                IOStream.OutputError($"You already have a wallet with the name \"{walletName}\".");
                return false;
            }

            Wallets.Add(walletName, new Wallet(walletName, 0));

            Transaction.Type type = Transaction.Type.NEWWALLET;
            Transaction newTrans = new(type)
            {
                Username = this.Username,
                WalletName = walletName,
                Time = DateTime.Now
            };
            Transactions.Add(newTrans);
            return true;
        }

        public bool RemoveWallet(string walletName)
        {
            if (!Wallets.ContainsKey(walletName))
            {
                IOStream.OutputError($"You have no wallet with the name \"{walletName}\".");
                return false;
            }

            Wallet wallet = Wallets[walletName];
            if (!AdjustValue(wallet.Value, true, Wallet.DefaultName))
            {
                IOStream.OutputError($"Your spare money is too much to add more money from wallet \"{walletName}\"");
                return false;
            }

            Wallets.Remove(walletName);

            Transaction.Type type = Transaction.Type.RMWALLET;
            Transaction newTrans = new(type)
            {
                Username = this.Username,
                WalletName = walletName,
                Time = DateTime.Now
            };
            Transactions.Add(newTrans);
            return true;
        }
    }
}
