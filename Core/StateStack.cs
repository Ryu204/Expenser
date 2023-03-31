using System.Diagnostics;
using Expenser.Utility;
using Expenser.State;

namespace Expenser.Core
{
    public class StateStack
    {
        private readonly Dictionary<string, IState> statePool = new();
        private string currentState = string.Empty;
        private string pendingState = string.Empty;
        private bool popCurrentState = false;

        private IOStream ios = new();

        public StateStack()
        {
            RegisterState("Menu", new MenuState(this));

            currentState = "Menu";
        }

        void RegisterState(string ID, IState state)
        {
            Debug.Assert(state != null && !statePool.ContainsKey(ID));
            state.Init();
            statePool[ID] = state;
        }

        bool ExecuteCommand(Command command)
        {
            string message = string.Empty;
            if (statePool[currentState].ValidateCommand(command, ref message))
            {
                statePool[currentState].ProcessCommand(command);
                return true;
            }
            else
            {
                ios.Output(message);
                return false;
            }
        }

        public void RegisterSwitch(string newState)
        {
            Debug.Assert(statePool.ContainsKey(newState));
            popCurrentState = true;
            pendingState = newState;
        }

        public void Process()
        {
            Command input = ios.ParseCommand();
            while (ios.State != IOStream.InputState.OKAY)
            {
                ios.Output(ios.Message);
                input = ios.ParseCommand();
            }

            ExecuteCommand(input);

            if (popCurrentState)
            {
                popCurrentState = false;
                currentState = pendingState;
                statePool[currentState].Init();
            }
        }
    }
}
