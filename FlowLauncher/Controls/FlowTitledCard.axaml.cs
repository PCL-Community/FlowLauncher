using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.VisualTree;
using CommunityToolkit.Mvvm.Input;

namespace FlowLauncher.Controls;

public partial class FlowTitledCard : FlowCard
{
    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<FlowTitledCard, string?>(nameof(Title));

    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly StyledProperty<bool> CanFoldProperty =
        AvaloniaProperty.Register<FlowTitledCard, bool>(nameof(CanFold), true);

    public bool CanFold
    {
        get => GetValue(CanFoldProperty);
        set => SetValue(CanFoldProperty, value);
    }

    public static readonly StyledProperty<bool> IsFoldedProperty =
        AvaloniaProperty.Register<FlowTitledCard, bool>(nameof(IsFolded));

    public bool IsFolded
    {
        get => GetValue(IsFoldedProperty);
        set => SetValue(IsFoldedProperty, value);
    }

    public static readonly StyledProperty<IBrush?> TitleBarForegroundProperty =
        AvaloniaProperty.Register<FlowTitledCard, IBrush?>(nameof(TitleBarForeground));

    public IBrush? TitleBarForeground
    {
        get => GetValue(TitleBarForegroundProperty);
        set => SetValue(TitleBarForegroundProperty, value);
    }

    [RelayCommand]
    private void ClickTitleBar()
    {
        if (!CanFold) return;
        IsFolded = !IsFolded;
    }

    static FlowTitledCard()
    {
        CanFoldProperty.Changed.AddClassHandler<FlowTitledCard, bool>((sender, e) =>
        {
            // unfold if folding is disabled
            if (!e.NewValue.Value && sender.IsFolded) sender.IsFolded = false;
        });
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        var descendants = this.GetVisualDescendants();
        var titleBarArea = descendants.OfType<Border>().First(x => x.Name == "TitleBarArea");
        var titleBarPullDownButton = descendants.OfType<FlowIconButton>().First(x => x.Name == "TitleBarPullDownButton");
        titleBarArea.PointerPressed += (_, e1) => titleBarPullDownButton.InjectPointerPressed(e1);
        titleBarArea.PointerReleased += (_, e1) => titleBarPullDownButton.InjectPointerReleased(e1);
        titleBarArea.PointerExited += (_, e1) => titleBarPullDownButton.InjectPointerExited(e1);
    }
}
