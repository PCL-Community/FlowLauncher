using FlowLauncher.Views;

namespace FlowLauncher.Platforms;

public sealed partial class MacWindow : BaseWindow
{
    public MacWindow()
    {
        Content = new RootLayout
        {
            ParentWindow = this,
            DataContext = RootLayout
        };
        InitializeComponent();
    }
}
