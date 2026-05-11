using FlowLauncher.Resources;
using FlowLauncher.Views;

namespace FlowLauncher.ViewModels;

public partial class MainPageViewModel : PageViewModel
{
    public MainPageViewModel() : base("main", Strings.PageTitleMain)
    {
        LeftMenuItems = [
            new MenuTitleViewModel
            {
                Title = "title"
            },
            new MenuItemViewModel
            {
                Title = "123"
            },
            new MenuItemViewModel
            {
                Title = "456"
            },
            new MenuItemViewModel
            {
                Title = "789"
            },
        ];
        Content = new MainPage { DataContext = this };
    }
}
