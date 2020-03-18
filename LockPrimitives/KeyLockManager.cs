using System;
using System.Collections.Concurrent;
using System.Threading;

namespace LockPrimitives
{
    public class KeyLockItem<T> 
        : PinableObject<T, KeyLockItem<T>>, IDisposable
    {
        private object LockObject = new object();

        private bool _locked = false;

        public KeyLockItem(T key, KeyLockManager<T> keyLockManager)
            : base(key, keyLockManager)
        { }

        public void Lock()
        {
            Monitor.Enter(LockObject);
            _locked = true;
        }

        public void Unlock()
        {
            _locked = false;
            Monitor.Exit(LockObject);
        }

        public void Dispose()
        {
            if(_locked)
                Unlock();

            ReturnToPool();
        }
    }

    public class KeyLockManager<T> : PinableObjecsPool<T, KeyLockItem<T>>
    {
        protected override KeyLockItem<T> NewObject(T key)
        {
            return new KeyLockItem<T>(key, this);
        }
    }
}
