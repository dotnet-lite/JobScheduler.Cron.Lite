namespace JobScheduler.Cron.UnitTests;

public sealed class Job(TimeProvider timeProvider) : IJob, IAsyncDisposable
{
    public const string Cron = "*/1 * * * * *";
    public const int CountExpectedExecutions = 5;
    public static TimeSpan ExecutionTime { get; set; }
    public static readonly TimeSpan CronTimeBetweenExecutions = TimeSpan.FromSeconds(1);
    public static readonly TimeSpan AllowedTimeMargin = TimeSpan.FromMilliseconds(100);

    public List<DateTimeOffset> TimeStartExecution { get; } = [];
    public bool ExecuteDisposeAsync { get; set; }

    public async Task Execute(CancellationToken cancellationToken)
    {
        TimeStartExecution.Add(timeProvider.GetUtcNow());
        try
        {
            await Task.Delay(ExecutionTime, cancellationToken);
        }
        catch (TaskCanceledException)
        {
            // Expected exception
        }
    }

    public ValueTask DisposeAsync()
    {
        ExecuteDisposeAsync = true;
        return ValueTask.CompletedTask;
    }
}