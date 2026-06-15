using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Metadata;

namespace FlowLauncher.Controls;

public class FlowCard : Border
{
    public static readonly StyledProperty<IControlTemplate?> DecorationTemplateProperty =
        AvaloniaProperty.Register<FlowCard, IControlTemplate?>(nameof(DecorationTemplate));

    public IControlTemplate? DecorationTemplate
    {
        get => GetValue(DecorationTemplateProperty);
        set => SetValue(DecorationTemplateProperty, value);
    }

    public static readonly StyledProperty<IControlTemplate?> ContentTemplateProperty =
        AvaloniaProperty.Register<FlowCard, IControlTemplate?>(nameof(ContentTemplate));

    public IControlTemplate? ContentTemplate
    {
        get => GetValue(ContentTemplateProperty);
        set => SetValue(ContentTemplateProperty, value);
    }

    public static readonly StyledProperty<Control?> ContentProperty =
        AvaloniaProperty.Register<FlowCard, Control?>(nameof(Content));

    [Content]
    public Control? Content
    {
        get => GetValue(ContentProperty);
        set => SetValue(ContentProperty, value);
    }

    public static readonly StyledProperty<Thickness> ContentMarginProperty =
        AvaloniaProperty.Register<FlowCard, Thickness>(nameof(ContentMargin));

    public Thickness ContentMargin
    {
        get => GetValue(ContentMarginProperty);
        set => SetValue(ContentMarginProperty, value);
    }

    public static readonly StyledProperty<HorizontalAlignment> DecorationHorizontalAlignmentProperty =
        AvaloniaProperty.Register<FlowCard, HorizontalAlignment>(nameof(DecorationHorizontalAlignment));

    public HorizontalAlignment DecorationHorizontalAlignment
    {
        get => GetValue(DecorationHorizontalAlignmentProperty);
        set => SetValue(DecorationHorizontalAlignmentProperty, value);
    }

    public static readonly StyledProperty<VerticalAlignment> DecorationVerticalAlignmentProperty =
        AvaloniaProperty.Register<FlowCard, VerticalAlignment>(nameof(DecorationVerticalAlignment));

    public VerticalAlignment DecorationVerticalAlignment
    {
        get => GetValue(DecorationVerticalAlignmentProperty);
        set => SetValue(DecorationVerticalAlignmentProperty, value);
    }

    public static readonly StyledProperty<Color> ShadowProperty =
        AvaloniaProperty.Register<FlowCard, Color>(nameof(Shadow));

    public Color Shadow
    {
        get => GetValue(ShadowProperty);
        set => SetValue(ShadowProperty, value);
    }

    private void UpdateBoxShadow()
    {
        BoxShadow = new BoxShadows(new BoxShadow
        {
            Color = Shadow,
            Blur = 5,
            Spread = .625
        });
    }

    static FlowCard()
    {
        ShadowProperty.Changed.AddClassHandler<FlowCard, Color>((sender, e) =>
        {
            if (!e.NewValue.HasValue) return;
            sender.UpdateBoxShadow();
        });
    }
}
