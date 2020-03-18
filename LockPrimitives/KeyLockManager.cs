using System;
using System.Collections.Concurrent;
using System.Threading;

namespace LockPrimitives
{
    public class KeyLockItem<T> : PinableObject, IDisposable
    {
        public readonly T Key;

        public readonly KeyLockManager<T> KeyLockManager;

        private object LockObject = new object();

        private bool _locked = false;

        public KeyLockItem(T key, KeyLockManager<T> keyLockManager)
        {
            Key = key;
            KeyLockManager = keyLockManager;
        }

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

            KeyLockManager.ReturnLockItem(this);
        }
    }

    public class KeyLockManager<T>
    {
        private ConcurrentDictionary<T, KeyLockItem<T>> _keyToLockItems = new ConcurrentDictionary<T, KeyLockItem<T>>();
        
        public KeyLockItem<T> GetLockItem(T key)
        {
            while (true)
            {
                var newItem = new KeyLockItem<T>(key, this);

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

        public void ReturnLockItem(KeyLockItem<T> item)
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
