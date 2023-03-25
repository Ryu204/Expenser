namespace Expenser
{
    namespace Core
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

            public override bool ValidateCommand(string command, ref string message)
            {
                if (command == "h" || command == "r")
                    return true;
                else
                {
                    message = "i only understand if u type \'h\'";
                    return false;
                }
            }

            public override void ProcessCommand(string command)
            {
                if (command == "h")
                    Console.WriteLine("I am sorry, there is currently no help available.");
                else if (command == "r")
                    SwitchTo("Menu");
            }
        }
    }
}
