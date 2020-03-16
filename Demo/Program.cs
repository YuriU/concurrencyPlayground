using System;
using System.Threading;
using LockPrimitives;

namespace Demo
{
    class Program
    {
        private static KeyLockManager KeyLockManager = new KeyLockManager();

        // The value protected by keylock manager
        public static int value = 0;

        static void Main(string[] args)
        {
            
            Thread[] threads = new Thread[10];
            for (int i = 0; i < 10; i++)
            {
                threads[0] = new Thread(ThreadProc);
                threads[0].Start();
            }

            Console.Read();
        }

        private static void ThreadProc(object obj)
        {
            while (true)
            {
                Random rnd = new Random(Thread.CurrentThread.ManagedThreadId);

                Thread.Sleep(rnd.Next(100, 3000));

                Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId}: Just before take a lock. Value is {value}");


                var numberToAdd = rnd.Next(0, 1000);

                using (var item = KeyLockManager.GetLockItem("hello"))
                {
                    

                    item.Lock();

                    Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId}:Under the lock. BC Value is {value}");

                    value += numberToAdd;

                    Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId}:Under the lock. Changed Value is {value}");


                    value -= numberToAdd;

                    
                    item.Unlock();
                }
            }
        }
    }
}
