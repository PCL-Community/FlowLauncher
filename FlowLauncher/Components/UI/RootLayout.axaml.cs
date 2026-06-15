using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Presenters;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using FlowLauncher.Controls;

namespace FlowLauncher.Components.UI;

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

    private RootLayoutViewModel ViewModel => DataContext as RootLayoutViewModel
        ?? throw new InvalidOperationException("Layout view model not set or type mismatch.");

    protected override void OnDataContextChanged(EventArgs e)
    {
        base.OnDataContextChanged(e);
        if (DataContext is not RootLayoutViewModel) return;
        ViewModel.RegisterPropertyChanged(nameof(ViewModel.CurrentPage), OnPageChanged);
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        base.OnKeyUp(e);
        if (e.Key == Key.Escape)
        {
            ViewModel.BackCommand.Execute(null);
        }
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        OnPageChanged();
    }

    private void OnPageChanged()
    {
        Dispatcher.UIThread.Invoke<Task>(async () =>
        {
            // wait for control loading
            while (LeftMenuItemsControl.ItemsPanelRoot == null) await Task.Delay(100);
            // trigger animations
            var controlItems = LeftMenuItemsControl.ItemsPanelRoot.Children;
            if (controlItems.Count == 0) return;
            foreach (var item in controlItems)
            {
                await Task.Delay(TimeSpan.FromSeconds(.05));
                if (item is not ContentPresenter { Child: { RenderTransform: TranslateTransform transform } control }) break;
                transform.X = 0;
                control.Opacity = 1;
            }
        });
    }

    private void TopBar_OnPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        ParentWindow?.BeginMoveDrag(e);
    }

    private void LeftMenuItem_OnClick(object? sender, RoutedEventArgs e)
    {
        if (sender is not FlowButton { Tag: ContentViewModel targetContent }) return;
        ViewModel.SwitchContentCommand.Execute(targetContent);
    }
}
