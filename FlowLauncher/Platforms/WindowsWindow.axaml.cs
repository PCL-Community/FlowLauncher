using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;

namespace FlowLauncher.Platforms;

public sealed partial class WindowsWindow : BaseWindow
{
    public WindowsWindow()
    {
        InitializeComponent();
        RootLayout.RegisterPropertyChanged(nameof(RootLayout.HasLastPage), UpdateNavigation);
    }

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);
        BackgroundPanel.RenderTransform = new ScaleTransform(1.0, 1.0);
        BackgroundPanel.Opacity = 1;
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
            TitlePanel.Opacity = 0;
            ((TranslateTransform)BackPanel.RenderTransform!).X = 0;
            BackPanel.Opacity = 1;
        }
        else
        {
            ((TranslateTransform)BackPanel.RenderTransform!).X = -40;
            BackPanel.Opacity = 0;
            ((TranslateTransform)TitlePanel.RenderTransform!).X = 0;
            TitlePanel.Opacity = 1;
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

    private void TitleButton_OnClick(object? sender, RoutedEventArgs e)
    {
        RootLayout.ForwardCommand.Execute("main");
    }

    private void BackButton_OnClick(object? sender, RoutedEventArgs e)
    {
        RootLayout.BackCommand.Execute(null);
    }
}
