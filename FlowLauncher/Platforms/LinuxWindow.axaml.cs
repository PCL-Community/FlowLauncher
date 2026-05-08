using FlowLauncher.Views;

namespace FlowLauncher.Platforms;

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
