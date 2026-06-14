using CommunityToolkit.Mvvm.Input;
using FlowLauncher.Components.UI;
using FlowLauncher.Resources;
using FlowLauncher.Views;

namespace FlowLauncher.ViewModels;

public partial class ToolsPageViewModel : PageViewModel<ToolsPage>
{
    public ToolsPageViewModel() : base("tools", Strings.PageTitleTools)
    {
        LeftMenuItems = [
            new LeftMenuTitleViewModel
            {
                Title = "Title"
            },
            new LeftMenuItemViewModel
            {
                Title = "Settings",
                Icon = Icon("Settings"),
                TargetContent = Content,
                RefreshCommand = new RelayCommand(() => {})
            },
            new LeftMenuItemViewModel
            {
                Title = "456",
                Icon = Icon("Settings"),
                TargetContent = PageContent<MainPage>()
            },
            new LeftMenuItemViewModel
            {
                Title = "789",
                Icon = Icon("Refresh")
            },
        ];
    }
}
