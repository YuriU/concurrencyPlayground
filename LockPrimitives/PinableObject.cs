using System;
using System.Threading;

namespace LockPrimitives
{
    public class PinableObject<TKey, TObject>
        where TObject : PinableObject<TKey, TObject>
    {
        public readonly TKey Key;
        
        public readonly PinableObjecsPool<TKey, TObject> Pool;

        private int _refCounter;
        
        public PinableObject(TKey key, PinableObjecsPool<TKey, TObject> pool)
        {
            Key = key;
            Pool = pool;
            _refCounter = 1;
        }
        
        // Trying to keep RefCounter to be positive value
        // Once it is equal to 0, the item can be disposed, so we lost the race
        public bool Pin()
        {
            // Spinning while one of the following cases happen
            while (true)
            {
                int refCounter = _refCounter;

                // 1. The item is released by it's previous owner, so it can be deleted from the repository
                if (refCounter == 0)
                {
                    return false;
                }

                // 2. We added our reference to the counter, so while we release it, noone can delete it
                if (Interlocked.CompareExchange(ref _refCounter, refCounter + 1, refCounter) == refCounter)
                {
                    return true;
                }
            }
        }

        public bool Unpin()
        {
            var value = Interlocked.Decrement(ref _refCounter);
            if (value == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        protected void ReturnToPool()
        {
            Pool.ReturnLockItem(this);
        }
    }
}