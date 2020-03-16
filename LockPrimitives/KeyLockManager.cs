using System;
using System.Collections.Concurrent;
using System.Threading;

namespace LockPrimitives
{
    public class KeyLockItem : IDisposable
    {
        public readonly string Key;

        public readonly KeyLockManager KeyLockManager;

        private int RefCounter;

        private object LockObject = new object();

        public KeyLockItem(string key, KeyLockManager keyLockManager)
        {
            Key = key;
            KeyLockManager = keyLockManager;
            RefCounter = 1;
        }

        public void Lock()
        {
            Monitor.Enter(LockObject);
        }

        public void Unlock()
        {
            Monitor.Exit(LockObject);
        }

        // Trying to keep RefCounter to be positive value
        // Once it is equal to 0, the item can be disposed, so we lost the race
        public bool Pin()
        {
            // Spinning while one of the following cases happen
            while (true)
            {
                int refCounter = RefCounter;

                // 1. The item is released by it's previous owner, so it can be deleted from the repository
                if (refCounter == 0)
                {
                    return false;
                }

                // 2. We added our reference to the counter, so while we release it, noone can delete it
                if (Interlocked.CompareExchange(ref RefCounter, refCounter + 1, refCounter) == refCounter)
                {
                    return true;
                }
            }
        }

        public bool Unpin()
        {
            var value = Interlocked.Decrement(ref RefCounter);
            if (value == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Dispose()
        {
            KeyLockManager.ReturnLockItem(this);
        }
    }

    public class KeyLockManager
    {
        private ConcurrentDictionary<string, KeyLockItem> _keyToLockItems = new ConcurrentDictionary<string, KeyLockItem>();

        public KeyLockManager()
        {
        }

        public KeyLockItem GetLockItem(string key)
        {
            while (true)
            {
                var newItem = new KeyLockItem(key, this);

                var item = _keyToLockItems.GetOrAdd(key, newItem);

                // Item was just added. The items are created with refCount = 1, so we don't need to pin it
                if (newItem == item)
                {
                    return item;
                }
                // Item was already in dictionary. Trying to pin it
                else
                {
                    // Pinned successfully other thread will not delete it
                    if (item.Pin())
                    {
                        return item;
                    }

                    // The item was just successfully unpinned. and is about to be deleted
                    // We got to the moment it is still in dictionary, but already unpinned.
                    // Going to the next round
                    continue;
                }
            }
        }

        public void ReturnLockItem(KeyLockItem item)
        {
            if (item.KeyLockManager != this)
            {
                throw new ArgumentException("The item does not belong to current KeyLockManager");
            }

            // Try to unpin item. Set RefCounter to 0
            if (item.Unpin())
            {
                // Here we are free do delete it. Even if some of the threads refer to the item, they would skip it and wait until we delete it
                _keyToLockItems.TryRemove(item.Key, out var item2);
            }
        }
    }
}
