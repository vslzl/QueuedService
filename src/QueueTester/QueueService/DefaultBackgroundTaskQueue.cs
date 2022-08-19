using System.Threading.Channels;

namespace QueueTester.QueueService;

public class DefaultBackgroundTaskQueue : IBackgroundTaskQueue
{
    private readonly Channel<Func<CancellationToken, Task>> _queue;

    public DefaultBackgroundTaskQueue(int capacity)
    {
        // BoundedChannelOptions options = new BoundedChannelOptions(capacity)
        // {
        //     FullMode = BoundedChannelFullMode.Wait,
        //     AllowSynchronousContinuations = false,
        // };
        // _queue = Channel.CreateBounded<Func<CancellationToken, ValueTask>>(options);

        BoundedChannelOptions options = new(capacity)
        {
            FullMode = BoundedChannelFullMode.Wait
        };

        _queue = Channel.CreateBounded<Func<CancellationToken, Task>>(options);
    }

    public async ValueTask QueueBackgroundWorkItemAsync(
        Func<CancellationToken, Task> workItem)
    {
        if (workItem is null)
        {
            throw new ArgumentNullException(nameof(workItem));
        }

        await _queue.Writer.WriteAsync(workItem);
    }

    public async ValueTask<Func<CancellationToken, Task>> DequeueAsync(
        CancellationToken cancellationToken)
    {
        Func<CancellationToken, Task>? workItem =
            await _queue.Reader.ReadAsync(cancellationToken);

        return workItem;
    }
}