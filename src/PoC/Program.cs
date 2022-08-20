// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");


Func<string, CancellationToken, Task<string>> a = GetStringAsync;

var result = await a("test", CancellationToken.None);

Console.WriteLine(result);

async Task<string> GetStringAsync(string someString, CancellationToken ct)
{
    await Task.CompletedTask;
    return someString.ToUpperInvariant();
}