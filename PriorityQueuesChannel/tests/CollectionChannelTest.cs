using System;
using System.Threading;
using System.Threading.Tasks;
using PriorityQueuesChannel.Tests.QueueChannel;
using Xunit;

namespace PriorityQueuesChannel.Tests
{
    public class CollectionChannelTest
    {
        [Fact]
        public async Task CollectionQueueChannel_NewerReturns()
        {
            // Arrange
            var channel = new CollectionTestQueueChannel(new [] { "a", "b" }, TimeSpan.FromSeconds(3));
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
        public async Task CollectionQueueChannel_AddEmptyResults_NeverReturns()
        {
            // Arrange
            var channel = new CollectionTestQueueChannel(new [] { "a", "b" }, TimeSpan.FromSeconds(3));

            channel
                .AddEmptyResult("a")
                .AddEmptyResult("b");
            
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
        public async Task CollectionQueueChannel_WithOneItem_WithingInterval_ReturnsWithDelay()
        {
            // Arrange
            var channel = new CollectionTestQueueChannel(new [] { "a", "b" }, TimeSpan.FromSeconds(3));

            channel
                .AddItemWithDelay("a",TimeSpan.FromSeconds(1), "a_123")
                .AddEmptyResult("b");
            
            var ct = new CancellationTokenSource();
            ct.CancelAfter(TimeSpan.FromSeconds(5));
            await channel.Start(ct.Token);
            
            // Act
            var item = await channel.GetNextItem(TimeSpan.FromSeconds(1));
            ct.Cancel();
            
            // Assert
            Assert.Equal("a_123", item.Value);
            Assert.Equal(1, channel.GetLongPolls("a"));
        }
        
        [Fact]
        public async Task CollectionQueueChannel_WithOneItem_AfterInterval_ReturnsWithDelay()
        {
            // Arrange
            var channel = new CollectionTestQueueChannel(new [] { "a", "b" }, TimeSpan.FromSeconds(3));

            channel
                .AddItemWithDelay("a",TimeSpan.FromSeconds(5), "a_123")
                .AddEmptyResult("b");
            
            var ct = new CancellationTokenSource();
            ct.CancelAfter(TimeSpan.FromSeconds(6));
            await channel.Start(ct.Token);
            
            // Act
            var item = await channel.GetNextItem(TimeSpan.FromSeconds(1));
            ct.Cancel();
            
            // Assert
            Assert.Equal("a_123", item.Value);
            Assert.Equal(2, channel.GetLongPolls("a"));
        }
        
        [Fact]
        public async Task CollectionQueueChannel_WithinWindow_TakenMostPrioritized()
        {
            // Arrange
            var channel = new CollectionTestQueueChannel(new [] { "a", "b" }, TimeSpan.FromSeconds(3));

            channel
                .AddItemWithDelay("a",TimeSpan.FromSeconds(2), "a_123")
                .AddItemWithDelay("b",TimeSpan.FromSeconds(1), "b_123");
            
            var ct = new CancellationTokenSource();
            ct.CancelAfter(TimeSpan.FromSeconds(6));
            await channel.Start(ct.Token);
            
            // Act
            var item = await channel.GetNextItem(TimeSpan.FromSeconds(3));
            ct.Cancel();
            
            // Assert
            Assert.Equal("a_123", item.Value);
        }
        
        [Fact]
        public async Task CollectionQueueChannel_WithinWindow_NotPrioritizedIsAvailable()
        {
            // Arrange
            var channel = new CollectionTestQueueChannel(new [] { "a", "b" }, TimeSpan.FromSeconds(3));

            channel
                .AddItemWithDelay("a",TimeSpan.FromSeconds(2), "a_123")
                .AddItemWithDelay("b",TimeSpan.FromSeconds(1), "b_123");
            
            var ct = new CancellationTokenSource();
            ct.CancelAfter(TimeSpan.FromSeconds(6));
            await channel.Start(ct.Token);
            
            // Act
            var item = await channel.GetNextItem(TimeSpan.FromSeconds(3));
            // Assert
            Assert.Equal("a_123", item.Value);
            
            item = await channel.GetNextItem(TimeSpan.FromSeconds(3));
            Assert.Equal("b_123", item.Value);
        }
        
        [Fact]
        public async Task CollectionQueueChannel_AfterWindow_TakenMostPrioritized()
        {
            // Arrange
            var channel = new CollectionTestQueueChannel(new [] { "a", "b" }, TimeSpan.FromSeconds(3));

            channel
                .AddItemWithDelay("a",TimeSpan.FromSeconds(3), "a_123")
                .AddItemWithDelay("b",TimeSpan.FromSeconds(1), "b_123");
            
            var ct = new CancellationTokenSource();
            ct.CancelAfter(TimeSpan.FromSeconds(6));
            await channel.Start(ct.Token);
            
            // Act
            var item = await channel.GetNextItem(TimeSpan.FromSeconds(2));
            
            // Assert
            Assert.Equal("b_123", item.Value);
        }
        
        [Fact]
        public async Task CollectionQueueChannel_AfterWindow_RestAvailable()
        {
            // Arrange
            var channel = new CollectionTestQueueChannel(new [] { "a", "b" }, TimeSpan.FromSeconds(3));

            channel
                .AddItemWithDelay("a",TimeSpan.FromSeconds(3), "a_123")
                .AddItemWithDelay("b",TimeSpan.FromSeconds(1), "b_123");
            
            var ct = new CancellationTokenSource();
            ct.CancelAfter(TimeSpan.FromSeconds(6));
            await channel.Start(ct.Token);
            
            // Act
            var item = await channel.GetNextItem(TimeSpan.FromSeconds(2));
            
            // Assert
            Assert.Equal("b_123", item.Value);
            
            item = await channel.GetNextItem(TimeSpan.FromSeconds(2));
            Assert.Equal("a_123", item.Value);
        }
    }
}