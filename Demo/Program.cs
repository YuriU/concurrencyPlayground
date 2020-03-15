using System;
using LockPrimitives;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var keyLockManager = new KeyLockManager();


            using (var item = keyLockManager.GetLockItem("hello"))
            {
                
                Console.WriteLine("Item");
                
            }

        }
    }
}
