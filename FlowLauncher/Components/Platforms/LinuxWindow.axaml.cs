using FlowLauncher.Components.UI;

namespace FlowLauncher.Components.Platforms;

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
