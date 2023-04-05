using Expenser.Core;
using Expenser.User;
using Expenser.Utility;

namespace Expenser.State
{
    /// <summary>
    /// The initial state of the program
    /// </summary>
    public class MenuState : IState
    {
        private readonly Account account = new();

        public MenuState(StateStack stack)
            : base(stack)
        {
            RuleChecker rule1 = new("login", new Type[] { typeof(string) });
            Function func1 = Login;
            AddAction(rule1, func1);

            RuleChecker rule2 = new("logout", Array.Empty<Type>());
            Function func2 = Logout;
            AddAction(rule2, func2);

            RuleChecker rule3 = new("add", new Type[] { typeof(uint) });
            Function func3 = Add;
            AddAction(rule3, func3);

            RuleChecker rule4 = new("exit", Array.Empty<Type>());
            Function func4 = Exit;
            AddAction(rule4, func4);

            RuleChecker rule5 = new("signup", Array.Empty<Type>());
            Function func5 = Signup;
            AddAction(rule5, func5);
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
            if (!char.IsLetter(username[0]))
            {
                IOStream.OutputError($"An username must start with a character.");
                return;
            }

            if (account.SetUser(username)) 
            {
                context.User = username;
                IOStream.Output($"Hello {username}. You currently have {account.Value:N0} VND in your account.");
            }
        }

        private void Logout()
        {
            Context context = GetContext();

            if (context.User == string.Empty)
            {
                IOStream.OutputError("You have not logged in.");
                return;
            }
            account.SaveUsersData();
            IOStream.Output("Logged out.");
            context.Reset();
        }

        private void Signup()
        {
            Context context = GetContext();
            if (context.User != string.Empty)
            {
                IOStream.OutputError("You must logout first.");
                return;
            }

            if (account.Signup())
            {
                context.Reset();
                context.User = account.Username;
            }
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
            if (account.Add(increment))
                IOStream.Output($"Added {increment:N0} VND to user {account.Username}'s account. You now have {account.Value:N0} VND.");
        }

        private void Exit()
        {
            if (!string.IsNullOrWhiteSpace(GetContext().User))
                account.SaveUsersData();
            
            IOStream.Output("Goodbye.");
            CloseStack();
        }
    }
}
