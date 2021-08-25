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
            foreach(var t in threads)
            {
                t.Start();
            }
            
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
                vector[i] = new Thread(() => 
                {
                    ThreadMethod(start, end);
                });

                start = end;
                end += countPerThread;
            }
            vector[^1] = new Thread(() => 
            {
                ThreadMethod(start, elements.Length);
            });
        }

        static void ThreadMethod(int start, int end)
        {
            var localSum = 0; //sum;
            for (var i = start; i < end; i++)
            {
                localSum += elements[i];
            }
            
            // Lock
            myLock.Acquire();
            sum += localSum;
            myLock.Release();
        }
    }
}
