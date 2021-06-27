using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PriorityQueuesChannel
{
    
    public abstract class PrioritiesQueueChannelBase<T> : IQueueChannel<T>
        where T : class
    {
        private readonly QueryNewTaskWindow<T> _queryWindow;

        private readonly string[] _queuesByPriority;

        protected PrioritiesQueueChannelBase(string[] queuesByPriority)
        {
            _queuesByPriority = queuesByPriority;
            _queryWindow = new QueryNewTaskWindow<T>(
                _queuesByPriority,
                (task, action) =>  PutItemBack(task)
            );
        }

        public void Start(CancellationToken ct)
        {
            foreach (var queue in _queuesByPriority)
            {
                Task.Factory.StartNew(() => PollQueueTask(queue, ct), TaskCreationOptions.LongRunning);
            }
        }

        public async Task<T> GetNextItem(TimeSpan windowToStayOpenTime)
        {
            var (action, task) = await _queryWindow.AwaitCompletion(windowToStayOpenTime);
            return task;
        }
        
        private async Task PollQueueTask(string action, CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                await _queryWindow.NextEventTask;
                
                while (!ct.IsCancellationRequested)
                {
                    var result = await LongPollForItem(action);
                    if (result != null)
                    {
                        await _queryWindow.TrySaveResult(action, result);
                        break;
                    }
                    else
                    {
                        if (!_queryWindow.IsOpened)
                            break;
                    }
                }    
            }

            _queryWindow.SetCanceled(action);
        }

        protected abstract Task<T> LongPollForItem(string queue);

        protected abstract Task PutItemBack(T item);
        
        public abstract Task AckItem(T item);
    }
}