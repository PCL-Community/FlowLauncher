using Avalonia;
using Avalonia.Media;

namespace FlowLauncher.Controls;

public class FlowIconButton : FlowClickable
{
    public static readonly StyledProperty<Geometry?> IconProperty =
        AvaloniaProperty.Register<FlowIconButton, Geometry?>(nameof(Icon));

    public Geometry? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly StyledProperty<Thickness> IconMarginProperty =
        AvaloniaProperty.Register<FlowIconButton, Thickness>(nameof(IconMargin));

    public Thickness IconMargin
    {
        get => GetValue(IconMarginProperty);
        set => SetValue(IconMarginProperty, value);
    }

    public static readonly StyledProperty<double> IconWidthProperty =
        AvaloniaProperty.Register<FlowIconButton, double>(nameof(IconWidth));

    public double IconWidth
    {
        get => GetValue(IconWidthProperty);
        set => SetValue(IconWidthProperty, value);
    }

    public static readonly StyledProperty<bool> IsClickBubbleVisibleProperty =
        AvaloniaProperty.Register<FlowIconButton, bool>(nameof(IsClickBubbleVisible));

    public bool IsClickBubbleVisible
    {
        get => GetValue(IsClickBubbleVisibleProperty);
        set => SetValue(IsClickBubbleVisibleProperty, value);
    }

    public static readonly StyledProperty<double> IconRotationAngleProperty =
        AvaloniaProperty.Register<FlowIconButton, double>(nameof(IconRotationAngle));

    public double IconRotationAngle
    {
        get => GetValue(IconRotationAngleProperty);
        set => SetValue(IconRotationAngleProperty, value);
    }

    public static readonly StyledProperty<double> IconScaleProperty =
        AvaloniaProperty.Register<FlowIconButton, double>(nameof(IconScale), 1);

    public double IconScale
    {
        get => GetValue(IconScaleProperty);
        set => SetValue(IconScaleProperty, value);
    }
}
