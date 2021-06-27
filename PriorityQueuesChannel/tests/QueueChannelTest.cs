using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace PriorityQueuesChannel.Tests
{
    [CollectionDefinition(name:"QueueChannelTest", DisableParallelization = true)]
    public class QueueChannelTest
    {
        [Fact]
        public async Task TestQueueChannel_NewerReturns()
        {
            var channel = new TestQueueChannel(new []{ "a", "b" }, TimeSpan.FromSeconds(1));

            var ct = new CancellationTokenSource();
            
            ct.CancelAfter(TimeSpan.FromSeconds(5));
            channel.Start(ct.Token);
                
            var item = await channel.GetNextItem(TimeSpan.FromSeconds(1));
            
            Assert.Null(item);
        }
        
        [Fact]
        public async Task TestQueueChannel_SetResult_ReturnsIt()
        {
            var channel = new TestQueueChannel(new [] { "a", "b" }, TimeSpan.FromSeconds(3));

            var ct = new CancellationTokenSource();
            
            ct.CancelAfter(TimeSpan.FromSeconds(5));
            channel.Start(ct.Token);

            const string valueToSet = "hello_b";
            Task.Factory.StartNew(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(2));
                channel.AddValue("b", valueToSet);
            });
            
            var item = await channel.GetNextItem(TimeSpan.FromSeconds(1));
            
            Assert.Equal(valueToSet, item);
        }
        
        [Fact]
        public async Task TestQueueChannel_ResultFromPrevIteration_ReturnsIt()
        {
            var channel = new TestQueueChannel(new []{ "a", "b" }, TimeSpan.FromSeconds(3));

            var ct = new CancellationTokenSource();
            
            ct.CancelAfter(TimeSpan.FromSeconds(5));
            channel.Start(ct.Token);

            const string valueToSet = "hello_b";
            channel.AddValue("b", valueToSet);
            
            var item = await channel.GetNextItem(TimeSpan.FromSeconds(1));
            
            Assert.Equal(valueToSet, item);
        }
        
        [Fact]
        public async Task TestQueueChannel_SeveralResults_ReturnsMostPrioritized()
        {
            var channel = new TestQueueChannel(new []{ "a", "b" }, TimeSpan.FromSeconds(3));

            var ct = new CancellationTokenSource();
            
            ct.CancelAfter(TimeSpan.FromSeconds(5));
            channel.Start(ct.Token);

            const string valueToSet_A = "hello_a";
            const string valueToSet_B = "hello_b";
            
            channel.AddValue("b", valueToSet_B);
            channel.AddValue("a", valueToSet_A);
            
            var item = await channel.GetNextItem(TimeSpan.FromSeconds(1));
            
            Assert.Equal(valueToSet_A, item);
        }
    }
}