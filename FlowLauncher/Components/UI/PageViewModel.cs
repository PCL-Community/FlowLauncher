using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Layout;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using FlowLauncher.Components.Platforms;

namespace FlowLauncher.Components.UI;

public abstract partial class PageViewModel(string id, string title = "Untitled") : ViewModelBase
{
    public static RootLayoutViewModel RootLayout => BaseWindow.RootLayout;

    public string Id { get; } = id;

    public string Title { get; protected set => SetProperty(ref field, value); } = title;

    [ObservableProperty]
    public partial Collection<LeftMenuItemViewModel> LeftMenuItems { get; set; } = new ObservableCollection<LeftMenuItemViewModel>();

    public PageContentViewModel? LeftExtraContent
    {
        get;
        init
        {
            value?.ViewControl.DataContext = value.ViewModel;
            field = value;
        }
    } = null;

    [ObservableProperty]
    public partial PageContentViewModel? Content { get; set; } = null;

    [ObservableProperty]
    public partial VerticalAlignment LeftExtraContentAlignment { get; set; } = VerticalAlignment.Stretch;

    protected static Geometry? Icon(string resourceKey)
    {
        if (!resourceKey.StartsWith("Icon")) resourceKey = "Icon" + resourceKey;
        if (Application.Current?.Resources is not { } res) return null;
        if (!res.TryGetValue(resourceKey, out var value)) return null;
        return value as Geometry;
    }

    protected PageContentViewModel<TThisClass> PageContent<TThisClass, TPageContent>()
        where TThisClass : PageViewModel
        where TPageContent : PageContentViewModel<TThisClass>, new()
    {
        return this is TThisClass page ? new TPageContent { Page = page }
            : throw new InvalidCastException("Type mismatch: please use current class as the first type parameter.");
    }

    protected PageContentViewModel PageContent<TPageContent>(bool bypassCache = false)
        where TPageContent : PageContentView, new()
    {
        var view = PageContentView.GetViewCacheOrCreate<TPageContent>(bypassCache);
        return new SimplePageContentViewModel(view, this);
    }
}

public abstract class PageViewModel<TThisClass, TMainContent> : PageViewModel
    where TThisClass : PageViewModel
    where TMainContent : PageContentViewModel<TThisClass>, new()
{
    protected PageViewModel(string id, string title = "Untitled") : base(id, title)
    {
        Content = PageContent<TThisClass, TMainContent>();
    }
}

public abstract class PageViewModel<TMainContent> : PageViewModel
    where TMainContent : PageContentView, new()
{
    protected PageViewModel(string id, string title = "Untitled") : base(id, title)
    {
        Content = PageContent<TMainContent>();
    }
}

file class SimplePageContentViewModel : PageContentViewModel
{
    public override PageContentView ViewControl { get; }

    public SimplePageContentViewModel(PageContentView view, PageViewModel viewModel)
    {
        ViewControl = view;
        ViewModel = viewModel;
    }
}
