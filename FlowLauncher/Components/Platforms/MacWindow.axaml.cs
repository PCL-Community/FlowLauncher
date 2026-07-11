using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using FlowLauncher.Components.UI;

namespace FlowLauncher.Components.Platforms;

public sealed partial class MacWindow : BaseWindow
{
    public MacWindow()
    {
        InitializeComponent();
        RootLayout.RegisterPropertyChanged(nameof(RootLayout.HasLastPage), UpdateNavigation);
    }

    private void UpdateNavigation()
    {
        Dispatcher.UIThread.Invoke(async () =>
        {
            await Task.Delay(TimeSpan.FromMilliseconds(100));
            if (!RootLayout.HasLastPage) BackPanel.IsVisible = false;
        });
        BackPanel.IsVisible = true;
        ((TranslateTransform)BackPanel.RenderTransform!).X = RootLayout.HasLastPage ? 0 : -40;
    }

    private void BackButton_OnClick(object? sender, RoutedEventArgs e) => ClickBack();

    private void TasksButton_OnClick(object? sender, RoutedEventArgs e) => ClickTasks();
}
