using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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

        public async Task Start(CancellationToken ct)
        {
            List<Task> listerersReadyTasks = new List<Task>();
            foreach (var queue in _queuesByPriority)
            {
                var whenReady = new TaskCompletionSource<bool>();
                Task.Factory.StartNew(() => PollQueueTask(queue, whenReady, ct), TaskCreationOptions.LongRunning);
                listerersReadyTasks.Add(whenReady.Task);
            }

            await Task.WhenAll(listerersReadyTasks);
        }

        public async Task<T> GetNextItem(TimeSpan windowToStayOpenTime)
        {
            var (action, task) = await _queryWindow.AwaitCompletion(windowToStayOpenTime);
            return task;
        }
        
        private async Task PollQueueTask(string queue, TaskCompletionSource<bool> whenReady, CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                whenReady.SetResult(true);
                await _queryWindow.NextEventTask;
                
                while (!ct.IsCancellationRequested)
                {
                    var result = await LongPollForItem(queue);
                    if (result != null)
                    {
                        await _queryWindow.TrySaveResult(queue, result);
                        break;
                    }
                    else
                    {
                        if (!_queryWindow.IsOpened)
                            break;
                    }
                }    
            }

            _queryWindow.SetCanceled(queue);
        }

        protected abstract Task<T> LongPollForItem(string queue);

        protected abstract Task PutItemBack(T item);
        
        public abstract Task AckItem(T item);
    }
}