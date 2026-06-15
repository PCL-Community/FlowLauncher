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

    public ContentViewModel? LeftExtraContent
    {
        get;
        init
        {
            value?.ViewControl.DataContext = value.ViewModel;
            field = value;
        }
    } = null;

    [ObservableProperty]
    public partial ContentViewModel? Content { get; set; } = null;

    [ObservableProperty]
    public partial VerticalAlignment LeftExtraContentAlignment { get; set; } = VerticalAlignment.Stretch;

    protected static Geometry? ReferIcon(string resourceKey)
    {
        if (!resourceKey.StartsWith("Icon")) resourceKey = "Icon" + resourceKey;
        if (Application.Current?.Resources is not { } res) return null;
        if (!res.TryGetValue(resourceKey, out var value)) return null;
        return value as Geometry;
    }

    protected ContentViewModel<TThisClass> ReferContent<TThisClass, TContentViewModel>()
        where TThisClass : PageViewModel
        where TContentViewModel : ContentViewModel<TThisClass>, new()
    {
        return this is TThisClass page ? new TContentViewModel { Page = page }
            : throw new InvalidCastException("Type mismatch: please use current class as the first type parameter.");
    }

    protected ContentViewModel ReferContent<TContent>(bool bypassCache = false)
        where TContent : ContentView, new()
    {
        var view = ContentView.GetViewCacheOrCreate<TContent>(bypassCache);
        return new SimplePageContentViewModel(view, this);
    }
}

public abstract class PageViewModel<TThisClass, TMainContent> : PageViewModel
    where TThisClass : PageViewModel
    where TMainContent : ContentViewModel<TThisClass>, new()
{
    protected PageViewModel(string id, string title = "Untitled") : base(id, title)
    {
        Content = ReferContent<TThisClass, TMainContent>();
    }
}

public abstract class PageViewModel<TMainContent> : PageViewModel
    where TMainContent : ContentView, new()
{
    protected PageViewModel(string id, string title = "Untitled") : base(id, title)
    {
        Content = ReferContent<TMainContent>();
    }
}

file class SimplePageContentViewModel : ContentViewModel
{
    public override ContentView ViewControl { get; }

    public SimplePageContentViewModel(ContentView view, PageViewModel viewModel)
    {
        ViewControl = view;
        ViewModel = viewModel;
    }
}
