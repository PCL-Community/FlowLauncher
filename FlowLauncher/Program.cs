using System.Runtime.InteropServices;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Threading;
using FlowLauncher.Platforms;
using FlowNet.Core;

namespace FlowLauncher;

[Flow.Scope("app")]
static partial class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
#if DEBUG
        Flow.EnableTaskInvokingInfo = true;
#endif
        Task.Run(async () =>
        {
            try { await FlowInterops.Run(); }
            catch (Exception ex) { OnFatalException(ex); }
        });
        BuildAvaloniaApp().Start(AppMain, args);
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();

    private static readonly TaskCompletionSource OnAvaloniaInitialized = new();
    private static readonly ManualResetEventSlim OnCreateWindow = new();
    private static readonly TaskCompletionSource OnAvaloniaEnd = new();
    private static readonly ManualResetEventSlim OnProgramEnd = new();
    private static readonly TaskCompletionSource OnProgramExit = new();
    private static readonly CancellationTokenSource OnAvaloniaEndTokenSource = new();

    private static void OnFatalException(Exception ex)
    {
        Console.Error.WriteLine(ex);
        Environment.Exit(1);
    }

    private static void AppMain(Application application, string[] args)
    {
        if (application is not App app) return;
        Dispatcher.UIThread.UnhandledException += (_, e) => OnFatalException(e.Exception);
        OnAvaloniaInitialized.SetResult();
        OnCreateWindow.Wait();
        CreateMainWindow();
        app.Run(OnAvaloniaEndTokenSource.Token);
        OnAvaloniaEnd.SetResult();
        OnProgramEnd.Wait();
        OnProgramExit.SetResult();
    }

    private static void CreateMainWindow()
    {
        Window mainWindow;
        if (OperatingSystem.IsWindows()) mainWindow = new WindowsWindow();
        else if (OperatingSystem.IsMacOS()) mainWindow = new MacWindow();
        else if (OperatingSystem.IsLinux()) mainWindow = new LinuxWindow();
        else throw new NotSupportedException($"Platform not supported: {RuntimeInformation.OSDescription}");
        mainWindow.Show();
    }

    [Flow.Task] [Flow.Run]
    [Flow.Task("load:before")] [Flow.Run(After = "app")]
    [Flow.Task("run:before")] [Flow.Run(After = "app:load")]
    [Flow.Task("stop:before")] [Flow.Run(After = "app:run")]
    [Flow.Task("stop")] [Flow.Run(After = "app:stop:before")]
    [Flow.Task("exit:before")] [Flow.Run(After = "app:stop")]
    [Flow.Task("exit")] [Flow.Run(After = "app:exit:before")]
    private static Task _([Flow.InvokingInfo] FlowTaskInvokingInfo info) {
#if DEBUG
        var (target, _, callers) = info;
        Console.WriteLine($"Wildcard '{target}' was invoked by ['{string.Join("', '", callers)}']");
#endif
        return Task.CompletedTask;
    }

    [Flow.Task] [Flow.Run(After = "app:load:before")]
    private static Task Load() => OnAvaloniaInitialized.Task;

    [Flow.Task] [Flow.Run(After = "app:run:before")]
    private static Task Run()
    {
        OnCreateWindow.Set();
        return OnAvaloniaEnd.Task;
    }

    private static bool _isStopping = false;

    [Flow.Task("func:stop")]
    private static async Task StopAvaloniaAsync()
    {
        if (_isStopping)
        {
            await OnProgramExit.Task.ConfigureAwait(false);
            return;
        }
        _isStopping = true;
        Console.WriteLine("Stopping...");
        await OnAvaloniaEndTokenSource.CancelAsync().ConfigureAwait(false);
    }

    [Flow.Task("func:exit")] [Flow.Run(After = "app:exit")]
    private static async Task EndProgram()
    {
        Console.WriteLine("Exiting...");
        OnProgramEnd.Set();
        await OnProgramExit.Task.WaitAsync(TimeSpan.FromSeconds(5)).ConfigureAwait(false);
        if (OnProgramExit.Task.IsCompleted) return;
        Console.WriteLine("Exit request timeout, force exiting...");
        Environment.Exit(1);
    }
}
