using Expenser.User;

namespace Expenser.Core
{
    public class Context
    {
        public string User = string.Empty;
        public string Wallet = string.Empty;
        public Command CurrentCommand = new();

        public void Reset()
        {
            User = string.Empty;
            Wallet = string.Empty;
            CurrentCommand = new();
        }
    }
}
