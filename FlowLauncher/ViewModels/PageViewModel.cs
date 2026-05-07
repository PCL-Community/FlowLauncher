namespace FlowLauncher.ViewModels;

public abstract class PageViewModel(string id, string title = "Untitled") : ViewModelBase
{
    public string Id { get; } = id;

    public string Title { get; protected set => SetProperty(ref field, value); } = title;
}
