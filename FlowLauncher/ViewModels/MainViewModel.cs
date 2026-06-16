using FlowLauncher.Components.UI;
using FlowLauncher.Resources;
using FlowLauncher.Views;

namespace FlowLauncher.ViewModels;

public partial class MainViewModel : PageViewModel<MainPage>
{
    public MainViewModel() : base("main", Strings.PageTitleMain)
    {
        LeftExtraContent = ReferContent<MainPageLeft>();
    }
}
