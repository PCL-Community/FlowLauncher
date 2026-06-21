using FlowLauncher.Components.FlowIntegration;
using FlowLauncher.Components.UI;
using FlowLauncher.Resources;
using FlowLauncher.Views;

namespace FlowLauncher.ViewModels;

[FirstLoadingPage]
public partial class MainViewModel : PageViewModel<MainPage>
{
    public MainViewModel() : base("main", Strings.PageTitleMain)
    {
        LeftExtraContent = ReferContent<MainPageLeft>();
    }
}
