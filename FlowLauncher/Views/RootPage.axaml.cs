using Avalonia.Controls;
using FlowLauncher.ViewModels;

namespace FlowLauncher.Views;

public partial class RootPage : UserControl
{
    public RootPage()
    {
        DataContext = new RootPageViewModel();
        InitializeComponent();
    }
}
