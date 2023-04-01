using Expenser.Core;

namespace Expenser
{
    /// <summary>
    /// This program keeps track of user's incomes
    /// and outcomes by dividing them into categories
    /// </summary>
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                StateStack stack = new();
                while (true)
                    stack.Process();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An exception occured. Here is the detailed description:");
                Console.WriteLine(ex.ToString());
            }
        }
    }
}