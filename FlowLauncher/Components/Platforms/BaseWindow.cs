using Avalonia.Controls;
using Avalonia.Input;
using RootLayoutViewModel = FlowLauncher.Components.UI.RootLayoutViewModel;

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
        if (e.Key == Key.Escape)
        {
            RootLayout.BackCommand.Execute(null);
        }
    }
}
