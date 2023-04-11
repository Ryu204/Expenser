using Expenser.User;
using Expenser.Utility;
using ConsoleTables;

namespace Expenser.Report
{
    /// <summary>
    /// Reliable for calculating expense statistics
    /// </summary>
    public static class Statistics
    {
        public enum Flag
        {
            NONE = 0,
            ACTION = 1 << 0,
            TIME = 1 << 1,
        }

        private static string OperationToString(Transaction.Type type, bool shortVersion)
        {
            switch (type)
            {
                case Transaction.Type.NEWWALLET:
                    return shortVersion ? "C" : "CREATE";
                case Transaction.Type.RMWALLET:
                    return shortVersion ? "R" : "REMOVE";
                case Transaction.Type.ADD:
                    return shortVersion ? "A" : "ADD";
                case Transaction.Type.SUB:
                    return shortVersion ? "S" : "SUBTRACT";
                default:
                    return string.Empty;
            }
        }

        private static void AddRowToDefaultTable(ConsoleTable table, Transaction tran, bool shortVersion)
        {
            string date = shortVersion ? DateOnly.FromDateTime(tran.Time).ToShortDateString() :
                                         DateOnly.FromDateTime(tran.Time).ToLongDateString();
            string operation = OperationToString(tran.Operation, shortVersion);
            int value = ((int)tran.Operation & (int)Transaction.Component.VALUE) != 0 ? (int)tran.Value : 0;

            string valueStr;
            if (tran.Operation == Transaction.Type.ADD)
                valueStr = $"+ {value:N0} VND";
            else if (tran.Operation == Transaction.Type.SUB)
                valueStr = $"- {value:N0} VND";
            else
                valueStr = string.Empty;

            string wallet = ((int)tran.Operation & (int)Transaction.Component.WALLETNAME) != 0 ? tran.WalletName : string.Empty;
            table.AddRow(date, operation, wallet, valueStr);
        }

        private static ConsoleTable CreateTable(bool shortVer)
        {
            if (shortVer)
                return new("Date", "Opr", "Wal", "Val");
            else
                return new("Date", "Operation", "Wallet", "Value");
        }

        private static ConsoleTable TableFromWallets(Dictionary<string, Wallet> wallets)
        {
            ConsoleTable table = new("Wallet", "Value");
            foreach (var wallet in wallets)
                table.AddRow(wallet.Key, $"{wallet.Value.Value:N0}");
            return table;
        }

        public static void DefaultLog(Account account, bool shortVer)
        {
            IOStream.Output("Current state:");
            TableFromWallets(account.Wallets).Write();
            IOStream.Output("Past transaction(s):");
            ConsoleTable table = CreateTable(shortVer);
            foreach (var tran in account.Transactions)
                AddRowToDefaultTable(table, tran, shortVer);
            table.Write();
            IOStream.Output($"You now have {account.Value:N0} VND.");
        }

        public static void LogOneDay(Account account, DateOnly date, bool shortVer)
        {
            Dictionary<string, Wallet> wallets = AccountReverter.FetchFromAccount(account);
            uint before, after;

            Func<uint> Sum = () => 
            {
                uint res = 0;
                foreach (var wallet in wallets.Values)
                    res += wallet.Value;
                return res;
            };
            // Revert the time to before the start of the day
            for (int i = account.Transactions.Count - 1; i >= 0; --i)
            {
                if (date > DateOnly.FromDateTime(account.Transactions[i].Time.Date))
                    break;
                else
                    AccountReverter.Revert(wallets, account.Transactions[i]);
            }

            // Render stat before the day

            IOStream.Output($"Before: ", false, ConsoleColor.White);
            IOStream.Output($"{before = Sum():N0} VND", true, ConsoleColor.Green);
            if (!shortVer)
                TableFromWallets(wallets).Write();

            // Render what happened
            IOStream.Output($"Operations at ", false, ConsoleColor.White);
            IOStream.Output($"{date.ToShortDateString()}: ");
            ConsoleTable mainTable = CreateTable(shortVer);
            foreach (var tran in account.Transactions)
            {
                if (date == DateOnly.FromDateTime(tran.Time))
                {
                    AddRowToDefaultTable(mainTable, tran, shortVer);
                    AccountReverter.Accumulate(wallets, tran);
                }
            }
            mainTable.Write();

            // Render the final result
            IOStream.Output($"After: ", false, ConsoleColor.White);
            IOStream.Output($"{after = Sum():N0} VND", true, ConsoleColor.Green);
            IOStream.Output("Difference: ", false, ConsoleColor.White);
            if (before > after)
                IOStream.Output($"-{before - after:N0} VND", true, ConsoleColor.Red);
            else
                IOStream.Output($"+{after - before:N0} VND", true, ConsoleColor.Green);

            if (!shortVer)
                TableFromWallets(wallets).Write();
        }
    }
}
