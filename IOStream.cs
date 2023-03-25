using System;

namespace Expenser
{
    public class IOStream
    {
        bool good = false;

        public bool Good
        {
            get { return good; }
        }

        public void Output(string message)
        {
            Console.WriteLine(message);
        }

        public string Input()
        {
            string? res = Console.ReadLine();
            if (res == null)
            {
                good = false;
                return string.Empty;
            }
            
            HashSet<char> whitespaces = new();
            foreach (char c in res) 
                if (char.IsWhiteSpace(c))
                    whitespaces.Add(c);

            if (res.Split(whitespaces.ToArray(), StringSplitOptions.RemoveEmptyEntries).Length == 0)
            {
                good = false;
                return string.Empty;
            }

            good = true;
            return res.Trim();
        }
    }
}
