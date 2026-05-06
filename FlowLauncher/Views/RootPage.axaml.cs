using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using FlowLauncher.Controls;
using FlowLauncher.ViewModels;

namespace FlowLauncher.Views;

public partial class RootPage : UserControl
{
    public static readonly StyledProperty<Window?> ParentWindowProperty =
        AvaloniaProperty.Register<FlowRadioButton, Window?>(nameof(ParentWindow));

    public Window? ParentWindow
    {
        get => GetValue(ParentWindowProperty);
        set => SetValue(ParentWindowProperty, value);
    }

    public RootPage()
    {
        DataContext = new RootPageViewModel();
        InitializeComponent();
    }

    private void TopBar_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        ParentWindow?.BeginMoveDrag(e);
    }
}
