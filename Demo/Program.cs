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
            }

            using (var item = keyLockManager.GetLockItem("hello"))
            {
                item.Lock();
                Console.WriteLine("Item");
                item.Unlock();
            }
        }

        private static void ThreadProc(object obj)
        {
            while (true)
            {
                Random rnd = new Random(Thread.CurrentThread.ManagedThreadId);

                Thread.Sleep(rnd.Next(100, 3000));

                Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId}: Just before take a lock. Value is {value}");


                var numberToAdd = rnd.Next();

                using (var item = KeyLockManager.GetLockItem("hello"))
                {
                    

                    item.Lock();

                    Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId}:Under the lock. Value is {value}");

                    value += numberToAdd;

                    Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId}:Under the lock. Value is {value}");


                    value += numberToAdd;

                    
                    item.Unlock();
                }
            }
        }
    }
}
