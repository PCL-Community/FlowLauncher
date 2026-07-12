using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FlowLauncher.Components.UI;

public partial class RootLayoutViewModel : ViewModelBase
{
    private static readonly List<Func<PageViewModel>> FirstLoadingPages = [];

    /// <summary>
    /// 注册首次加载的页面视图模型，将通过委托在布局初始化后懒加载，页面 ID 在加载后可用
    /// </summary>
    /// <param name="page">加载该视图模型的委托</param>
    public static void RegisterFirstLoadingPage(Func<PageViewModel> page) => FirstLoadingPages.Add(page);

    private Dictionary<string, PageViewModel> _NavigateMap => field
        ??= FirstLoadingPages.Select(f => f()).ToDictionary(p => p.Id);

    /// <summary>
    /// 注册页面视图模型，页面 ID 将立即可用
    /// </summary>
    /// <param name="page">视图模型</param>
    public void RegisterPage(PageViewModel page) => _NavigateMap[page.Id] = page;

    private readonly Stack<PageViewModel> _BackStack = [];

    /// <summary>
    /// 上一页面，当 <see cref="HasLastPage"/> 为 <see langword="false"/> 时该值为 <see langword="null"/>
    /// </summary>
    [ObservableProperty]
    public partial PageViewModel? LastPage { get; private set; }

    /// <summary>
    /// 指示是否存在上一页面
    /// </summary>
    [ObservableProperty]
    public partial bool HasLastPage { get; private set; }

    /// <summary>
    /// 当前页面的预览视图模型
    /// </summary>
    public PageViewModel CurrentPagePreview
    {
        get => field ??= _NavigateMap["main"];
        private set
        {
            HasLastPage = _BackStack.TryPeek(out var page);
            LastPage = HasLastPage ? page! : null;
            SetProperty(ref field, value);
            CurrentPageContentPreview = value.Content;
        }
    }

    /// <summary>
    /// 当前页面内容的视图模型
    /// </summary>
    public ContentViewModel? CurrentPageContentPreview
    {
        get => field ??= CurrentPagePreview.Content;
        private set => SetProperty(ref field, value);
    }

    /// <summary>
    /// 当前页面的视图模型
    /// </summary>
    public PageViewModel CurrentPage
    {
        get
        {
            if (field != null) return field;
            field = CurrentPagePreview;
            CurrentPageHasLeftExtraContent = field.LeftExtraContent != null;
            return field;
        }
        private set
        {
            SetProperty(ref field, value);
            CurrentPageHasLeftExtraContent = value.LeftExtraContent != null;
        }
    }

    /// <summary>
    /// 指示当前页面左侧侧栏是否有额外内容
    /// </summary>
    [ObservableProperty] public partial bool CurrentPageHasLeftExtraContent { get; private set; }

    // === Animation Variables ===
    [ObservableProperty] public partial double _LeftMenuControl_TranslateX { get; private set; } = 0;
    [ObservableProperty] public partial double _LeftMenuControl_Opacity { get; private set; } = 1;
    [ObservableProperty] public partial double _LeftExtraControl_Scale { get; private set; } = 1;
    [ObservableProperty] public partial double _LeftExtraControl_Opacity { get; private set; } = 1;
    [ObservableProperty] public partial double _MainContent_TranslateY { get; internal set; } = 0;
    [ObservableProperty] public partial double _MainContent_Opacity { get; internal set; } = 1;

    private void _Navigate(string? pageId, bool forward = true)
    {
        if (CurrentPagePreview.Id == pageId) return; // prevent loop
        // get navigate target
        PageViewModel? page;
        if (pageId == null)
        {
            if (_BackStack.Count <= 0 || !CurrentPagePreview.TriggerBack()) return;
            if (!_BackStack.TryPop(out page)) return;
        }
        else
        {
            if (!_NavigateMap.TryGetValue(pageId, out page)) return;
            if (forward) _BackStack.Push(CurrentPagePreview);
            else _BackStack.Clear();
        }
        // navigate & play animation
        Dispatcher.UIThread.Invoke(async () =>
        {
            page.TriggerEnterPage();
            CurrentPagePreview = page;
            _MainContent_Opacity = 0;
            _LeftExtraControl_Scale = .6;
            _LeftExtraControl_Opacity = 0;
            _LeftMenuControl_TranslateX = -20;
            _LeftMenuControl_Opacity = 0;
            await Task.Delay(TimeSpan.FromSeconds(.05));
            _MainContent_TranslateY = -80;
            await Task.Delay(TimeSpan.FromSeconds(.05));
            var leavingPage = CurrentPage;
            CurrentPage = page;
            page.Content?.ViewControl.DataContext = page.Content.ViewModel;
            _LeftExtraControl_Scale = 1;
            _LeftExtraControl_Opacity = 1;
            _LeftMenuControl_TranslateX = 0;
            _LeftMenuControl_Opacity = 1;
            await Task.Delay(TimeSpan.FromSeconds(.1));
            _MainContent_TranslateY = 0;
            await Task.Delay(TimeSpan.FromSeconds(.05));
            _MainContent_Opacity = 1;
            leavingPage.TriggerLeavePage();
        });
    }

    /// <summary>
    /// 返回上一页
    /// </summary>
    [RelayCommand]
    public void Back() => _Navigate(null);

    /// <summary>
    /// 切换到指定页并清空返回栈
    /// </summary>
    /// <param name="pageId">指定页面 ID</param>
    [RelayCommand]
    public void Navigate(string pageId) => _Navigate(pageId, false);

    /// <summary>
    /// 切换到指定页并记录返回栈
    /// </summary>
    /// <param name="pageId">指定页面 ID</param>
    [RelayCommand]
    public void Forward(string pageId) => _Navigate(pageId);

    /// <summary>
    /// 切换页面内容
    /// </summary>
    /// <param name="target">目标内容的视图模型</param>
    [RelayCommand]
    public void SwitchContent(ContentViewModel target)
    {
        if (CurrentPagePreview.Content == target) return;
        Dispatcher.UIThread.Invoke(async () =>
        {
            CurrentPageContentPreview = target;
            _MainContent_Opacity = 0;
            await Task.Delay(TimeSpan.FromSeconds(.05));
            _MainContent_TranslateY = -80;
            await Task.Delay(TimeSpan.FromSeconds(.05));
            CurrentPagePreview.Content = target;
            target.ViewControl.DataContext = target.ViewModel;
            await Task.Delay(TimeSpan.FromSeconds(.1));
            _MainContent_TranslateY = 0;
            await Task.Delay(TimeSpan.FromSeconds(.05));
            _MainContent_Opacity = 1;
        });
    }

    /// <summary>
    /// 消息中心
    /// </summary>
    public MessageHostModel MessageHost { get; }

    public RootLayoutViewModel()
    {
        MessageHost = new MessageHostModel(this);
    }
}
