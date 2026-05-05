using Avalonia.Controls;
using FlowLauncher.Views;

namespace FlowLauncher.Platforms;

public partial class MacWindow : Window
{
    public MacWindow()
    {
        Content = new RootPage();
        InitializeComponent();
    }
}
