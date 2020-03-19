using System;
using System.Collections.Concurrent;

namespace LockPrimitives
{
    public abstract class ConcurrentObjecsPool<TKey, TObject>
        where TObject : ConcurrentObjectPoolItem<TKey, TObject>
    {
        private ConcurrentDictionary<TKey, TObject> _keyToLockItems = new ConcurrentDictionary<TKey, TObject>();

        protected abstract TObject NewObject(TKey key);
        public TObject GetLockItem(TKey key)
        {
            while (true)
            {
                var newItem = NewObject(key);

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
        
        public void ReturnLockItem(ConcurrentObjectPoolItem<TKey, TObject> item)
        {
            if (item.Pool != this)
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