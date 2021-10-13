using System.Threading;

namespace TP3.Process
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var process = new Process(int.Parse(args[0]), int.Parse(args[1]));
            process.Start();

            while(true)
            {
                process.Update();
                // 50 Updates per second
                Thread.Sleep(20);
            }
        }
    }
}
