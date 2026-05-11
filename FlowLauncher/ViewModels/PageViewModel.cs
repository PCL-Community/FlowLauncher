using System.Collections.ObjectModel;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FlowLauncher.ViewModels;

public abstract partial class PageViewModel(string id, string title = "Untitled") : ViewModelBase
{
    public string Id { get; } = id;

    public string Title { get; protected set => SetProperty(ref field, value); } = title;

    [ObservableProperty] private Collection<MenuItemViewModel> _leftMenuItems = new ObservableCollection<MenuItemViewModel>();

    [ObservableProperty] private Control? _leftExtraContent = null;

    [ObservableProperty] private Control? _content = null;
}
