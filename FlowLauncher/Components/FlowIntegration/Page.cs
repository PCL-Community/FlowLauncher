using FlowLauncher.Components.UI;

namespace FlowLauncher.Components.FlowIntegration;

public static class Page
{
    public static Task RegisterFirstLoading(Func<PageViewModel> page)
    {
        RootLayoutViewModel.RegisterFirstLoadingPage(page);
        return Task.CompletedTask;
    }

    public static Task Register(PageViewModel page)
    {
        BaseWindow.RootLayout.RegisterPage(page);
        return Task.CompletedTask;
    }
}
