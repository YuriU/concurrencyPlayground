namespace LockPrimitives
{
    public static class KeyLockManagerExtensions
    {
        public static KeyLockItem<T> GetLocked<T>(this KeyLockManager<T> keyLockManager, T key)
        {
            var item = keyLockManager.GetLockItem(key);
            item.Lock();
            return item;
        }
    }
}