using System;
using System.Threading;
using System.Threading.Tasks;

namespace PriorityQueuesChannel
{
    public interface IQueueChannel<T>
        where T : class
    {
        void Start(CancellationToken ct);

        Task<T> GetNextItem(TimeSpan windowToStayOpenTime);

        Task AckItem(T item);
    }
}