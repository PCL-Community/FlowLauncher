namespace FlowLauncher.Components.UI;

public abstract class ContentViewModel : ViewModelBase
{
    public abstract ContentView ViewControl { get; }

    public ViewModelBase ViewModel { get; protected init; }

    protected ContentViewModel()
    {
        ViewModel = this;
    }
}

public abstract class ContentViewModel<TParentPage> : ContentViewModel
    where TParentPage : PageViewModel
{
    public required TParentPage Page { get; init; }
}

public abstract class ContentViewModel<TParentPage, TContent> : ContentViewModel<TParentPage>
    where TParentPage : PageViewModel
    where TContent : ContentView, new()
{
    public override ContentView ViewControl { get; } = ContentView.GetViewCacheOrCreate<TContent>();
}
