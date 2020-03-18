using System;

namespace LockPrimitives
{
    public class KeyLockAsyncItem<T>
        : PinableObject<T, KeyLockAsyncItem<T>>, IDisposable
    {
        public void Dispose()
        {
        }

        public KeyLockAsyncItem(T key, PinableObjecsPool<T, KeyLockAsyncItem<T>> pool) 
            : base(key, pool)
        {
        }
    }
    
    public class KeyLockManagerAsync<T> : PinableObjecsPool<T, KeyLockAsyncItem<T>>
    {
        protected override KeyLockAsyncItem<T> NewObject(T key)
        {
            return new KeyLockAsyncItem<T>(key, this);
        }
    }
}