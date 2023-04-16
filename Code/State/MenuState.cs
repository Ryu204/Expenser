using Expenser.Core;
using Expenser.Report;
using Expenser.User;
using Expenser.Utility;
using System.Globalization;
using System.Xml.Linq;

namespace Expenser.State
{
    /// <summary>
    /// The beginning state of the program
    /// </summary>
    public class MenuState : IState
    {
        private Account account = new();
        private readonly HashSet<string> registeredUsers = new();

        public MenuState(StateStack stack)
            : base(stack)
        {
            registeredUsers = AccountLoader.LoadUserListFromFile();

            // Add operations
            RuleChecker rule1 = new("login", new Type[] { typeof(string) });
            Function func1 = Login;
            AddOperation(rule1, func1);

            RuleChecker rule2 = new("logout", Array.Empty<Type>());
            Function func2 = Logout;
            AddOperation(rule2, func2);

            RuleChecker rule3 = new("add", new Type[] { typeof(uint) });
            Function func3 = AddDefault;
            AddOperation(rule3, func3);

            RuleChecker rule325 = new("add", new Type[] { typeof(uint), typeof(string) });
            Function func325 = Add;
            AddOperation(rule325, func325);

            RuleChecker rule35 = new("sub", new Type[] { typeof(uint) });
            Function func35 = SubtractDefault;
            AddOperation(rule35, func35);

            RuleChecker rule375 = new("sub", new Type[] { typeof(uint), typeof(string) });
            Function func375 = Subtract;
            AddOperation(rule375, func375);

            RuleChecker rule4 = new("exit", Array.Empty<Type>());
            Function func4 = Exit;
            AddOperation(rule4, func4);

            RuleChecker rule5 = new("signup", new Type[] { typeof(string) });
            Function func5 = SignUp;
            AddOperation(rule5, func5);

            RuleChecker rule55 = new("new", new Type[] { typeof(string) });
            Function func55 = New;
            AddOperation(rule55, func55);

            RuleChecker rule575 = new("delete", new Type[] { typeof(string) });
            Function func575 = Delete;
            AddOperation(rule575, func575);

            RuleChecker rule6 = new("log", Array.Empty<Type>());
            Function func6 = Log;
            AddOperation(rule6, func6);

            RuleChecker rule7 = new("log", new Type[] { typeof(DateOnly) });
            Function func7 = LogOneDay;
            AddOperation(rule7, func7);

            RuleChecker rule8 = new("log", new Type[] { typeof(DateOnly), typeof(DateOnly) });
            Function func8 = LogFromTo;
            AddOperation(rule8, func8);

            RuleChecker rule9 = new("log", new Type[] { typeof(string) });
            Function func9 = LogOneWallet;
            AddOperation(rule9, func9);
        }

        protected override void Save()
        {
            if (GetContext().User != string.Empty)
                AccountLoader.SaveUserToFile(account);
        }

        private bool CheckLoggedOut()
        {
            if (GetContext().User != string.Empty)
            {
                IOStream.OutputError($"You must log out first, {GetContext().User}.");
                IOStream.OutputOther("[help] Try \"logout\" to log out.");
                return false;
            }
            return true;
        }

        private bool CheckLoggedIn()
        {
            if (GetContext().User == string.Empty)
            {
                IOStream.OutputError($"You must log in first.");
                IOStream.OutputOther("[help] Try \"login <username>\" to log in.");
                return false;
            }
            return true;
        }

        private bool CheckValidUsername(string username)
        {
            if (!Account.IsUsername(username))
            {
                IOStream.OutputError($"{username} is not a valid username.");
                IOStream.OutputOther("[help] An username consists of only letters.");
                return false;
            }
            return true;
        }

        private bool CheckUserExist(string username)
        {
            if (!registeredUsers.Contains(username))
            {
                IOStream.OutputError($"Username {username} does not exist.");
                return false;
            }
            return true;
        }

        private bool AuthorisePassword(string password)
        {
            string? input;
            try
            {
                while (true)
                {
                    IOStream.Output("Enter your password: ", false);
                    input = Console.ReadLine();
                    if (input == password)
                        return true;
                    else
                    {
                        IOStream.OutputError("Wrong password.");
                        if (IOStream.YesNoPrompt("Try again?"))
                            continue;
                        else
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                IOStream.OutputOther(ex.Message);
            }
            return false;
        }

        private void Login()
        {
            Context context = GetContext();
            if (!CheckLoggedOut())
                return;
            string username = context.CurrentCommand.Value[0];
            if (!CheckValidUsername(username))
                return;
            if (!CheckUserExist(username))
                return;
            Account acc = new();
            if (!AccountLoader.TryLoadUserFromFile(username, ref acc))
                return;
            if (!AuthorisePassword(acc.Password))
                return;
            account = acc;
            context.User = username;
            IOStream.Output($"Hello {username}. You currently have {account.Value:N0} VND in your account.");
        }

        private void Logout()
        {
            Context context = GetContext();

            if (!CheckLoggedIn())
                return;
            AccountLoader.SaveUserToFile(account);
            IOStream.Output("Logged out.");
            context.Reset();
        }

        private void SaveUserList()
        {
            try
            {
                string path = "Users/userlist.list";
                using (StreamWriter writer = File.CreateText(path))
                {
                    foreach (var user in registeredUsers)
                        writer.WriteLine(user);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void SignUp()
        {
            Context context = GetContext();
            if (!CheckLoggedOut())
                return;
            string username = context.CurrentCommand.Value[0];
            if (!CheckValidUsername(username))
                return;
            if (registeredUsers.Contains(username))
            {
                IOStream.OutputError($"Username {username} has been taken.");
                return;
            }
            if (AuthoriseSignUp(username))
            {
                context.Reset();
                context.User = account.Username;
                registeredUsers.Add(username);
                SaveUserList();
            }
        }

        public bool AuthoriseSignUp(string username)
        {
            string? password;
            bool passwordConfirmed = false;
           
            // Read password
            while (true)
            {
                IOStream.Output("Enter your password: ", false);
                password = Console.ReadLine();
                if (password == null || !Account.IsPassword(password))
                {
                    IOStream.OutputError("Password must be a combination of at least 6 letters and/or digits.");
                    if (IOStream.YesNoPrompt("Try again?"))
                        continue;
                    else
                        break;
                }
                else
                {
                    IOStream.Output("Confirm your password: ", false);
                    string? cfPassword = Console.ReadLine();
                    if (cfPassword != password)
                    {
                        IOStream.OutputError("Those passwords do not match.");
                        return false;
                    }
                    else
                    {
                        passwordConfirmed = true;
                        break;
                    }
                }
            }
            if (!passwordConfirmed)
                return false;

            account = new(username, password, new(), new());

            registeredUsers.Add(username);
            IOStream.Output($"Welcome, {username}!");
            return true;
        }

        private void AddDefault()
        {
            Context context = GetContext();

            if (!CheckLoggedIn())
                return;

            uint increment = uint.Parse(context.CurrentCommand.Value[0]);
            if (account.AdjustValue(increment, true, Wallet.DefaultName))
                IOStream.Output($"Added {increment:N0} VND to your account. You now have {account.Value:N0} VND.");
        }

        private void Add()
        {
            Context context = GetContext();
            string wallet = context.CurrentCommand.Value[1];
            if (wallet == Wallet.DefaultName)
            {
                AddDefault();
                return;
            }

            if (!CheckLoggedIn())
                return;

            uint increment = uint.Parse(context.CurrentCommand.Value[0]);

            if (!Wallet.IsWalletName(wallet))
            {
                IOStream.OutputError($"\"{wallet}\" is not a valid wallet name.");
                return;
            }

            if (context.CurrentCommand.Flags.Contains("transfer"))
            {
                if (account.AdjustValue(increment, true, wallet, false, true) 
                    && account.AdjustValue(increment, false, Wallet.DefaultName, false, true))
                {
                    account.AdjustValue(increment, false, Wallet.DefaultName, false);
                    account.AdjustValue(increment, true, wallet);
                }

                context.CurrentCommand.Flags.Remove("transfer");
            }
            else if (account.AdjustValue(increment, true, wallet))
                    IOStream.Output($"Added {increment:N0} VND to wallet \"{wallet}\". You now have {account.Value:N0} VND.");
        }

        private void SubtractDefault()
        {
            Context context = GetContext();

            if (!CheckLoggedIn())
                return;

            uint increment = uint.Parse(context.CurrentCommand.Value[0]);
            if (account.AdjustValue(increment, false, Wallet.DefaultName))
                IOStream.Output($"Subtracted {increment:N0} VND to your account. You now have {account.Value:N0} VND.");
        }

        private void Subtract()
        {
            Context context = GetContext();
            string wallet = context.CurrentCommand.Value[1];
            if (wallet == Wallet.DefaultName)
            {
                SubtractDefault();
                return;
            }

            if (!CheckLoggedIn())
                return;

            uint increment = uint.Parse(context.CurrentCommand.Value[0]);

            if (!Wallet.IsWalletName(wallet))
            {
                IOStream.OutputError($"\"{wallet}\" is not a valid wallet name.");
                return;
            }

            if (context.CurrentCommand.Flags.Contains("transfer"))
            {
                if (account.AdjustValue(increment, false, wallet, false, true)
                    && account.AdjustValue(increment, true, Wallet.DefaultName, false, true))
                {
                    account.AdjustValue(increment, true, Wallet.DefaultName, false);
                    account.AdjustValue(increment, false, wallet);
                }

                context.CurrentCommand.Flags.Remove("transfer");
            }
            else if (account.AdjustValue(increment, false, wallet))
                IOStream.Output($"Subtracted {increment:N0} VND to wallet \"{wallet}\". You now have {account.Value:N0} VND.");
        }

        private void New()
        {
            Context context = GetContext();
            string name = context.CurrentCommand.Value[0];
            if (!CheckLoggedIn())
                return;
            if (!Wallet.IsWalletName(name))
            {
                IOStream.OutputError($"\"{name}\" is not a valid wallet name.");
                return;
            }

            if (account.AddWallet(name))
            {
                IOStream.Output($"Created wallet \"{name}\".");
                context.Wallet = name;
                return;
            }
        }

        private void Delete()
        {
            if (GetContext().User != string.Empty)
                DeleteWallet();
            else
                DeleteAccount();
        }

        private void DeleteAccount()
        {
            Context context = GetContext();

            string name = context.CurrentCommand.Value[0];
            if (!CheckLoggedOut())
                return;
            if (!CheckValidUsername(name))
                return;
            if (!CheckUserExist(name))
                return;
            Account acc = new();
            if (!AccountLoader.TryLoadUserFromFile(name, ref acc))
            {
                IOStream.OutputError($"There was a problem checking user {name}'s data. The deletion could not proceed.");
                return;
            }
            if (AuthorisePassword(acc.Password))
            {
                try
                {
                    registeredUsers.Remove(name);
                    File.Delete($"Users/{name}{GrammarChecker.UserFileSuffix}");
                    SaveUserList();
                    IOStream.Output($"User {name}'s data has been removed from the database.");
                }
                catch(Exception e)
                {
                    IOStream.OutputError("The deletion failed because of system error:");
                    Console.WriteLine(e.Message);
                    return;
                }
            }
        }

        private void DeleteWallet()
        {
            Context context = GetContext();
            string name = context.CurrentCommand.Value[0];

            if (!CheckLoggedIn())
                return;
            if (!Wallet.IsWalletName(name))
            {
                IOStream.OutputError($"\"{name}\" is not a valid wallet name.");
                return;
            }
            if (name == Wallet.DefaultName)
            {
                IOStream.OutputError($"You cannot delete this wallet.");
                return;
            }
            if (!account.Wallets.ContainsKey(name))
            {
                IOStream.OutputError($"You does not have a wallet with the name \"{name}\".");
                return;
            }
            if (account.RemoveWallet(name))
            {
                IOStream.Output($"\"{name}\" has been removed from your wallet list.");
            }
        }

        private void Exit()
        {            
            IOStream.Output("Goodbye.");
            CloseStack();
        }

        private void Log()
        {
            Context context = GetContext();
            if (!CheckLoggedIn())
                return;
            else
            {
                HashSet<string> flags = context.CurrentCommand.Flags;
                bool shortVersion = false;
                DateOnly today = DateOnly.FromDateTime(DateTime.Now);
                if (flags.Contains("short"))
                {
                    shortVersion = true;
                    flags.Remove("short");
                }
                
                if (flags.Contains("day"))
                {
                    Statistics.LogOneDay(account, today, shortVersion);
                    flags.Remove("day");
                }
                else if (flags.Contains("week"))
                {
                    Statistics.LogFromTo(account, today.AddDays(-7), today, shortVersion);
                    flags.Remove("week");
                }
                else if (flags.Contains("month"))
                {
                    Statistics.LogFromTo(account, today.AddDays(-30), today, shortVersion);
                    flags.Remove("month");
                }
                else if (flags.Contains("year"))
                {
                    Statistics.LogFromTo(account, today.AddDays(-365), today, shortVersion);
                    flags.Remove("year");
                }
                else
                    Statistics.DefaultLog(account, shortVersion);
            }
        }

        private void LogOneDay()
        {
            Context context = GetContext();
            if (!CheckLoggedIn())
                return;
            DateOnly date = DateOnly.Parse(context.CurrentCommand.Value[0]);
            bool shortVersion = false;
            if (context.CurrentCommand.Flags.Contains("short"))
            {
                shortVersion = true;
                context.CurrentCommand.Flags.Remove("short");
            }
            Statistics.LogOneDay(account, date, shortVersion);
        }

        private void LogFromTo()
        {
            Context context = GetContext();
            if (!CheckLoggedIn())
                return;
            DateOnly begin = DateOnly.Parse(context.CurrentCommand.Value[0]);
            DateOnly end = DateOnly.Parse(context.CurrentCommand.Value[1]);
            if (end < begin)
            {
                var tmp = end; end = begin; begin = tmp;
            }
            bool shortVersion = false;
            if (context.CurrentCommand.Flags.Contains("short"))
            {
                shortVersion = true;
                context.CurrentCommand.Flags.Remove("short");
            }
            Statistics.LogFromTo(account, begin, end, shortVersion);
        }

        private void LogOneWallet()
        {
            Context context = GetContext();
            if (!CheckLoggedIn())
                return;
            string name = context.CurrentCommand.Value[0];
            if (!Wallet.IsWalletName(name))
            {
                IOStream.OutputError($"\"{name}\" is not a valid wallet name.");
                return;
            }
            if (!account.Wallets.ContainsKey(name))
            {
                IOStream.OutputError($"You does not have a wallet with the name \"{name}\".");
                return;
            }
            bool shortVersion = false;
            var flags = context.CurrentCommand.Flags;
            var today = DateOnly.FromDateTime(DateTime.Now);
            if (flags.Contains("short"))
            {
                shortVersion = true;
                context.CurrentCommand.Flags.Remove("short");
            }
            Statistics.LogOneWallet(account, name, shortVersion);
        }
    }
}
