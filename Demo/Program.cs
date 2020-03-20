using System;
using System.Threading;
using LockPrimitives;

namespace Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            //SyncLockDemo.Run();
            AsyncLockDemo.Run();
            Console.Read();
        }
    }
}
