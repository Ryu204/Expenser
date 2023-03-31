namespace Expenser.Core
{ 
    public abstract class IState
    {
        private readonly StateStack stack;

        public IState(StateStack stack)
        {
            this.stack = stack;
        }

        public abstract void Init();

        // Should be checked before a call to ProcessCommand is made
        public abstract bool ValidateCommand(Command command, ref string message);

        public abstract void ProcessCommand(Command command);
          
        protected void SwitchTo(string state)
        {
            stack.RegisterSwitch(state);
        }
    }
}