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

        public string Username { get; private set; } = string.Empty;
        public uint Value { get; private set; } = 0;

        public Account() { }
        public Account(string username, string password, uint value, List<Transaction> transactions)
        {
            this.Password = password;
            this.Transactions = transactions;
            Username = username;
            Value = value;
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
                    IOStream.OutputError($"Addition value {increment} is too large.");
                else
                    IOStream.OutputError($"You only have {Value} VND.");
                return false;
            }

            Value = newValue;
            Transaction.Type type = add ? Transaction.Type.ADD : Transaction.Type.SUB;
            Transaction newTrans = new(type)
            {
                Username = this.Username,
                Value = increment,
            };
            Transactions.Add(newTrans);
            return true;
        }
    }
}
