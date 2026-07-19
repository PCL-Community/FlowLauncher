using FlowLauncher.Components.FlowIntegration;
using FlowLauncher.Components.UI;

namespace FlowLauncher.Components.Platforms;

[PlatformSelector.Is(FlowPlatform.Linux)]
[PlatformWindow]
public sealed partial class LinuxWindow : BaseWindow
{
    public LinuxWindow()
    {
        Content = new RootLayout
        {
            ParentWindow = this,
            DataContext = RootLayout
        };
        InitializeComponent();
    }
}
