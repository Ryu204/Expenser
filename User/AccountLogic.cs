using System.ComponentModel;
using System.Diagnostics;
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
        public string Username { get; private set; } = string.Empty;
        private string password = string.Empty;
        public uint Value { get; private set; } = 0;

        private readonly HashSet<string> registeredUsers = new();
        private List<Transaction> transactions = new List<Transaction>();

        public Account()
        {
            LoadUserListFromFile();
        }

        public bool Add(uint increment)
        {
            uint newValue;
            try
            {
                checked
                {
                    newValue = Value + increment;
                }
            }
            catch (OverflowException)
            {
                IOStream.OutputError($"Addition value {increment} is too large.");
                return false;
            }

            Value = newValue;
            Transaction newTrans = new Transaction(Transaction.Type.ADD)
            {
                Username = this.Username,
                Value = increment,
            };
            transactions.Add(newTrans);
            return true;
        }

        public bool Signup()
        {
            string? name, password;
            bool usernameConfirmed = false, passwordConfirmed = false;
            // Read username
            while (true)
            {
                IOStream.Output("Enter your username: ", false);
                name = Console.ReadLine();
                if (string.IsNullOrEmpty(name) || name.Length <= 2 || !IOStream.IsAllLetter(name))
                {
                    IOStream.OutputError("A name must consists of only letters and have length at least 3. Special characters are not allowed.\nTry again? [y/n]");
                    string? confirm = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(confirm))
                        break;
                    else if (confirm.ToLower() == "y")
                        continue;
                    else
                        break;
                }
                else if (registeredUsers.Contains(name))
                {
                    IOStream.OutputError($"The username {name} has been taken. Try again? [y/n]");
                    string? confirm = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(confirm))
                        break;
                    else if (confirm.ToLower() == "y")
                        continue;
                    else
                        break;
                }
                else
                {
                    usernameConfirmed = true;
                    break;
                }
            }

            if (!usernameConfirmed)
                return false;
            // Read password
            while (true)
            {
                IOStream.Output("Enter your password: ", false);
                password = Console.ReadLine();
                if (string.IsNullOrEmpty(password))
                {
                    IOStream.OutputError("Empty password. Try again? [y/n]");
                    string? confirm = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(confirm))
                        break;
                    else if (confirm.ToLower() == "y")
                        continue;
                    else
                        break;
                }
                else
                {
                    bool continous = true;
                    foreach (char c in password)
                        if (char.IsWhiteSpace(c))
                        {
                            continous = false;
                        }
                    if (password.Length <= 5 || !continous)
                    {
                        IOStream.OutputError("Password must be continous and have length at least 6.\nTry again? [y/n]");
                        string? confirm = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(confirm))
                            break;
                        else if (confirm.ToLower() == "y")
                            continue;
                        else
                            break;
                    }
                    else
                    {
                        bool valid = true;
                        foreach (char c in password)
                            if (!char.IsAsciiLetterOrDigit(c))
                                valid = false;
                        if (!valid)
                        {
                            IOStream.OutputError("Password must only consist of ASCII letters and numbers.\nTry again? [y/n]");
                            string? confirm = Console.ReadLine();
                            if (string.IsNullOrWhiteSpace(confirm))
                                break;
                            else if (confirm.ToLower() == "y")
                                continue;
                            else
                                break;
                        }
                        else
                        {
                            passwordConfirmed = true;
                            break;
                        }
                    }
                }
            }

            if (passwordConfirmed)
            {
                Username = name;
                this.password = password;
                Value = 0;
                transactions.Clear();
                registeredUsers.Add(Username);
                IOStream.Output($"Welcome, {Username}!");
                return true;
            }

            return false;
        }
    }
}
