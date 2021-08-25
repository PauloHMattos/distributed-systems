using System;
using System.Diagnostics;
using System.Threading;

namespace tp2
{
    unsafe class Program
    {
        static int sum;
        static char[] elements;
        static SpinLock myLock;

        static void Main(string[] args)
        {
            myLock = new SpinLock();
            var numberOfThreads = Int32.Parse(args[0]);
            var vectorLength = Int32.Parse(args[1]);
            elements = new char[vectorLength];

            var sum = 0;
            InitializeRandomVector(elements);
            for (var i = 0; i < 10; i++)
            {
                sum += RunCaseStudy(numberOfThreads);
            }
            Console.Write($"{sum / (float)10}");
        }

        private static int RunCaseStudy(int numberOfThreads)
        {
            var threads = new Thread[numberOfThreads];
            InitializeThreadsVector(threads);

            var watch = new Stopwatch();
            watch.Start();

            // Starts the threads stored in the vector
            foreach(var t in threads)
            {
                t.Start();
            }
            
            // Joins the threads blocking the main thread until
            // all other threads have completed
            foreach(var t in threads)
            {
                t.Join();
            }
            
            watch.Stop();
            
            return watch.Elapsed.Milliseconds;
        }

        static void InitializeRandomVector(char[] vector)
        {
            var rnd = new Random();
            for (var i = 0; i < vector.Length; i++)
            {
                // Random.Next => First Value inclusive, second exclusive
                elements[i] = (char)rnd.Next(-100, 101);
            }
        }

        static void InitializeThreadsVector(Thread[] vector)
        {
            var countPerThread = elements.Length / vector.Length;
            var start = 0;
            var end = countPerThread;
            for (int i = 0; i < vector.Length - 1; i++)
            {
                // Each thread will compute the sum of countPerThread (N/K) elements
                vector[i] = new Thread(() => 
                {
                    ThreadMethod(start, end);
                });

                // The next thread bounds
                start = end;
                end += countPerThread;
            }

            // The last thread will compute the remainder
            vector[^1] = new Thread(() => 
            {
                ThreadMethod(start, elements.Length);
            });
        }

        static void ThreadMethod(int start, int end)
        {
            // Compute the sum inside the bounds
            // No synchronization needed as each thread has it's own bounds
            // And the bounds don't overlap
            var localSum = 0;
            for (var i = start; i < end; i++)
            {
                localSum += elements[i];
            }
            
            // After this segment of the array is computed
            // aquire the lock to add the localSum to the global sum
            myLock.Acquire();
            sum += localSum;

            // After the value is modified, release the lock
            myLock.Release();
        }
    }
}
