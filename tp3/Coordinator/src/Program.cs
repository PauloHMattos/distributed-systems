using System;
using System.Diagnostics;

namespace TP3.Coordinator
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // 128 is the max on TP3.pdf
            var coordinator = new Coordinator(128);
            coordinator.Start();

            
            Console.WriteLine("Waiting for one of the following commands:");
            Console.WriteLine("");
            Console.WriteLine("\t 1: Prints the current requests queue.");
            Console.WriteLine("\t 2: Prints how many times each process has been served.");
            Console.WriteLine("\t 3: Finishes the execution.");
            Console.WriteLine("");

            while(true)
            {
                coordinator.Update();
            }
        }
    }
}
