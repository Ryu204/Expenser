using System.ComponentModel;
using System.Diagnostics;

namespace Expenser.Core
{
    /// <summary>
    /// A rule is in the form "This action takes n 
    /// argument types: X1,X2,...,Xn respectively"
    /// </summary>
    public class RuleChecker
    {
        public string Action { get; }
        private Type[] Arguments { get; }

        public RuleChecker(string action, Type[] types)
        {
            Debug.Assert(CommandParser.IsAction(action));
            Action = action;
            Arguments = types;
        }

        // Check if a command matches this rule
        public bool Check(Command command)
        {
            if (command.Action != Action || command.Value.Length != Arguments.Length)
                return false;
            for (int i = 0; i < Arguments.Length; ++i)
            {
                TypeConverter converter = TypeDescriptor.GetConverter(Arguments[i]);

                if (!converter.IsValid(command.Value[i]))
                    return false;
            }    
            return true;
        }
    }
}
