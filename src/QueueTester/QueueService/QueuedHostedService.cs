namespace QueueTester.QueueService;

public sealed class QueuedHostedService : BackgroundService
{
    private readonly IBackgroundTaskQueue _taskQueue;
    private readonly ILogger<QueuedHostedService> _logger;
    private readonly IList<Task> _tasks;
    private const int MaximumParallelTasks = 3;

    public QueuedHostedService(
        IBackgroundTaskQueue taskQueue,
        ILogger<QueuedHostedService> logger)
    {
        (_taskQueue, _logger) = (taskQueue, logger);
        _tasks = new List<Task>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            $"{nameof(QueuedHostedService)} is running.{Environment.NewLine}" +
            $"{Environment.NewLine}Tap W to add a work item to the " +
            $"background queue.{Environment.NewLine}");

        await ProcessTaskQueueAsync(stoppingToken);
    }

    private async Task ProcessTaskQueueAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await FillTasksAsync(stoppingToken);
                var finishedTask = await Task.WhenAny(_tasks);
                _tasks.Remove(finishedTask);
            }
            catch (OperationCanceledException)
            {
                // Prevent throwing if stoppingToken was signaled
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred executing task work item.");
            }
        }
    }

    private async Task FillTasksAsync(CancellationToken stoppingToken)
    {
        while (_tasks.Count < MaximumParallelTasks)
        {
            Func<CancellationToken, Task>? workItem = await _taskQueue.DequeueAsync(stoppingToken);
            if (workItem is null) break;
            _tasks.Add(workItem(stoppingToken));
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation(
            $"{nameof(QueuedHostedService)} is stopping.");

        await base.StopAsync(stoppingToken);
    }
}