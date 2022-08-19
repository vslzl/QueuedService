namespace QueueTester;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;

    public Worker(ILogger<Worker> logger)
    {
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
       await Task.CompletedTask;
    }

    private void LogFault(Guid result)
    {
        _logger.LogInformation("Queued work item {Guid} HAS BEEN FINISHED", result);
    }

    private void LogFinished(Guid result)
    {
        _logger.LogInformation("Queued work item {Guid} HAS BEEN FINISHED", result);
    }

    private async Task<Guid> BuildWorkItemAsync(CancellationToken token)
    {
        // Simulate three 5-second tasks to complete
        // for each enqueued work item

        int delayLoop = 0;
        var guid = Guid.NewGuid();

        _logger.LogInformation("Queued work item {Guid} is starting.", guid);

        while (!token.IsCancellationRequested && delayLoop < 3)
        {
            try
            {
                await Task.Delay(TimeSpan.FromMilliseconds(200), token);
            }
            catch (OperationCanceledException)
            {
                // Prevent throwing if the Delay is cancelled
            }

            ++delayLoop;

            _logger.LogInformation("Queued work item {Guid} is running. {DelayLoop}/3", guid, delayLoop);
        }

        string format = delayLoop switch
        {
            3 => "Queued Background Task {Guid} is complete.",
            _ => "Queued Background Task {Guid} was cancelled."
        };

        _logger.LogInformation(format, guid);
        
        return guid;
    }
}
