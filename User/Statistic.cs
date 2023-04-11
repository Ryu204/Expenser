using Expenser.Utility;

namespace Expenser.User
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

        private static void PrintDescription(Transaction tran, int strip = 0)
        {
            if ((strip & (int)Flag.ACTION) == 0)
            {
                if (tran.Operation == Transaction.Type.ADD)
                    IOStream.Output("ADD\t", false, ConsoleColor.Blue);
                else if (tran.Operation == Transaction.Type.SUB)
                    IOStream.Output("SUBTRACT\t", false, ConsoleColor.Blue);
                else if (tran.Operation == Transaction.Type.NEWWALLET)
                    IOStream.Output("CREATE\t", false, ConsoleColor.Blue);
                else if (tran.Operation == Transaction.Type.RMWALLET)
                    IOStream.Output("REMOVE\t", false, ConsoleColor.Blue);
            }

            if (tran.Operation == Transaction.Type.SUB)
                IOStream.Output($"|-{tran.Value}VND|", false, ConsoleColor.Red);
            if (tran.Operation == Transaction.Type.ADD)
                IOStream.Output($"|+{tran.Value}VND|", false, ConsoleColor.Green);

            ConsoleColor color = ConsoleColor.White;
            string symbol = "";
            if (tran.Operation == Transaction.Type.RMWALLET)
            {
                color = ConsoleColor.Red;
                symbol += '-';
            }
            else if (tran.Operation == Transaction.Type.NEWWALLET)
            {
                color = ConsoleColor.Green;
                symbol += '+';
            }
            if (((int)tran.Operation & (int)Transaction.Component.WALLETNAME) != 0)
                IOStream.Output($"|{symbol}{tran.WalletName}|", false, color);
            IOStream.Output(string.Empty);
        }

        public static void DefaultLog(List<Transaction> transactions, int flags)
        {
            if (transactions.Count == 0)
                return;

            DateOnly date = DateOnly.FromDateTime(transactions[0].Time);
            foreach (Transaction tran in transactions)
            {
                if (date != DateOnly.FromDateTime(tran.Time))
                {
                    IOStream.Output("---------------------------", true, ConsoleColor.Yellow);
                    date = DateOnly.FromDateTime(tran.Time);
                }
                if ((flags & (int)Flag.TIME) == 0)
                    IOStream.Output($"{tran.Time}|", false, ConsoleColor.White);
                else
                    IOStream.Output($"{tran.Time.ToShortDateString()}|", false, ConsoleColor.White);
                PrintDescription(tran, flags);
            }
        }

        public static void LogOneDay(List<Transaction> transactions, DateOnly date, int flags)
        {
            foreach (var tran in transactions)
            {
                if (date == DateOnly.FromDateTime(tran.Time))
                    PrintDescription(tran, flags);
            }
        }
    }
}
