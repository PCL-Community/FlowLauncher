using Avalonia;
using Avalonia.Metadata;

namespace FlowLauncher.Controls;

public class FlowMenuItem : FlowRadioButton
{
    public static readonly DirectProperty<FlowMenuItem, Avalonia.Controls.Controls> ExtraControlsProperty =
        AvaloniaProperty.RegisterDirect<FlowMenuItem, Avalonia.Controls.Controls>(
            nameof(ExtraControls),
            i => i.ExtraControls,
            (i, v) => i.ExtraControls = v
        );

    [Content]
    public Avalonia.Controls.Controls ExtraControls
    {
        get;
        set => SetAndRaise(ExtraControlsProperty, ref field, value);
    } = [];
}
