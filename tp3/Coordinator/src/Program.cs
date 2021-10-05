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

            while(true)
            {
                coordinator.Update();
            }
        }
    }
}
