using System;
using System.Threading;
using System.Threading.Tasks;

namespace LockPrimitives
{
    public class KeyLockAsyncItem<T>
        : ConcurrentObjectPoolItem<T, KeyLockAsyncItem<T>>, IDisposable
    {
        readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        
        private bool _locked = false;

        public async Task WaitAsync()
        {
            await _semaphore.WaitAsync();
            _locked = true;
        }

        public void Release()
        {
            _locked = false;
            _semaphore.Release(1);
        }
        
        public void Dispose()
        {
            if(_locked)
                Release();
            
            ReturnToPool();
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

        public async Task ProcessInLock(T key, Action action)
        {
            using (var item = GetLockItem(key))
            {
                await item.WaitAsync();
                action();
                item.Release();
            }
        }
    }
}