namespace QueueTester.Models;

public class QueueJobItem<TParameter, TResult>
{
    private DateTime CreatedAt { get; set; }
    private DateTime? QueuedAt { get; set; }
    private DateTime? StartedAt { get; set; }
    private DateTime? EndedAt { get; set; }
    public Exception? Exception { get; set; }
    public int JobId { get; }
    private Func<TParameter, CancellationToken, Task<TResult>> Function { get; set; } = null!;

    public QueueJobItem(int jobId, Func<TParameter, CancellationToken, Task<TResult>> function)
    {
        JobId = jobId;
        Function = function;
        CreatedAt = DateTime.Now;
    }

    public async Task<TResult?> ExecuteAsync(TParameter arg1, CancellationToken arg2)
    {
        TResult? result = default(TResult);
        try
        {
            StartedAt = DateTime.Now;
            result = await Function(arg1, arg2);
        }
        catch(OperationCanceledException)
        {
            // Ignored
        }
        catch (Exception e)
        {
            Exception = e;
        }
        finally
        {
            EndedAt = DateTime.Now;
        }

        return result;
    }

}