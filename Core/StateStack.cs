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
        public bool Empty { get; private set; } = false;
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
            Debug.Assert(!Empty);

            string[] input = IOStream.GetInputAsArray();
            Command command = new();
            while (!Command.Parser.TryParse(input, ref command, out string message))
            {
                message = $"Parser error: {message}.";
                IOStream.OutputError(message);
                input = IOStream.GetInputAsArray();
            }

            ExecuteCommand(command);

            if (popCurrentState)
            {
                popCurrentState = false;
                if (pendingState == string.Empty)
                {
                    Empty = true;
                    return;
                }
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
            currentContext.CurrentCommand = command;
            if (statePool[currentState].ValidateCommand(command, out string message))
            {
                currentContext.CurrentCommand = command;
                statePool[currentState].ProcessCommand();
            }
            else
                IOStream.OutputError(message);
        }
    }
}
