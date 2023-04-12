using Expenser.Utility;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Xml.Linq;

namespace Expenser.Core
{ 
    /// <summary>
    /// Base class of program's states
    /// </summary>
    public abstract class IState
    {
        private readonly StateStack stack;

        protected delegate void Function();
        private readonly Dictionary<RuleChecker, Function> operations = new();
        private readonly HashSet<string> operationNames = new();
        private RuleChecker? callbackKey = null;

        public IState(StateStack stack)
        {
            this.stack = stack;
        }

        public virtual void Init() { }

        protected abstract void Save();

        // Should be checked before a call to ProcessCommand is made
        // Check if the command matches a possible rule
        public bool ValidateCommand(Command command, out string message)
        {
            if (!operationNames.Contains(command.Action))
            {
                message = $"There is no action called \"{command.Action}\" in current context.";
                return false;
            }

            uint matchArgumentCount = 0;

            foreach (var rule in operations)
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
            Debug.Assert(callbackKey != null && operations.ContainsKey(callbackKey));
            operations[callbackKey]();

            Command cmd = GetContext().CurrentCommand;
            if (cmd.Flags.Count > 0)
            {
                IOStream.OutputOther("Ignored flag(s):", false);
                foreach (var flag in cmd.Flags)
                    IOStream.OutputOther($" {flag}", false);
                IOStream.OutputOther(".");
            }

            Save();
        }

        protected void AddOperation(RuleChecker rule, Function funcion)
        {
            Debug.Assert(!operations.ContainsKey(rule));
            operations[rule] = funcion;
            operationNames.Add(rule.Action);
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