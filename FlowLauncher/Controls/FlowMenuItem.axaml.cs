using System.Collections.Specialized;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Metadata;

namespace FlowLauncher.Controls;

public class FlowMenuItem : FlowButton
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
        set
        {
            field.CollectionChanged -= OnExtraControlsCollectionChanged;
            SetAndRaise(ExtraControlsProperty, ref field, value);
            field.CollectionChanged += OnExtraControlsCollectionChanged;
            SyncDataContextToExtraControls();
        }
    } = [];

    static FlowMenuItem()
    {
        // ExtraControls items are re-parented by an inner ItemsControl whose ItemsPanel sits
        // in a deeply nested template, so DataContext inheritance via $parent[FlowMenuItem]
        // breaks (see FlowMenuItem.axaml). We explicitly forward DataContext here so callers
        // can keep writing plain {Binding SomeProp} against the original LeftMenuItemViewModel
        // (or whatever DataContext is on the FlowMenuItem itself).
        DataContextProperty.Changed.AddClassHandler<FlowMenuItem>((s, _) => s.SyncDataContextToExtraControls());
    }

    public FlowMenuItem()
    {
        ExtraControls.CollectionChanged += OnExtraControlsCollectionChanged;
    }

    private void OnExtraControlsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.NewItems is null) return;
        foreach (Control control in e.NewItems) control.DataContext = DataContext;
    }

    private void SyncDataContextToExtraControls()
    {
        foreach (var control in ExtraControls) control.DataContext = DataContext;
    }
}
