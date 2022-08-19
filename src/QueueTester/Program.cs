using QueueTester;
using QueueTester.QueueService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// IHost host = Host.CreateDefaultBuilder(args)
//     .ConfigureServices((context, services) =>
//     {
//         services.AddSingleton<MonitorLoop>();
//         services.AddHostedService<QueuedHostedService>();
//         services.AddSingleton<IBackgroundTaskQueue>(_ =>
//         {
//             if (!int.TryParse(context.Configuration["QueueCapacity"], out var queueCapacity))
//             {
//                 queueCapacity = 100;
//             }

//             return new DefaultBackgroundTaskQueue(queueCapacity);
//         });
//     })
//     .Build();


builder.Services.AddSingleton<MonitorLoop>();
builder.Services.AddSingleton<IdService>();
builder.Services.AddHostedService<QueuedHostedService>();
builder.Services.AddSingleton<IBackgroundTaskQueue>(_ =>
{
    if (!int.TryParse(builder.Configuration["QueueCapacity"], out var queueCapacity))
    {
        queueCapacity = 10;
    }
    return new DefaultBackgroundTaskQueue(queueCapacity);
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.Run();
