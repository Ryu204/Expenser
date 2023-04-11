using Expenser.User;

namespace Expenser.Report
{
    /// <summary>
    /// Revert changes from Transaction
    /// </summary>
    public static class AccountReverter
    {
        public static Dictionary<string, Wallet> FetchFromAccount(Account account)
        {
            Dictionary<string, Wallet> res = new();
            foreach (var wallet in account.Wallets)
                res.Add(wallet.Value.Name, new(wallet.Value.Name, wallet.Value.Value));
            return res;
        }

        public static void Revert(Dictionary<string, Wallet> wallets, Transaction tran)
        {
            if (tran.Operation == Transaction.Type.RMWALLET)
                wallets.Add(tran.WalletName, new Wallet(tran.WalletName, 0));
            else if (tran.Operation == Transaction.Type.NEWWALLET)
                wallets.Remove(tran.WalletName); 
            else if (tran.Operation == Transaction.Type.ADD)
                wallets[tran.WalletName] = new(tran.WalletName, wallets[tran.WalletName].Value - tran.Value);
            else if (tran.Operation == Transaction.Type.SUB)
                wallets[tran.WalletName] = new(tran.WalletName, wallets[tran.WalletName].Value + tran.Value);
        }

        public static void Accumulate(Dictionary<string, Wallet> wallets, Transaction tran)
        {
            if (tran.Operation == Transaction.Type.NEWWALLET)
                wallets.Add(tran.WalletName, new Wallet(tran.WalletName, 0));
            else if (tran.Operation == Transaction.Type.RMWALLET)
                wallets.Remove(tran.WalletName);
            else if (tran.Operation == Transaction.Type.SUB)
                wallets[tran.WalletName] = new(tran.WalletName, wallets[tran.WalletName].Value - tran.Value);
            else if (tran.Operation == Transaction.Type.ADD)
                wallets[tran.WalletName] = new(tran.WalletName, wallets[tran.WalletName].Value + tran.Value);
        }
    }
}
