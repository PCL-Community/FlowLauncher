using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FlowLauncher.Components.FlowIntegration;
using FlowLauncher.Components.UI;
using FlowLauncher.Views;

namespace FlowLauncher.ViewModels;

/// <summary>
/// 用于演示自定义控件的测试页面视图模型
/// </summary>
[FirstLoadingPage]
public partial class TestViewModel : PageViewModel<TestPage>
{
    private bool _isSimulating;
    private int _simulationVersion;
    private int _smallTextVariant;

    private static readonly string[] SmallTextVariants = [
        "Click to start",
        "Alternative subtitle",
        "A longer subtitle for layout testing"
    ];

    /// <summary>
    /// 获取当前模拟进度
    /// </summary>
    [ObservableProperty]
    public partial double Progress { get; private set; }

    /// <summary>
    /// 获取进度按钮的主文本
    /// </summary>
    [ObservableProperty]
    public partial string ProgressText { get; private set; } = "Simulate progress";

    /// <summary>
    /// 获取进度按钮的次级文本
    /// </summary>
    [ObservableProperty]
    public partial string SmallProgressText { get; private set; } = "Click to start";

    /// <summary>
    /// 获取是否显示进度按钮的次级文本
    /// </summary>
    [ObservableProperty]
    public partial bool IsSmallTextVisible { get; private set; } = true;

    /// <summary>
    /// 获取当前应显示的次级文本
    /// </summary>
    public string? DisplayedSmallProgressText => IsSmallTextVisible ? SmallProgressText : null;

    public TestViewModel() : base("test", "Test") { }

    [RelayCommand]
    private async Task SimulateProgressAsync()
    {
        if (_isSimulating) return;

        _isSimulating = true;
        var simulationVersion = ++_simulationVersion;
        Progress = 0;
        ProgressText = "Simulating progress";

        try
        {
            for (var step = 1; step <= 20; step++)
            {
                await Task.Delay(50);
                if (simulationVersion != _simulationVersion) return;
                Progress = step / 20d;
                SmallProgressText = $"{Progress:P0} complete";
            }

            ProgressText = "Complete";
            SmallProgressText = "Click to run again";
        }
        finally
        {
            if (simulationVersion == _simulationVersion) _isSimulating = false;
        }
    }

    [RelayCommand]
    private void ResetProgress()
    {
        _simulationVersion++;
        _isSimulating = false;
        Progress = 0;
        ProgressText = "Simulate progress";
        SmallProgressText = "Click to start";
    }

    [RelayCommand]
    private void ChangeSmallText()
    {
        _smallTextVariant = (_smallTextVariant + 1) % SmallTextVariants.Length;
        SmallProgressText = SmallTextVariants[_smallTextVariant];
    }

    [RelayCommand]
    private void ToggleSmallText() => IsSmallTextVisible = !IsSmallTextVisible;

    partial void OnSmallProgressTextChanged(string value) => OnPropertyChanged(nameof(DisplayedSmallProgressText));

    partial void OnIsSmallTextVisibleChanged(bool value) => OnPropertyChanged(nameof(DisplayedSmallProgressText));
}
