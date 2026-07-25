using Avalonia;

namespace FlowLauncher.Controls;

public class FlowProgressButton : FlowClickable
{
    /// <summary>
    /// 获取或设置主文本
    /// </summary>
    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<FlowProgressButton, string?>(nameof(Text));

    /// <summary>
    /// 获取或设置主文本
    /// </summary>
    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>
    /// 获取或设置次级小文本
    /// </summary>
    public static readonly StyledProperty<string?> SmallTextProperty =
        AvaloniaProperty.Register<FlowProgressButton, string?>(nameof(SmallText));

    /// <summary>
    /// 获取或设置次级小文本
    /// </summary>
    public string? SmallText
    {
        get => GetValue(SmallTextProperty);
        set => SetValue(SmallTextProperty, value);
    }

    /// <summary>
    /// 获取或设置进度, 有效范围为 0.0 到 1.0
    /// </summary>
    public static readonly StyledProperty<double> ProgressProperty =
        AvaloniaProperty.Register<FlowProgressButton, double>(nameof(Progress));

    /// <summary>
    /// 获取或设置进度, 有效范围为 0.0 到 1.0
    /// </summary>
    public double Progress
    {
        get => GetValue(ProgressProperty);
        set => SetValue(ProgressProperty, value);
    }

    /// <summary>
    /// 获取或设置预测进度, 用于按压反馈动画
    /// </summary>
    public static readonly StyledProperty<double> PredictiveProgressProperty =
        AvaloniaProperty.Register<FlowProgressButton, double>(nameof(PredictiveProgress));

    /// <summary>
    /// 获取或设置预测进度, 用于按压反馈动画
    /// </summary>
    public double PredictiveProgress
    {
        get => GetValue(PredictiveProgressProperty);
        set => SetValue(PredictiveProgressProperty, value);
    }

    /// <summary>
    /// 获取或设置预测进度层的不透明度
    /// </summary>
    public static readonly StyledProperty<double> PredictiveOpacityProperty =
        AvaloniaProperty.Register<FlowProgressButton, double>(nameof(PredictiveOpacity));

    /// <summary>
    /// 获取或设置预测进度层的不透明度
    /// </summary>
    public double PredictiveOpacity
    {
        get => GetValue(PredictiveOpacityProperty);
        set => SetValue(PredictiveOpacityProperty, value);
    }
}
