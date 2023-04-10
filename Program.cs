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
                {
                    if (stack.Empty)
                        break;
                    stack.Process();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("An exception occured:");
                Console.WriteLine(ex.Message);
            }
        }
    }
}