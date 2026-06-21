using CommunityToolkit.Mvvm.Input;
using FlowLauncher.Components.FlowIntegration;
using FlowLauncher.Components.UI;
using FlowLauncher.Resources;
using FlowLauncher.Views;

namespace FlowLauncher.ViewModels;

[FirstLoadingPage]
public partial class ToolsViewModel : PageViewModel<ToolsPage>
{
    public ToolsViewModel() : base("tools", Strings.PageTitleTools)
    {
        LeftMenuItems = [
            new LeftMenuTitleViewModel
            {
                Title = "Title"
            },
            new LeftMenuItemViewModel
            {
                Title = "Settings",
                Icon = ReferIcon("Settings"),
                TargetContent = Content,
                RefreshCommand = new RelayCommand(() => {})
            },
            new LeftMenuItemViewModel
            {
                Title = "456",
                Icon = ReferIcon("Settings"),
                TargetContent = ReferContent<MainPage>()
            },
            new LeftMenuItemViewModel
            {
                Title = "789",
                Icon = ReferIcon("Refresh")
            },
        ];
    }
}
