using FlowLauncher.Components.UI;
using FlowNet.Collector;

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

/// <summary>
/// Mark a view model class as a first-loading page, this class must have an empty
/// constructor and implement <see cref="PageViewModel"/>.
/// </summary>
[CollectorMeta("FlowLauncher.Components.FlowIntegration.Page.RegisterFirstLoading")]
[CollectorMeta.SupportsNamedType(TypeGenerationMode.EmptyConstructor, true)]
[CollectorMeta.SupportsMethod<Func<PageViewModel>>]
[AttributeUsage(AttributeTargets.Class)]
public sealed class FirstLoadingPageAttribute : Attribute;

/// <summary>
/// Mark a view model class as a page, this class must have an empty constructor and
/// implement <see cref="PageViewModel"/>.
/// </summary>
[CollectorMeta("FlowLauncher.Components.FlowIntegration.Page.RegisterFirstLoading")]
[CollectorMeta.SupportsNamedType(TypeGenerationMode.EmptyConstructor)]
[AttributeUsage(AttributeTargets.Class)]
public sealed class PageAttribute : Attribute;
