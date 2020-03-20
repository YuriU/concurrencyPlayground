using System;
using System.Threading;
using System.Threading.Tasks;
using LockPrimitives;

namespace Demo
{
    public class AsyncLockDemo
    {
        // The value protected by keylock manager
        public static int value = 0;
        
        private static KeyLockManagerAsync<int> KeyLockManager = new KeyLockManagerAsync<int>();
        
        public static void Run()
        {
            for (int i = 0; i < 10; i++)
            {
                Task.Factory.StartNew(() => TaskProc());
            }
        }

        private static async Task TaskProc()
        {
            while (true)
            {
                Random rnd = new Random(Thread.CurrentThread.ManagedThreadId);

                Thread.Sleep(rnd.Next(100, 3000));

                Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId}: Just before take a lock. Value is {value}");
                
                var numberToAdd = rnd.Next(0, 1000);

                using (var item = KeyLockManager.GetLockItem(32))
                {
                    await item.WaitAsync();
                    
                    Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId}:Under the lock. BC Value is {value}");

                    value += numberToAdd;

                    Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId}:Under the lock. Changed Value is {value}");

                    value -= numberToAdd;
                    
                    item.Release();
                }
            }
        }
        
    }
}