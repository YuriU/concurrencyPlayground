using System;
using System.Threading;
using LockPrimitives;

namespace Demo
{
    public class SyncLockDemo
    {
        // The value protected by keylock manager
        public static int value = 0;
        
        private static KeyLockManager<int> KeyLockManager = new KeyLockManager<int>();
        
        public static void Run()
        {
            Thread[] threads = new Thread[10];
            for (int i = 0; i < 10; i++)
            {
                threads[0] = new Thread(ThreadProc);
                threads[0].Start();
            }
        }
        
        private static void ThreadProc(object obj)
        {
            while (true)
            {
                Random rnd = new Random(Thread.CurrentThread.ManagedThreadId);

                Thread.Sleep(rnd.Next(100, 3000));

                Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId}: Just before take a lock. Value is {value}");
                
                var numberToAdd = rnd.Next(0, 1000);

                using (var item = KeyLockManager.GetLocked(88))
                {
                    Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId}:Under the lock. BC Value is {value}");

                    value += numberToAdd;

                    Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId}:Under the lock. Changed Value is {value}");

                    value -= numberToAdd;
                }
            }
        }
    }
}