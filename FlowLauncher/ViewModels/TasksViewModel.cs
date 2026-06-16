using CommunityToolkit.Mvvm.ComponentModel;
using FlowLauncher.Components.UI;
using FlowLauncher.Resources;
using FlowLauncher.Views;

namespace FlowLauncher.ViewModels;

public partial class TasksViewModel() : PageViewModel<TasksPage>("tasks", Strings.PageTitleTasks)
{
    [ObservableProperty] public partial string? CurrentTaskId { get; private set; }

    private void RefreshTaskList()
    {
        // TODO
    }

    protected override void OnEnterPage()
    {
        base.OnLeavePage();
        // refresh task list when enter page
        RefreshTaskList();
    }
}
