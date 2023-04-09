using System.Diagnostics;
using Expenser.Utility;
using Expenser.State;

namespace Expenser.Core
{

    /// <summary>
    /// Control which state of the program is active
    /// </summary>
    public class StateStack
    {
        public bool Empty { get { return currentState == string.Empty; } }
        private readonly Dictionary<string, IState> statePool = new();

        private Context currentContext = new();

        private string currentState = string.Empty;
        private string pendingState = string.Empty;
        private bool popCurrentState = false;

        // All IState subclass must be registered here
        public StateStack()
        {
            RegisterState("Menu", new MenuState(this));

            currentState = "Menu";
        }

        public void RegisterSwitchState(string newState)
        {
            Debug.Assert(statePool.ContainsKey(newState));
            popCurrentState = true;
            pendingState = newState;
        }

        public void RegisterClose()
        {
            popCurrentState = true;
            pendingState = string.Empty;
        }

        public void Process()
        {
            Debug.Assert(currentState != string.Empty);

            string[] input = IOStream.GetInputAsArray();
            Command command = new();
            while (!CommandParser.TryParse(input, ref command, out string message))
            {
                IOStream.OutputError(message);
                input = IOStream.GetInputAsArray();
            }

            ExecuteCommand(command);

            if (popCurrentState)
            {
                popCurrentState = false;
                currentState = pendingState;
                statePool[currentState].Init();
            }
        }

        // Only give access to the current State
        // via stack.GetContext(this)
        public Context GetContext(IState requester)
        {
            Debug.Assert(requester == statePool[currentState]);
            return currentContext;
        }

        private void RegisterState(string ID, IState state)
        {
            Debug.Assert(state != null && !statePool.ContainsKey(ID));
            state.Init();
            statePool[ID] = state;
        }

        private void ExecuteCommand(Command command)
        {
            if (statePool[currentState].ValidateCommand(command))
            {
                currentContext.CurrentCommand = command;
                statePool[currentState].ProcessCommand();
            }
            else
                IOStream.OutputError(IState.ErrorMessage);
        }
    }
}
