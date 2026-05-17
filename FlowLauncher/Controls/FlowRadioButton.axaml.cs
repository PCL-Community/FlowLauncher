using Avalonia;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;

namespace FlowLauncher.Controls;

public class FlowRadioButton : FlowButton
{
    public FlowRadioButton Self => this;

    public static readonly StyledProperty<string> TextProperty =
        AvaloniaProperty.Register<FlowRadioButton, string>(nameof(Text));

    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly StyledProperty<Geometry?> IconProperty =
        AvaloniaProperty.Register<FlowRadioButton, Geometry?>(nameof(Icon));

    public Geometry? Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly StyledProperty<Thickness> IconMarginProperty =
        AvaloniaProperty.Register<FlowRadioButton, Thickness>(nameof(IconMargin));

    public Thickness IconMargin
    {
        get => GetValue(IconMarginProperty);
        set => SetValue(IconMarginProperty, value);
    }

    public static readonly StyledProperty<double> IconWidthProperty =
        AvaloniaProperty.Register<FlowRadioButton, double>(nameof(IconWidth));

    public double IconWidth
    {
        get => GetValue(IconWidthProperty);
        set => SetValue(IconWidthProperty, value);
    }

    public static readonly StyledProperty<IBrush?> PressingForegroundProperty =
        AvaloniaProperty.Register<FlowRadioButton, IBrush?>(nameof(PressingForeground));

    public IBrush? PressingForeground
    {
        get => GetValue(PressingForegroundProperty);
        set => SetValue(PressingForegroundProperty, value);
    }

    public static readonly StyledProperty<double> PressingBackgroundOpacityProperty =
        AvaloniaProperty.Register<FlowRadioButton, double>(nameof(PressingBackgroundOpacity), 1);

    public double PressingBackgroundOpacity
    {
        get => GetValue(PressingBackgroundOpacityProperty);
        set => SetValue(PressingBackgroundOpacityProperty, value);
    }

    public static readonly StyledProperty<double> PointerOverBackgroundOpacityProperty =
        AvaloniaProperty.Register<FlowRadioButton, double>(nameof(PointerOverBackgroundOpacity), .3);

    public double PointerOverBackgroundOpacity
    {
        get => GetValue(PointerOverBackgroundOpacityProperty);
        set => SetValue(PointerOverBackgroundOpacityProperty, value);
    }

    public static readonly StyledProperty<double> BackgroundOpacityProperty =
        AvaloniaProperty.Register<FlowRadioButton, double>(nameof(BackgroundOpacity), .1);

    public double BackgroundOpacity
    {
        get => GetValue(BackgroundOpacityProperty);
        set => SetValue(BackgroundOpacityProperty, value);
    }

    public static readonly StyledProperty<HorizontalAlignment> TextAlignmentProperty =
        AvaloniaProperty.Register<FlowRadioButton, HorizontalAlignment>(nameof(TextAlignment), HorizontalAlignment.Center);

    public HorizontalAlignment TextAlignment
    {
        get => GetValue(TextAlignmentProperty);
        set => SetValue(TextAlignmentProperty, value);
    }

    public static readonly StyledProperty<IControlTemplate?> RightAreaTemplateProperty =
        AvaloniaProperty.Register<FlowRadioButton, IControlTemplate?>(nameof(RightAreaTemplate));

    public IControlTemplate? RightAreaTemplate
    {
        get => GetValue(RightAreaTemplateProperty);
        set => SetValue(RightAreaTemplateProperty, value);
    }
}
