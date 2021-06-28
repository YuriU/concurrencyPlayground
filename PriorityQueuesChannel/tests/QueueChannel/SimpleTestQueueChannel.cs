using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PriorityQueuesChannel.Tests
{
    public class SimpleTestQueueChannel : PrioritiesQueueChannelBase<string>
    {
        private readonly TimeSpan _longPollInterval;

        private Dictionary<string, TaskCompletionSource<string>> _waiters =
            new Dictionary<string, TaskCompletionSource<string>>();

        public Dictionary<string, List<string>> PutBackItems = new Dictionary<string, List<string>>(); 

        public SimpleTestQueueChannel(string[] queuesByPriority, TimeSpan longPollInterval) 
            : base(queuesByPriority)
        {
            _longPollInterval = longPollInterval;
            foreach (var queue in queuesByPriority)
            {
                _waiters[queue] = new TaskCompletionSource<string>();
            }

            foreach (var queue in queuesByPriority)
            {
                PutBackItems[queue] = new List<string>();
            }
        }

        public void AddValue(string queue, string val)
        {
            _waiters[queue].SetResult(val);
        }
        
        protected override async Task<string> LongPollForItem(string queue)
        {
            var waiter = _waiters[queue];
            var task = await Task.WhenAny(waiter.Task, Task.Delay(_longPollInterval));
            
            if (task == waiter.Task)
            {
                return waiter.Task.Result;
            }
            
            return null;
        }

        protected override Task PutItemBack(string queue, string item)
        {
            PutBackItems[queue].Add(item);
            return Task.CompletedTask;
        }

        public override Task AckItem(string queue, string item)
        {
            return Task.CompletedTask;
        }
    }
}