using System.Threading.Tasks;

namespace PriorityQueuesChannel
{
    public static class TaskExtensions
    {
        public static void CompleteIfNeed<T>(this TaskCompletionSource<T> tcs, T val)
        {
            if (!tcs.Task.IsCompleted)
            {
                tcs.SetResult(val);
            }
        }
    }
}