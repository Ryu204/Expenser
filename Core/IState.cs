using System.Diagnostics;

namespace Expenser.Core
{ 
    /// <summary>
    /// Base class of program's states
    /// </summary>
    public abstract class IState
    {
        private readonly StateStack stack;

        protected delegate void Function();
        private readonly Dictionary<RuleChecker, Function> actions = new();
        private readonly HashSet<string> actionsNames = new();
        RuleChecker? callbackKey;

        // Static status indicator of all states
        public static string ErrorMessage { get; protected set; } = string.Empty;

        public IState(StateStack stack)
        {
            this.stack = stack;
        }

        public virtual void Init() { }

        // Should be checked before a call to ProcessCommand is made
        // Passing diagnostics message if the return value is false
        public bool ValidateCommand(Command command)
        {
            // If none of the actions' name matches
            if (!actionsNames.Contains(command.Action))
            {
                ErrorMessage = $"There is no action called \"{command.Action}\" in current context.";
                return false;
            }
            // Check if the action's name is valid
            foreach (var action in actions)
            {
                if (action.Key.Check(command))
                {
                    // Update the Context
                    GetContext().CurrentCommand = command;
                    callbackKey = action.Key;
                    return true;
                }
            }
            ErrorMessage = $"There is no call to action \"{command.Action}\" that takes {command.Value.Length} value(s).";
            return false;
        }

        public void ProcessCommand()
        {
            Debug.Assert(callbackKey != null && actions.ContainsKey(callbackKey));
            actions[callbackKey]();
        }

        protected void AddAction(RuleChecker rule, Function funcion)
        {
            Debug.Assert(!actions.ContainsKey(rule));
            actions[rule] = funcion;
            actionsNames.Add(rule.Action);
        }
          
        protected void SwitchTo(string state)
        {
            stack.RegisterSwitchState(state);
        }

        protected Context GetContext()
        {
            return stack.GetContext(this);
        }
    }
}