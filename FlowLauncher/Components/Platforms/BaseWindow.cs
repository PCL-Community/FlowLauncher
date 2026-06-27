using Avalonia.Controls;
using Avalonia.Input;
using FlowLauncher.Components.UI;

namespace FlowLauncher.Components.Platforms;

public class BaseWindow : Window
{
    public static RootLayoutViewModel RootLayout { get; } = new();

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        Flow.InvokeTask("app:func:stop");
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        base.OnKeyUp(e);
        if (e.Key == Key.Escape) ClickBack();
    }

    protected static void ClickBack()
    {
        RootLayout.BackCommand.Execute(null);
    }

    protected static void ClickTasks()
    {
        if (RootLayout.CurrentPagePreview.Id == "tasks") RootLayout.BackCommand.Execute(null);
        else RootLayout.ForwardCommand.Execute("tasks");
    }
}
