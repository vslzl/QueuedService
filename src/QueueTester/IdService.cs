namespace QueueTester;

public class IdService
{
    private int _id = 0;

    public int GetId()
    {
        return Interlocked.Increment(ref _id);
    }
}