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
        }

        private void Login()
        {
            Context context = GetContext();

            if (context.User != string.Empty)
            {
                IOStream.Output($"You must log out first, {context.User}.");
                return;
            }

            string username = context.CurrentCommand.Value[0];
            if (!char.IsLetter(username[0]))
            {
                IOStream.Output($"An username must start with a character.");
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
                IOStream.Output("You have not logged in");
                return;
            }
            context.Reset();
        }
    }
}
