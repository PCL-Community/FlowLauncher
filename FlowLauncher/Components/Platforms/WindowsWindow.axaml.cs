using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using FlowLauncher.Components.UI;

namespace FlowLauncher.Components.Platforms;

public sealed partial class WindowsWindow : BaseWindow
{
    public static readonly StyledProperty<bool> UseNativeWindowFrameProperty =
        AvaloniaProperty.Register<WindowsWindow, bool>(nameof(UseNativeWindowFrame));

    public bool UseNativeWindowFrame
    {
        get => GetValue(UseNativeWindowFrameProperty);
        set => SetValue(UseNativeWindowFrameProperty, value);
    }

    static WindowsWindow()
    {
        UseNativeWindowFrameProperty.Changed.AddClassHandler<WindowsWindow, bool>((sender, e) =>
        {
            sender.UpdateNativeWindowFrameProps(e.NewValue.Value);
        });
    }

    private void UpdateWindowStartupAnimationProps()
    {
        RootPanel.RenderTransform = new ScaleTransform(1.0, 1.0);
        RootPanel.Opacity = 1;
    }

    private void UpdateNativeWindowFrameProps(bool useNativeWindowFrame)
    {
        if (useNativeWindowFrame)
        {
            Resources["WindowMargin"] = default(Thickness);
            Resources["WindowCornerRadius"] = default(CornerRadius);
            if (OperatingSystem.IsWindowsVersionAtLeast(10, 0, 22000))
            {
                TransparencyLevelHint = [WindowTransparencyLevel.Mica, WindowTransparencyLevel.Transparent];
                Resources["FlowWindowBackground"] = Colors.Transparent;
            }
            else
            {
                TransparencyLevelHint = [WindowTransparencyLevel.AcrylicBlur, WindowTransparencyLevel.Transparent];
                Resources.Remove("FlowWindowBackground");
            }
        }
        else
        {
            Resources["WindowMargin"] = new Thickness(8);
            Resources["WindowCornerRadius"] = new CornerRadius(8);
            TransparencyLevelHint = [WindowTransparencyLevel.Transparent];
            Resources.Remove("FlowWindowBackground");
        }
    }

    public WindowsWindow()
    {
        UpdateNativeWindowFrameProps(false);
        InitializeComponent();
        RootLayout.RegisterPropertyChanged(nameof(RootLayout.HasLastPage), UpdateNavigation);
        if (UseNativeWindowFrame) UpdateWindowStartupAnimationProps();
    }

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);
        UpdateWindowStartupAnimationProps();
    }

    private void UpdateNavigation()
    {
        Dispatcher.UIThread.Invoke(async () =>
        {
            await Task.Delay(TimeSpan.FromMilliseconds(100));
            if (RootLayout.HasLastPage) TitlePanel.IsVisible = false;
            else BackPanel.IsVisible = false;
        });
        TitlePanel.IsVisible = true;
        BackPanel.IsVisible = true;
        if (RootLayout.HasLastPage)
        {
            ((TranslateTransform)TitlePanel.RenderTransform!).X = 40;
            ((TranslateTransform)BackPanel.RenderTransform!).X = 0;
        }
        else
        {
            ((TranslateTransform)BackPanel.RenderTransform!).X = -40;
            ((TranslateTransform)TitlePanel.RenderTransform!).X = 0;
        }
    }

    private void ButtonClose_OnClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void ButtonMinimize_OnClick(object? sender, RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    private void BackButton_OnClick(object? sender, RoutedEventArgs e) => ClickBack();

    private void ButtonTestNativeWindow_OnClick(object? sender, RoutedEventArgs e)
    {
        UseNativeWindowFrame = !UseNativeWindowFrame;
    }

    private void ButtonTasks_OnClick(object? sender, RoutedEventArgs e) => ClickTasks();
}
