using CommunityToolkit.Mvvm.Input;

namespace FlowLauncher.ViewModels;

public partial class RootLayoutViewModel : ViewModelBase
{
    private static readonly Dictionary<string, PageViewModel> _NavigateMap = new()
    {
        ["main"] = new MainPageViewModel(),
        ["install"] = new InstallPageViewModel(),
        ["tools"] = new ToolsPageViewModel(),
        ["settings"] = new SettingsPageViewModel(),
    };

    public static void RegisterPage(PageViewModel page)
    {
        _NavigateMap[page.Id] = page;
    }

    private readonly Stack<PageViewModel> _BackStack = [];

    public PageViewModel? LastPage
    {
        get;
        private set
        {
            if (field == value) return;
            SetProperty(ref field, value);
        }
    }

    public bool HasLastPage
    {
        get;
        private set
        {
            if (field == value) return;
            SetProperty(ref field, value);
        }
    }

    public PageViewModel CurrentPage
    {
        get;
        private set
        {
            HasLastPage = _BackStack.TryPeek(out var page);
            LastPage = HasLastPage ? page! : null;
            if (field == value) return;
            SetProperty(ref field, value);
        }
    } = _NavigateMap["main"];

    private void _Navigate(string pageId, bool forward)
    {
        if (!_NavigateMap.TryGetValue(pageId, out var page)) return;
        if (forward) _BackStack.Push(CurrentPage);
        else _BackStack.Clear();
        CurrentPage = page;
    }

    [RelayCommand]
    private void Back()
    {
        if (_BackStack.TryPop(out var page)) CurrentPage = page;
    }

    [RelayCommand]
    private void Navigate(string pageId) => _Navigate(pageId, false);

    [RelayCommand]
    private void Forward(string pageId) => _Navigate(pageId, true);
}
