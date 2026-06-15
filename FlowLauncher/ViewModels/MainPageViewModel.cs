using FlowLauncher.Components.UI;
using FlowLauncher.Resources;
using FlowLauncher.Views;

namespace FlowLauncher.ViewModels;

public partial class MainPageViewModel : PageViewModel<MainPage>
{
    public MainPageViewModel() : base("main", Strings.PageTitleMain)
    {
        LeftExtraContent = ReferContent<MainPageLeft>();
    }
}
