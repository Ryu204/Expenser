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
            if (shortVersion)
                table.AddRow(date, operation, wallet, valueStr);
            else
                table.AddRow(date, operation, wallet, valueStr, tran.Message);
        }

        private static ConsoleTable CreateTable(bool shortVer)
        {
            if (shortVer)
                return new("Date", "Opr", "Wal", "Val");
            else
                return new("Date", "Operation", "Wallet", "Value", "Message");
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
            ConsoleTable table = CreateTable(shortVer);
            // Create a new line between 2 different dates
            DateOnly? date = null;
            foreach (var tran in account.Transactions)
            {
                if (date == null)
                    date = DateOnly.FromDateTime(tran.Time);
                else if (date != DateOnly.FromDateTime(tran.Time))
                {
                    date = DateOnly.FromDateTime(tran.Time);
                    if (shortVer)
                        table.AddRow(string.Empty, string.Empty, string.Empty, string.Empty);
                    else
                        table.AddRow(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
                }
                AddRowToDefaultTable(table, tran, shortVer);
            }
            IOStream.Output($"There were {account.Transactions.Count} transaction(s) in total:");
            table.Write();
            IOStream.Output($"You now have {account.Value:N0} VND.");
        }

        public static void LogFromTo(Account account, DateOnly begin, DateOnly end, bool shortVer)
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
                if (begin > DateOnly.FromDateTime(account.Transactions[i].Time.Date))
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
            ConsoleTable mainTable = CreateTable(shortVer);
            uint operationCount = 0;
            foreach (var tran in account.Transactions)
            {
                if (begin <= DateOnly.FromDateTime(tran.Time) 
                    && DateOnly.FromDateTime(tran.Time) <= end)
                {
                    ++operationCount;
                    AddRowToDefaultTable(mainTable, tran, shortVer);
                    AccountReverter.Accumulate(wallets, tran);
                }
            }
            IOStream.Output($"There were {operationCount} operation(s) during: ", false, ConsoleColor.White);
            IOStream.Output($"{begin.ToShortDateString()} - {end.ToShortDateString()}:");
            
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

        public static void LogOneDay(Account account, DateOnly date, bool shortVer)
        {
            LogFromTo(account, date, date, shortVer);
        }

        public static void LogOneWallet(Account account, string name, bool shortVer)
        {
            IOStream.Output("Current value: ", false, ConsoleColor.White);
            IOStream.Output($"{account.Wallets[name].Value:N0} VND");

            ConsoleTable table = shortVer ? new("Date", "Opr", "Val", "Tot") : new("Date", "Operation", "Value", "Total", "Message"); 
            // Create a new line between 2 different dates
            DateOnly? date = null;
            uint currentValue = account.Wallets[name].Value;

            for (int i = account.Transactions.Count - 1; i >= 0; --i)
            {
                var tran = account.Transactions[i];
                string wallet = ((int)tran.Operation & (int)Transaction.Component.WALLETNAME) != 0 ? tran.WalletName : string.Empty;
                if (wallet != name)
                    continue;
                if (tran.Operation == Transaction.Type.ADD)
                    currentValue -= account.Transactions[i].Value;
                else if (tran.Operation == Transaction.Type.SUB)
                    currentValue += account.Transactions[i].Value;
            }    
            foreach (var tran in account.Transactions)
            {
                string wallet = ((int)tran.Operation & (int)Transaction.Component.WALLETNAME) != 0 ? tran.WalletName : string.Empty;
                if (wallet != name)
                    continue;
                if (tran.Operation == Transaction.Type.ADD || tran.Operation == Transaction.Type.SUB)
                {
                    // New row if new day
                    if (date == null)
                        date = DateOnly.FromDateTime(tran.Time);
                    else if (date != DateOnly.FromDateTime(tran.Time))
                    {
                        date = DateOnly.FromDateTime(tran.Time);
                        if (shortVer)
                            table.AddRow(string.Empty, string.Empty, string.Empty, string.Empty);
                        else
                            table.AddRow(string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);
                    }
                    // Forming table
                    string dateStr = shortVer ? tran.Time.ToShortDateString() : tran.Time.ToLongDateString();
                    string operation = OperationToString(tran.Operation, shortVer);
                    uint value = ((int)tran.Operation & (int)Transaction.Component.VALUE) != 0 ? tran.Value : 0;
                    string valueStr;
                    if (tran.Operation == Transaction.Type.ADD)
                    {
                        valueStr = $"+ {value:N0} VND";
                        currentValue += value;
                    }
                    else if (tran.Operation == Transaction.Type.SUB)
                    {
                        valueStr = $"- {value:N0} VND";
                        currentValue -= value;
                    }
                    else
                        valueStr = string.Empty;
                    if (shortVer)
                        table.AddRow(dateStr, operation, valueStr, $"{currentValue:N0} VND");
                    else
                        table.AddRow(dateStr, operation, valueStr, $"{currentValue:N0} VND", tran.Message);
                }
            }
            IOStream.Output("Past transaction(s):");
            table.Write();
        }

    }
}
