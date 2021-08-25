using System;
using System.Threading;

namespace producer_consumer
{
    class Program
    {
        static int[] buffer;
        static int consumedNumbers = 0;
        static int finishedThreads = 0;
        static Semaphore mutex;
        static Semaphore mutex2;
        static Semaphore empty;
        static Semaphore full;

        // Method used as an entry point for the program
        static void Main(string[] args)
        {
            // Parsing the CLI arguments to store parameters
            int nProducers = Int32.Parse(args[0]);
            int nConsumers = Int32.Parse(args[1]);
            int bufferSize = Int32.Parse(args[2]);

            // Initializing semaphores as described in the essay report
            empty = new Semaphore(bufferSize, bufferSize);
            full = new Semaphore(0, bufferSize);
            mutex = new Semaphore(1, 1);
            buffer = new int[bufferSize];

            // Intializing a stopwatch to time the execution
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            // Initializing an array of threads for the producers
            var producers = new Thread[nProducers];
            InitializeProducers(producers);

            // Initializing an array of threads for the producers
            var consumers = new Thread[nConsumers];
            InitializeConsumers(consumers);

            // Waits for every consumer thread to finish execution
            foreach(var t in consumers)
            {
                t.Join();
            }

            // Stops the stopwatch and prints the elapsed time
            watch.Stop();
            Console.WriteLine($"{watch.ElapsedMilliseconds}");

            Environment.Exit(0);
        }

        // Method responsible for parsing the producers threads and starting them
        static void InitializeProducers(Thread[] producers)
        {
            for (var i = 0; i < producers.Length; i++)
            {
                producers[i] = new Thread(StartProducer);
                producers[i].Start();
            }
        }

        // Method executed by each producer thread
        static void StartProducer()
        {
            // Initializes random seeder
            var rng = new Random();

            // Produces resources indefinitely
            while (true)
            {
                // Waits for mutex and for a free slot in buffer
                empty.WaitOne();
                mutex.WaitOne();

                // Iterate through buffer elements to find a free position
                for (var i = 0; i < buffer.Length; i++)
                {
                    // Continues iteration if position is occupied
                    if (buffer[i] != 0)
                    {
                        continue;
                    }

                    // Allocates first free position and breaks the loop
                    buffer[i] = (int)rng.Next(1, 10_000_001);
                    break;
                }

                // Releases mutex and signals a new resource available to be read
                mutex.Release();
                full.Release();
            }
        }


        // Method responsible for parsing the consumers threads and starting them
        static void InitializeConsumers(Thread[] consumers)
        {
            for (var i = 0; i < consumers.Length; i++)
            {
                consumers[i] = new Thread(StartConsumer);
                consumers[i].Start();
            }
        }

        // Method executed by each consumer thread
        static void StartConsumer()
        {
            int resource = -1;

            // Execute threads until 10ˆ5 numbers were consumed colectively
            while (consumedNumbers < 100_000)
            {
                // Waits for mutex and for an available resource in buffer
                full.WaitOne();
                mutex.WaitOne();

                // Iterate through buffer elements to find an available resource
                for (var i = 0; i < buffer.Length; i++)
                {
                    // Continues iteration if position is free
                    if (buffer[i] == 0)
                    {
                        continue;
                    }

                    // Fetch first available resource and free its position
                    resource = buffer[i];
                    buffer[i] = 0;
                    break;
                }

                // Compute that a new number was consumed
                consumedNumbers++;

                // Releases mutex and signals a new free position in buffer
                mutex.Release();
                empty.Release();

                // Prints whether read resource is prime or not
                var str = $"{resource} = {IsPrime(resource)}";
                Console.WriteLine(str);
            }
        }

        // Auxiliar method used to check whether a number is prime or not
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
