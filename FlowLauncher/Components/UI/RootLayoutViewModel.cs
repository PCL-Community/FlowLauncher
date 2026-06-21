using Avalonia.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace FlowLauncher.Components.UI;

public partial class RootLayoutViewModel : ViewModelBase
{
    private static readonly List<Func<PageViewModel>> FirstLoadingPages = [];

    public static void RegisterFirstLoadingPage(Func<PageViewModel> page) => FirstLoadingPages.Add(page);

    private Dictionary<string, PageViewModel> _NavigateMap => field
        ??= FirstLoadingPages.Select(f => f()).ToDictionary(p => p.Id);

    public void RegisterPage(PageViewModel page) => _NavigateMap[page.Id] = page;

    private readonly Stack<PageViewModel> _BackStack = [];

    [ObservableProperty]
    public partial PageViewModel? LastPage { get; private set; }

    [ObservableProperty]
    public partial bool HasLastPage { get; private set; }

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

    public ContentViewModel? CurrentPageContentPreview
    {
        get => field ??= CurrentPagePreview.Content;
        private set => SetProperty(ref field, value);
    }

    public PageViewModel CurrentPage
    {
        get => field ??= CurrentPagePreview;
        private set => SetProperty(ref field, value);
    }

    [ObservableProperty]
    public partial bool CurrentPageHasLeftExtraContent { get; private set; }

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
            CurrentPageHasLeftExtraContent = page.LeftExtraContent != null;
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

    [RelayCommand]
    private void Back() => _Navigate(null);

    [RelayCommand]
    private void Navigate(string pageId) => _Navigate(pageId, false);

    [RelayCommand]
    private void Forward(string pageId) => _Navigate(pageId);

    [RelayCommand]
    private void SwitchContent(ContentViewModel target)
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
}
