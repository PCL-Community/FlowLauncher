using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

namespace FlowLauncher.Views;

public partial class RootLayout : UserControl
{
    public static readonly StyledProperty<Window?> ParentWindowProperty =
        AvaloniaProperty.Register<RootLayout, Window?>(nameof(ParentWindow));

    public Window? ParentWindow
    {
        get => GetValue(ParentWindowProperty);
        set => SetValue(ParentWindowProperty, value);
    }

    public RootLayout()
    {
        InitializeComponent();
    }

    private void TopBar_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        ParentWindow?.BeginMoveDrag(e);
    }
}
