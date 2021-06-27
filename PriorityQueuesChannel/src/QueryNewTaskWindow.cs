using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PriorityQueuesChannel
{
    public class QueryNewTaskWindow<TResult>
        where TResult : class
    {
        // If window is open it is expected to store message inside
        private volatile bool _isOpened;

        public bool IsOpened => _isOpened;

        private readonly Dictionary<string, TaskCompletionSource<TResult>> _awaitingTasks;

        private readonly string[] _priorityOrder;
        
        // Action for return rest of the actions
        private readonly Func<TResult, string, Task> _returnAction;

        private TaskCompletionSource<bool> _nextWindowEvent = new TaskCompletionSource<bool>();

        private TaskCompletionSource<bool> _thisWindowEvent = new TaskCompletionSource<bool>();

        public Task NextEventTask => _nextWindowEvent.Task;

        public QueryNewTaskWindow(string[] actions, Func<TResult, string, Task> returnAction)
        {
            _priorityOrder = actions;
            _returnAction = returnAction;
            _awaitingTasks = new Dictionary<string, TaskCompletionSource<TResult>>();
            foreach (var action in actions)
            {
                var taskCompletionSource = new TaskCompletionSource<TResult>();
                _awaitingTasks[action] = taskCompletionSource;
            }
        }
        
        public async Task<bool> TrySaveResult(string action, TResult result)
        {
            if (!_isOpened)
            {
                await _returnAction(result, action);
                return false;
            }
            
            _awaitingTasks[action].SetResult(result);
            return true;
        }

        public void SetCanceled(string action)
        {
            if(!_awaitingTasks[action].Task.IsCompleted)
                _awaitingTasks[action].SetCanceled();
        }

        public async Task<(string, TResult)> AwaitCompletion(TimeSpan initialDelay)
        {
            // Opening window
            _isOpened = true;
            _thisWindowEvent = _nextWindowEvent;
            _nextWindowEvent = new TaskCompletionSource<bool>();
            _thisWindowEvent.SetResult(true);
            
            List<Task> tasks = _priorityOrder
                .Select(queue => _awaitingTasks[queue].Task)
                .Cast<Task>().ToList();

            // Initial delay needed to let new tasks to reach Queue over network
            await Task.Delay(initialDelay);
            
            // Waiting any of tasks 
            await Task.WhenAny(tasks);

            // Closing window
            _isOpened = false;
            
            var (queue, toBeReturned) = GetMostPrioritizedCompleted();
            
            // Returning rest items back
            await ReturnRestItemsBack(queue);

            if (toBeReturned.Task.Status == TaskStatus.Canceled)
            {
                return (queue, null);
            }
            
            return (queue, toBeReturned.Task.Result);
        }

        private async Task ReturnRestItemsBack(string queueItemToBeReturned)
        {
            _isOpened = false;
            foreach (var queue in _awaitingTasks.Keys.ToList())
            {
                if (_awaitingTasks[queue].Task.IsCompleted)
                {
                    if (queue != queueItemToBeReturned)
                    {
                        if(!_awaitingTasks[queue].Task.IsCanceled)
                            await _returnAction(_awaitingTasks[queue].Task.Result, queue);                        
                    }
                    
                    _awaitingTasks[queue] = new TaskCompletionSource<TResult>();
                }
            }
        }

        private (string, TaskCompletionSource<TResult>) GetMostPrioritizedCompleted()
        {
            foreach (var action in _priorityOrder)
            {
                if (_awaitingTasks[action].Task.IsCompleted)
                {
                    return (action, _awaitingTasks[action]);
                }
            }

            throw new Exception("Invalid state of the window");
        }
    }
}