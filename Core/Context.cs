namespace Expenser.Core
{
    public class Context
    {
        public string User = string.Empty;
        public Command CurrentCommand = new();

        public void Reset()
        {
            User = string.Empty;
            CurrentCommand = new();
        }
    }
}
