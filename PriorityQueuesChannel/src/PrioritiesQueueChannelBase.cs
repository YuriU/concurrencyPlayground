﻿using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace PriorityQueuesChannel
{
    
    public abstract class PrioritiesQueueChannelBase<T> : IQueueChannel<T>
        where T : class
    {
        private readonly QueryTimeWindow<T> _queryWindow;

        private readonly string[] _queuesByPriority;

        protected PrioritiesQueueChannelBase(string[] queuesByPriority)
        {
            _queuesByPriority = queuesByPriority;
            _queryWindow = new QueryTimeWindow<T>(
                _queuesByPriority,
                PutItemBack
            );
        }

        public async Task Start(CancellationToken ct)
        {
            List<Task> pollersReady = new List<Task>();
            foreach (var queue in _queuesByPriority)
            {
                var whenReady = new TaskCompletionSource<bool>();
                Task.Factory.StartNew(() => PollQueueTask(queue, whenReady, ct), TaskCreationOptions.LongRunning);
                pollersReady.Add(whenReady.Task);
            }

            await Task.WhenAll(pollersReady);
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
                whenReady.CompleteIfNeed(true);
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

        protected abstract Task PutItemBack(string queue, T item);
        
        public abstract Task AckItem(string queue, T item);
    }
}