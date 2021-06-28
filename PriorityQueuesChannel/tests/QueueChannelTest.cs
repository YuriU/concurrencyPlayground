using System;
using System.Threading;
using System.Threading.Tasks;
using PriorityQueuesChannel.Tests.QueueChannel;
using Xunit;

namespace PriorityQueuesChannel.Tests
{
    [CollectionDefinition(name:"QueueChannelTest", DisableParallelization = true)]
    public class QueueChannelTest
    {
        [Fact]
        public async Task TestQueueChannel_NewerReturns()
        {
            // Arrange
            var channel = new SimpleTestQueueChannel(new [] { "a", "b" }, TimeSpan.FromSeconds(1));
            var ct = new CancellationTokenSource();
            ct.CancelAfter(TimeSpan.FromSeconds(5));
            await channel.Start(ct.Token);
            
            // Act
            var item = await channel.GetNextItem(TimeSpan.FromSeconds(1));
            ct.Cancel();
            
            // Assert
            Assert.Null(item);
        }
        
        [Fact]
        public async Task TestQueueChannel_SetResult_ReturnsIt()
        {
            // Arrange
            var channel = new SimpleTestQueueChannel(new [] { "a", "b" }, TimeSpan.FromSeconds(3));

            var ct = new CancellationTokenSource();
            ct.CancelAfter(TimeSpan.FromSeconds(10));
            await channel.Start(ct.Token);
            
            // Act
            const string valueToSet = "hello_b";
            
            Task.Factory.StartNew(async () =>
            {
                await Task.Delay(TimeSpan.FromSeconds(2));
                channel.AddValue("b", valueToSet);
            }, TaskCreationOptions.LongRunning);
            
            var item = await channel.GetNextItem(TimeSpan.FromSeconds(1));
            ct.Cancel();
            
            // Assert
            Assert.Equal(valueToSet, item);
        }
        
        [Fact]
        public async Task TestQueueChannel_ResultFromPrevIteration_ReturnsIt()
        {
            // Arrange
            var channel = new SimpleTestQueueChannel(new [] { "a", "b" }, TimeSpan.FromSeconds(3));
           
            var ct = new CancellationTokenSource();
            ct.CancelAfter(TimeSpan.FromSeconds(5));
            await channel.Start(ct.Token);
            
            const string valueToSet = "hello_b";
            channel.AddValue("b", valueToSet);
            
            // Act
            var item = await channel.GetNextItem(TimeSpan.FromSeconds(1));
            ct.Cancel();
            
            // Assert
            Assert.Equal(valueToSet, item);
        }
        
        [Fact]
        public async Task TestQueueChannel_SeveralResults_ReturnsMostPrioritized()
        {
            // Arrange
            var channel = new SimpleTestQueueChannel(new [] { "a", "b" }, TimeSpan.FromSeconds(3));

            var ct = new CancellationTokenSource();
            
            ct.CancelAfter(TimeSpan.FromSeconds(5));
            await channel.Start(ct.Token);

            const string valueToSet_A = "hello_a";
            const string valueToSet_B = "hello_b";
            
            channel.AddValue("b", valueToSet_B);
            channel.AddValue("a", valueToSet_A);
            
            // Act
            var item = await channel.GetNextItem(TimeSpan.FromSeconds(1));
            ct.Cancel();
            
            // Assert
            Assert.Equal(valueToSet_A, item);
            Assert.Single(channel.PutBackItems["b"]);
        }
        
        [Fact]
        public async Task TestQueueChannel_SeveralResults_PrioritizedSetLaterButWithinWindow_ReturnsMostPrioritized()
        {
            // Arrange

            TimeSpan initialDelay = TimeSpan.FromSeconds(3); // Time during which the window will be opened, no matter if some tasks are ready
            
            var channel = new SimpleTestQueueChannel(new [] { "a", "b" }, TimeSpan.FromSeconds(4));
            var ct = new CancellationTokenSource();
            
            ct.CancelAfter(TimeSpan.FromSeconds(50));
            await channel.Start(ct.Token);

            const string valueToSet_A = "hello_a";
            const string valueToSet_B = "hello_b";
            
            channel.AddValue("b", valueToSet_B);
            Task.Factory.StartNew(async () =>
            {
                // Waiting for some time less than initial delay
                await Task.Delay(TimeSpan.FromSeconds(2));
                channel.AddValue("a", valueToSet_A);
            });

            // Act
            var item = await channel.GetNextItem(initialDelay);
            ct.Cancel();
            
            // Assert
            Assert.Equal(valueToSet_A, item);
            Assert.Single(channel.PutBackItems["b"]);
        }
    }
}