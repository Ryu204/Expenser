﻿using Expenser.Core;
using Expenser.User;
using Expenser.Utility;
using System.Transactions;

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
            Function func3 = Add;
            AddOperation(rule3, func3);

            RuleChecker rule35 = new("sub", new Type[] { typeof(uint) });
            Function func35 = Subtract;
            AddOperation(rule35, func35);

            RuleChecker rule4 = new("exit", Array.Empty<Type>());
            Function func4 = Exit;
            AddOperation(rule4, func4);

            RuleChecker rule5 = new("signup", new Type[] { typeof(string) });
            Function func5 = SignUp;
            AddOperation(rule5, func5);
        }

        private void Login()
        {
            Context context = GetContext();

            if (context.User != string.Empty)
            {
                IOStream.OutputError($"You must log out first, {context.User}.");
                return;
            }

            string username = context.CurrentCommand.Value[0];
            if (!Account.IsUsername(username))
            {
                IOStream.OutputError($"{username} is not a valid username.");
                return;
            }

            if (!registeredUsers.Contains(username))
            {
                IOStream.OutputError($"Username {username} does not exist.");
                return;
            }

            Account acc = new();
            if (!AccountLoader.TryLoadUserFromFile(username, ref acc))
                return;

            if (!AuthoriseLogin(acc.Password))
                return;

            account = acc;
            context.User = username;
            IOStream.Output($"Hello {username}. You currently have {account.Value:N0} VND in your account.");
        }

        private bool AuthoriseLogin(string password)
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
                        if (IOStream.TryAgainPrompt())
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

        private void Logout()
        {
            Context context = GetContext();

            if (context.User == string.Empty)
            {
                IOStream.OutputError("You have not logged in.");
                return;
            }
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
            if (context.User != string.Empty)
            {
                IOStream.OutputError($"You must logout first, {context.User}.");
                return;
            }

            string username = context.CurrentCommand.Value[0];
            if (!Account.IsUsername(username))
            {
                IOStream.OutputError($"{username} is not a valid username.");
                return;
            }

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
                    if (IOStream.TryAgainPrompt())
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

            account = new(username, password, 0, new List<User.Transaction>());

            registeredUsers.Add(username);
            IOStream.Output($"Welcome, {username}!");
            return true;
        }

        private void Add()
        {
            Context context = GetContext();

            if (string.IsNullOrWhiteSpace(context.User))
            {
                IOStream.OutputError("You have not loggin in.");
                return;
            }

            uint increment = uint.Parse(context.CurrentCommand.Value[0]);
            if (account.AdjustValue(increment, true))
                IOStream.Output($"Added {increment:N0} VND to user {account.Username}'s account. You now have {account.Value:N0} VND.");
        }

        private void Subtract()
        {
            Context context = GetContext();

            if (string.IsNullOrWhiteSpace(context.User))
            {
                IOStream.OutputError("You have not loggin in.");
                return;
            }

            uint increment = uint.Parse(context.CurrentCommand.Value[0]);
            if (account.AdjustValue(increment, false))
                IOStream.Output($"Subtracted {increment:N0} VND to user {account.Username}'s account. You now have {account.Value:N0} VND.");
        }

        private void Exit()
        {
            if (!string.IsNullOrWhiteSpace(GetContext().User))
                AccountLoader.SaveUserToFile(account);
            
            IOStream.Output("Goodbye.");
            CloseStack();
        }
    }
}
