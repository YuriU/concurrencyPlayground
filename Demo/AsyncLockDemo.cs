using System;
using System.Threading;
using System.Threading.Tasks;
using LockPrimitives;
using Microsoft.VisualBasic;

namespace Demo
{
    public class AsyncLockDemo
    {
        // Shared states
        public static int[] states = new[] { 0, 0, 0 };
        
        private static KeyLockManagerAsync<int> KeyLockManager = new KeyLockManagerAsync<int>();
        
        public static void Run()
        {
            for (int i = 0; i < 10; i++)
            {
                Task.Factory.StartNew(() => TaskProc2());
            }
        }

        private static async Task TaskProc1()
        {
            var index = Thread.CurrentThread.ManagedThreadId % states.Length;
            while (true)
            {
                Random rnd = new Random(Thread.CurrentThread.ManagedThreadId);

                await Task.Delay(rnd.Next(100, 3000));
                
                Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId}: Just before take a lock. Value is {states[index]}");
                
                var numberToAdd = rnd.Next(0, 1000);

                using (var item = KeyLockManager.GetLockItem(index))
                {
                    await item.WaitAsync();
                    
                    Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId}:Under the lock. BC Value is {states[index]}");

                    states[index] += numberToAdd;

                    Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId}:Under the lock. Changed Value is {states[index]}");

                    states[index] -= numberToAdd;
                    
                    item.Release();
                }
            }
        }
        
        private static async Task TaskProc2()
        {
            var index = Thread.CurrentThread.ManagedThreadId % states.Length;
            while (true)
            {
                Random rnd = new Random(Thread.CurrentThread.ManagedThreadId);

                await Task.Delay(rnd.Next(100, 3000));

                Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId}: Just before take a lock. Value {index} is {states[index]}");
                
                var numberToAdd = rnd.Next(0, 1000);

                await KeyLockManager.ProcessInLock(32, () =>
                {
                    Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId}:Under the lock. BC Value {index} is {states[index]}");

                    states[index] += numberToAdd;

                    Console.WriteLine($"Thread {Thread.CurrentThread.ManagedThreadId}:Under the lock. Changed Value {index} is {states[index]}");

                    states[index] -= numberToAdd;
                });
            }
        }
    }
}