using System;
using System.Threading;

namespace producer_consumer
{
    class Program
    {
        static int[] buffer;
        static int consumedNumbers = 0;
        static Semaphore mutex;
        static Semaphore empty;
        static Semaphore full;

        static void Main(string[] args)
        {
            int nProducers = Int32.Parse(args[0]);
            int nConsumers = Int32.Parse(args[1]);
            int bufferSize = Int32.Parse(args[2]);

            empty = new Semaphore(bufferSize, bufferSize);
            full = new Semaphore(0, bufferSize);
            mutex = new Semaphore(1, 1);
            buffer = new int[bufferSize];

            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            var producers = new Thread[nProducers];
            InitializeProducers(producers);

            var consumers = new Thread[nConsumers];
            InitializeConsumers(consumers);

            foreach(var t in consumers)
            {
                t.Join();
            }

            watch.Stop();
            Console.WriteLine($"{watch.ElapsedMilliseconds}");

            Environment.Exit(0);
        }

        static void InitializeProducers(Thread[] producers)
        {
            for (var i = 0; i < producers.Length; i++)
            {
                producers[i] = new Thread(StartProducer);
                producers[i].Start();
            }
        }

        static void StartProducer()
        {
            var rng = new Random();
            while (true)
            {
                empty.WaitOne();
                mutex.WaitOne();

                for (var i = 0; i < buffer.Length; i++)
                {
                    if (buffer[i] != 0)
                    {
                        continue;
                    }

                    buffer[i] = (int)rng.Next(1, 10_000_001);
                    break;
                }

                mutex.Release();
                full.Release();
            }
        }


        static void InitializeConsumers(Thread[] consumers)
        {
            for (var i = 0; i < consumers.Length; i++)
            {
                consumers[i] = new Thread(StartConsumer);
                consumers[i].Start();
            }
        }

        static void StartConsumer()
        {
            while (consumedNumbers < 100_000)
            {
                int resource = -1;

                full.WaitOne();
                mutex.WaitOne();

                for (var i = 0; i < buffer.Length; i++)
                {
                    if (buffer[i] == 0)
                    {
                        continue;
                    }

                    resource = buffer[i];
                    buffer[i] = 0;
                    break;
                }

                consumedNumbers++;

                mutex.Release();
                empty.Release();

                // var str = $"{resource} = {IsPrime(resource)}";
                // Console.WriteLine(str);
                // Check prime number
            }
        }

        static bool IsPrime(int n)
        {
            if (n <= 1)
            {
                return false;
            }
            if (n <= 3)
            {
                return true;
            }

            // This is checked so that we can skip
            // middle five numbers in below loop
            if (n % 2 == 0 || n % 3 == 0)
            {
                return false;
            }

            int sqrtN = (int)MathF.Sqrt(n);
            for (int i = 5; i <= sqrtN; i += 6)
            {
                if (n % i == 0 || n % (i + 2) == 0)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
