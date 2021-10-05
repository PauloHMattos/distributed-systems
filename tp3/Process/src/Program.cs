namespace TP3.Process
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var process = new Process();
            process.Start();

            while(true)
            {
                process.Update();
            }
        }
    }
}
