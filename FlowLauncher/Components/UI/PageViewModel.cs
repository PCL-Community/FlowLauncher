using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Layout;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;

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

    protected virtual void OnEnterPage() { }
    internal void TriggerEnterPage() => OnEnterPage();

    protected virtual void OnLeavePage() { }
    internal void TriggerLeavePage() => OnLeavePage();

    protected virtual bool OnBack() => true;
    internal bool TriggerBack() => OnBack();

    /// <summary>
    /// 引用全局资源字典中的几何图标，图标资源应为 <see cref="Geometry"/> 类型，键名以 <c>Icon</c> 开头
    /// </summary>
    /// <param name="resourceKey">资源键，可省略 <c>Icon</c> 前缀</param>
    /// <returns>图标实例</returns>
    protected static Geometry? ReferIcon(string resourceKey)
    {
        if (!resourceKey.StartsWith("Icon")) resourceKey = "Icon" + resourceKey;
        if (Application.Current?.Resources is not { } res) return null;
        if (!res.TryGetValue(resourceKey, out var value)) return null;
        return value as Geometry;
    }

    /// <summary>
    /// 引用内容视图模型，并将本类作为该内容的父页面
    /// </summary>
    /// <typeparam name="TThisClass">本类</typeparam>
    /// <typeparam name="TContentViewModel">内容视图模型的类型</typeparam>
    /// <returns>内容视图模型的实例</returns>
    /// <exception cref="InvalidCastException"><typeparamref name="TThisClass"/> 传入的类型与本类不兼容</exception>
    protected ContentViewModel<TThisClass> ReferContent<TThisClass, TContentViewModel>()
        where TThisClass : PageViewModel
        where TContentViewModel : ContentViewModel<TThisClass>, new()
    {
        return this is TThisClass page ? new TContentViewModel { Page = page }
            : throw new InvalidCastException("Type mismatch: please use current class as the first type parameter.");
    }

    /// <summary>
    /// 引用内容视图，本类将直接成为该内容的视图模型<br/>
    /// <b>NOTE</b>: 该引用将缓存并复用视图实例，请确保引用的内容视图遵循标准的 MVVM 实现，否则将导致多处引用时的状态同步问题
    /// </summary>
    /// <param name="bypassCache">跳过缓存直接初始化新实例，用于同时显示多个同一类型视图或引用的视图不遵循 MVVM 实现的情境，常规情况下不建议启用</param>
    /// <typeparam name="TContent">内容视图的类型</typeparam>
    /// <returns>内容视图的实例</returns>
    protected ContentViewModel ReferContent<TContent>(bool bypassCache = false)
        where TContent : ContentView, new()
    {
        var view = ContentView.GetViewCacheOrCreate<TContent>(bypassCache);
        return new SimpleContentViewModel(view, this);
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

file class SimpleContentViewModel : ContentViewModel
{
    public override ContentView ViewControl { get; }

    public SimpleContentViewModel(ContentView view, PageViewModel viewModel)
    {
        ViewControl = view;
        ViewModel = viewModel;
    }
}
