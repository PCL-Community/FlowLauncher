using System.Windows.Input;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FlowLauncher.Components.UI;

public partial class MenuItemViewModel : ViewModelBase
{
    [ObservableProperty]
    public partial bool IsEnabled { get; set; } = true;

    [ObservableProperty]
    public partial string Title { get; set; } = "";

    [ObservableProperty]
    public partial Geometry? Icon { get; set; } = null;

    [ObservableProperty]
    public partial string? ToolTip { get; set; } = null;

    [ObservableProperty]
    public partial ICommand? Command { get; set; } = null;

    [ObservableProperty]
    public partial object? CommandParameter { get; set; } = null;

    [ObservableProperty]
    public partial ICommand? RefreshCommand { get; set; } = null;

    [ObservableProperty]
    public partial object? RefreshCommandParameter { get; set; } = null;

    [ObservableProperty]
    public partial ICommand? RestoreCommand { get; set; } = null;

    [ObservableProperty]
    public partial object? RestoreCommandParameter { get; set; } = null;
}

public partial class LeftMenuItemViewModel : MenuItemViewModel
{
    [ObservableProperty]
    public partial ContentViewModel? TargetContent { get; set; } = null;
}

public class MenuTitleViewModel : MenuItemViewModel;

public class LeftMenuTitleViewModel : LeftMenuItemViewModel;
