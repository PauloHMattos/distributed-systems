using System;
using System.Threading;

namespace tp2
{
    unsafe class Program
    {
        static volatile int sum;
        static char[] elements;
        static SpinLock myLock;

        static void Main(string[] args)
        {
            myLock = new SpinLock();
            var numberOfThreads = 32; //Int32.Parse(args[0]);
            long vectorLength = 1_000_000_000; //Int32.Parse(args[1]);

            var threads = new Thread[numberOfThreads];
            elements = new char[vectorLength];

            InitializeRandomVector(elements, vectorLength);
            InitializeThreadsVector(threads, vectorLength);

            foreach(var t in threads)
            {
                t.Join();
            }

            Console.WriteLine("Hello World!");
            Console.WriteLine($"Resultado = {sum}");
        }

        static void InitializeRandomVector(char[] vector, long length)
        {
            var rnd = new Random();

            for (var i = 0; i < length; i++)
            {
                elements[i] = (char)1; //(char)rnd.Next(-100, 101);
            }
        }

        static void InitializeThreadsVector(Thread[] vector, long numberOfElements)
        {
            var countPerThread = numberOfElements / vector.Length;
            var start = 0L;
            var end = countPerThread;
            for (int i = 0; i < vector.Length - 1; i++)
            {
                vector[i] = new Thread(() => 
                {
                    ThreadMethod(start, end);
                });
                vector[i].IsBackground = true;

                start = end;
                end += countPerThread;
            }
            vector[^1] = new Thread(() => 
            {
                ThreadMethod(start, numberOfElements);
            });
            vector[^1].IsBackground = true;

            for (int i = 0; i < vector.Length; i++)
            {
                vector[i].Start();
            }
        }

        static void ThreadMethod(long start, long end)
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
