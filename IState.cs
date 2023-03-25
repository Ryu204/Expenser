namespace Expenser
{
    namespace Core
    {
        public abstract class IState
        {
            StateStack stack;

            public IState(StateStack stack)
            {
                this.stack = stack;
            }

            public abstract void Init();

            // Must be called before a call to ProcessCommand is made
            public abstract bool ValidateCommand(string command, ref string message);

            public abstract void ProcessCommand(string command);
          
            protected void SwitchTo(string state)
            {
                stack.RegisterSwitch(state);
            }
        }
    }
}