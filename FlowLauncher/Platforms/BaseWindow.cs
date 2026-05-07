using Avalonia.Controls;
using FlowLauncher.ViewModels;
using FlowNet.Core;

namespace FlowLauncher.Platforms;

public class BaseWindow : Window
{
    public RootLayoutViewModel RootLayout { get; } = new();

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        Flow.InvokeTask("app:func:stop");
    }
}
