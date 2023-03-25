using System.Diagnostics;

namespace Expenser
{
    namespace Core
    {
        public class StateStack
        {
            Dictionary<string, IState> statePool = new Dictionary<string, IState>();
            string currentState = string.Empty;
            string pendingState = string.Empty;
            bool popCurrentState = false;

            IOStream ios = new IOStream();

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

            bool ExecuteCommand(string command)
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
                string input = ios.Input();
                while (!ios.Good)
                {
                    ios.Output("You have entered an empty command.");
                    input = ios.Input();
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
}
