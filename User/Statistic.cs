
using Expenser.Utility;

namespace Expenser.User
{
    /// <summary>
    /// Reliable for calculating expense statistics
    /// </summary>
    public static class Statistics
    {
        public static void DefaultLog(List<Transaction> transactions)
        {
            foreach (Transaction tran in transactions)
            {
                IOStream.Output($"{tran.Time}:\t", false);
                if (tran.Operation == Transaction.Type.NEWWALLET)
                    IOStream.OutputOther($"Created wallet \"{tran.WalletName}\"");
                else if (tran.Operation == Transaction.Type.ADD)
                    IOStream.OutputOther($"Added {tran.Value:N0} VND to \"{tran.WalletName}\"");
                else if (tran.Operation == Transaction.Type.SUB)
                    IOStream.OutputOther($"Subtracted {tran.Value:N0} VND from \"{tran.WalletName}\"");
            }
        }
    }
}
