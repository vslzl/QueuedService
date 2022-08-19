using Microsoft.AspNetCore.Mvc;
using QueueTester.QueueService;

namespace QueueTester.Controllers;

[Route("api/[controller]")]
public class QueueController : Controller
{
    private readonly ILogger<QueueController> _logger;
    private readonly IBackgroundTaskQueue _taskQueue;
    private readonly CancellationToken _cancellationToken;
    private readonly IdService _idService;

    public QueueController(ILogger<QueueController> logger, IBackgroundTaskQueue taskQueue, IHostApplicationLifetime applicationLifetime, IdService idService)
    {
        _logger = logger;
        _taskQueue = taskQueue;
        _cancellationToken = applicationLifetime.ApplicationStopping;
        _idService = idService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAsync()
    {
        for (int i = 0; i < 12; i++)
            await _taskQueue.QueueBackgroundWorkItemAsync(BuildWorkItemAsync);
        return Ok();
    }


    private async Task BuildWorkItemAsync(CancellationToken token)
    {
        // Simulate three 5-second tasks to complete
        // for each enqueued work item

        int delayLoop = 0;
        var guid = _idService.GetId();

        // _logger.LogInformation("{Guid}: Queued work item is starting.", guid);

        while (!token.IsCancellationRequested && delayLoop < 3)
        {
            try
            {
                await Task.Delay(TimeSpan.FromMilliseconds(1000), token);
            }
            catch (OperationCanceledException)
            {
                // Prevent throwing if the Delay is cancelled
            }

            ++delayLoop;

            // _logger.LogInformation("Queued work item {Guid} is running. {DelayLoop}/3", guid, delayLoop);
        }

        string format = delayLoop switch
        {
            3 => "{Guid}: Queued Background Task is complete.",
            _ => "{Guid}: Queued Background Task was cancelled."
        };

        _logger.LogInformation(format, guid);
    }
}