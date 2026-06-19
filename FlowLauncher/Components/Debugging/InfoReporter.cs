namespace FlowLauncher.Components.Debugging;

#if DEBUG

[Flow.Scope("info")]
public static partial class InfoReporter
{
    [Flow.Task] [Flow.Run(After = "app")]
    public static Task ReportStart()
    {
        Console.WriteLine("App started");
        return Task.CompletedTask;
    }

    [Flow.Task] [Flow.Run(After = "app:stop")]
    public static Task ReportStop()
    {
        Console.WriteLine("App stopped");
        return Task.CompletedTask;
    }
}

#endif
