using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PriorityQueuesChannel.Tests.QueueChannel
{
    public class CollectionTestQueueChannel : PrioritiesQueueChannelBase<CollectionQueueItem>
    {
        private readonly TimeSpan _longPollInterval;

        public Dictionary<string, List<CollectionQueueItemWrapper>> _queues =
            new Dictionary<string, List<CollectionQueueItemWrapper>>();
        
        public Dictionary<string, int> _longPollsByQueue =
            new Dictionary<string, int>();
        
        public CollectionTestQueueChannel(string[] queuesByPriority, TimeSpan longPollInterval) 
            : base(queuesByPriority)
        {
            _longPollInterval = longPollInterval;
            foreach (var queue in queuesByPriority)
            {
                _queues[queue] = new List<CollectionQueueItemWrapper>();
                _longPollsByQueue[queue] = 0;
            }
        }

        public int GetLongPolls(string queue)
        {
            return _longPollsByQueue[queue];
        }
        
        protected override async Task<CollectionQueueItem> LongPollForItem(string queue)
        {
            var items = _queues[queue];
            if (items.Count > 0)
            {
                var firstItem = items[0];
                if (firstItem.TimeToWait > _longPollInterval)
                {
                    firstItem.TimeToWait = firstItem.TimeToWait - _longPollInterval;
                    await Task.Delay(_longPollInterval);
                    _longPollsByQueue[queue]++;
                    return null;
                }
                
                await Task.Delay(firstItem.TimeToWait);
                items.Remove(firstItem);
                _longPollsByQueue[queue]++;
                return firstItem.ContainsResult 
                    ? firstItem.Value 
                    : null;
            }
            else
            {
                await Task.Delay(_longPollInterval);
                _longPollsByQueue[queue]++;
                return null;
            }
        }

        protected override Task PutItemBack(string queue, CollectionQueueItem item)
        {
            var items = _queues[queue];
            var wrapper = new CollectionQueueItemWrapper
            {
                Value = item,
                ContainsResult = true,
                TimeToWait = TimeSpan.FromMilliseconds(100)
            };
            
            items.Insert(0, wrapper);
            return Task.CompletedTask;
        }

        public override Task AckItem(string queue, CollectionQueueItem item)
        {
            return Task.CompletedTask;
        }

        public CollectionTestQueueChannel AddItemWithDelay(string queue, TimeSpan delay, string val)
        {
            _queues[queue].Add(new CollectionQueueItemWrapper()
            {
                ContainsResult = true,
                TimeToWait = delay,
                Value = new CollectionQueueItem()
                {
                    Value = val
                }
            });

            return this;
        }
        
        public CollectionTestQueueChannel AddEmptyResult(string queue)
        {
            _queues[queue].Add(new CollectionQueueItemWrapper()
            {
                ContainsResult = false,
                TimeToWait = _longPollInterval,
            });

            return this;
        }
    }
}