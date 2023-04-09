using System.Diagnostics;
using System.Net.NetworkInformation;

namespace Expenser.Core
{ 
    /// <summary>
    /// Base class of program's states
    /// </summary>
    public abstract class IState
    {
        private readonly StateStack stack;

        protected delegate void Function();
        private readonly Dictionary<RuleChecker, Function> commandSet = new();
        private readonly HashSet<string> commandNames = new();
        RuleChecker? callbackKey = null;

        public IState(StateStack stack)
        {
            this.stack = stack;
        }

        public virtual void Init() { }

        // Should be checked before a call to ProcessCommand is made
        // Check if the command matches a possible rule
        public bool ValidateCommand(Command command, out string message)
        {
            if (!commandNames.Contains(command.Action))
            {
                message = $"There is no action called \"{command.Action}\" in current context.";
                return false;
            }

            uint matchArgumentCount = 0;

            foreach (var rule in commandSet)
            {
                if (rule.Key.Check(command))
                {
                    callbackKey = rule.Key;
                    message = string.Empty;
                    return true;
                }
                else if (command.Action == rule.Key.Action && command.Value.Length == rule.Key.Arguments.Length)
                    ++matchArgumentCount;
            }

            if (matchArgumentCount == 0)
                message = $"There is no call to action \"{command.Action}\" that takes {command.Value.Length} value(s).";
            else
                message = $"There are {matchArgumentCount} way(s) to call action \"{command.Action}\" "
                               + $"with {command.Value.Length} value(s), but the signatures mismatch.";
            return false;
        }

        public void ProcessCommand()
        {
            Debug.Assert(callbackKey != null && commandSet.ContainsKey(callbackKey));
            commandSet[callbackKey]();
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

        protected void CloseStack()
        {
            stack.RegisterClose();
        }
        protected Context GetContext()
        {
            return stack.GetContext(this);
        }
    }
}