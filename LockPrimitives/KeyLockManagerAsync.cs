using System;

namespace LockPrimitives
{
    public class KeyLockAsyncItem<T>
        : ConcurrentObjectPoolItem<T, KeyLockAsyncItem<T>>, IDisposable
    {
        public void Dispose()
        {
        }

        public KeyLockAsyncItem(T key, ConcurrentObjecsPool<T, KeyLockAsyncItem<T>> pool) 
            : base(key, pool)
        {
        }
    }
    
    public class KeyLockManagerAsync<T> : ConcurrentObjecsPool<T, KeyLockAsyncItem<T>>
    {
        protected override KeyLockAsyncItem<T> NewObject(T key)
        {
            return new KeyLockAsyncItem<T>(key, this);
        }
    }
}