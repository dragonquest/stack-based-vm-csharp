using System;

namespace VM
{
    public class Monitor : IMonitor
    {
        public bool InsertNewLineBefore = false;

        public void Display(string msg)
        {
            if (InsertNewLineBefore)
            {
                Console.WriteLine();
            }

            Console.Write("{0}", msg);
        }
    }
}
