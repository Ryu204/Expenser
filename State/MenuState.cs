using Expenser.Core;

namespace Expenser.State
{
    public class MenuState : IState
    {
        public MenuState(StateStack stack)
            : base(stack)
        { }

        public override void Init()
        {
            Console.WriteLine("Menu initialized.");
        }

        public override bool ValidateCommand(Command command, ref string message)
        {
            if (command.Action == "help" || command.Action == "restart")
                return true;
            else
            {
                message = "No action called " + command.Action + " in current context";
                return false;
            }
        }

        public override void ProcessCommand(Command command)
        {
            if (command.Action == "help")
            {
                if (command.Flags.Contains("other"))
                {
                    Console.Write("(-v-)//\t");
                    foreach (string val in command.Value)
                        Console.Write($"{ val } \t");
                    Console.WriteLine();
                }
                else
                    Console.WriteLine("I am sorry, there is currently no help available.");
            }
            else if (command.Action == "restart")
                SwitchTo("Menu");
        }
    }
}
