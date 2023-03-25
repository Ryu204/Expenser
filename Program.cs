using Expenser.Core;

namespace Expenser
{
    class Program
    {
        static void Main(string[] args)
        {
            StateStack stack = new StateStack();
            while (true)
            {
                stack.Process();
            }
        }
    }
}