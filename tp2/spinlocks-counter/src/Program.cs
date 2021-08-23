using System;
using System.Threading;

namespace tp2
{
    public class SpinLock
    {
        private int _locked;

        public SpinLock()
        {
            _locked = 0;
        }

        public void Acquire()
        {
            while (Interlocked.CompareExchange(ref _locked, 1, 0) == 1)
            {
            }
        }

        public void Release()
        {
            _locked = 0;
        }
    }

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

            // TODO: Allow lengths greater than int.MaxValue
            var threads = new Thread[numberOfThreads];
            elements = new char[vectorLength];

            InitializeRandomVector(elements, vectorLength);
            InitializeThreadsVector(threads, vectorLength);

            //Thread.Sleep(5000);
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

            fixed(char* p = &elements[0])
            {
                for (var i = 0; i < length; i++)
                {
                    *(p + i) = (char)1; //(char)rnd.Next(-100, 101);
                }
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
            fixed(char* p = &elements[0])
            {
                for (var i = start; i < end; i++)
                {
                    localSum += *(p + i);
                }
            }

            // Lock
            myLock.Acquire();
            sum += localSum;
            myLock.Release();
        }
    }
}
