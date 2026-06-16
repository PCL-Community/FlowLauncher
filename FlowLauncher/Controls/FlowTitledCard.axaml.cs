using Avalonia;
using Avalonia.Animation.Easings;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;
using Avalonia.VisualTree;
using CommunityToolkit.Mvvm.Input;

namespace FlowLauncher.Controls;

public partial class FlowTitledCard : FlowCard
{
    private const double DefaultTitleBarHeight = 40;
    private const double PressOffset = 10;

    private Border? _titleBarArea;
    private PredicatedAnimation? _foldAnimation;
    private Setter? _predicateHeightSetter;
    private Setter? _restoreHeightSetter;
    private Setter? _continueHeightSetter;
    private Setter? _completionHeightSetter;

    // User-set Height captured before the latest press; double.NaN means "auto" was in effect.
    private double _heightBeforePress = double.NaN;
    // User-set Height captured before the latest fold; restored on the unfold animation.
    private double _userHeightSnapshot = double.NaN;
    private bool _isTitleBarPressing;
    private int _animationSeq;

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

        IsFoldedProperty.Changed.AddClassHandler<FlowTitledCard, bool>((sender, e) =>
        {
            // Plays a direct (non-predictive) fold/unfold animation.
            sender.TriggerContinueAnimation(e.NewValue.Value);
        });
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);
        var descendants = this.GetVisualDescendants();
        var titleBarArea = descendants.OfType<Border>().First(x => x.Name == "TitleBarArea");
        var titleBarPullDownButton = descendants.OfType<FlowIconButton>().First(x => x.Name == "TitleBarPullDownButton");
        _titleBarArea = titleBarArea;
        InitializeFoldAnimation();

        titleBarArea.PointerPressed += (_, e1) =>
        {
            titleBarPullDownButton.InjectPointerPressed(e1);
            if (!CanFold || !e1.Properties.IsLeftButtonPressed) return;
            TriggerPredicateAnimation();
        };
        titleBarArea.PointerReleased += (_, e1) =>
        {
            titleBarPullDownButton.InjectPointerReleased(e1);
            _isTitleBarPressing = false;
        };
        titleBarArea.PointerExited += (_, e1) =>
        {
            titleBarPullDownButton.InjectPointerExited(e1);
            if (!_isTitleBarPressing) return;
            _isTitleBarPressing = false;
            TriggerRestoreAnimation();
        };

        // Apply initial folded state without animation, remembering the user-set Height.
        if (IsFolded)
        {
            _userHeightSnapshot = Height;
            Height = GetFoldedHeight();
        }
    }

    private double GetFoldedHeight()
    {
        if (_titleBarArea is { Bounds.Height: > 0 } area) return area.Bounds.Height;
        return DefaultTitleBarHeight;
    }

    private double GetNaturalHeight()
    {
        var saved = Height;
        Height = double.NaN;
        var availableWidth = Bounds.Width > 0 ? Bounds.Width : double.PositiveInfinity;
        Measure(new Size(availableWidth, double.PositiveInfinity));
        var natural = DesiredSize.Height;
        Height = saved;
        return natural > 0 ? natural : Bounds.Height;
    }

    private double GetCurrentHeight()
    {
        if (double.IsFinite(Height)) return Height;
        return Bounds.Height;
    }

    private void InitializeFoldAnimation()
    {
        if (_foldAnimation is not null) return;

        _predicateHeightSetter = new Setter(HeightProperty, 0.0);
        _restoreHeightSetter = new Setter(HeightProperty, 0.0);
        _continueHeightSetter = new Setter(HeightProperty, 0.0);
        _completionHeightSetter = new Setter(HeightProperty, double.NaN);

        _foldAnimation = new PredicatedAnimation
        {
            PredicateDuration = TimeSpan.FromMilliseconds(70),
            RestoreDuration = TimeSpan.FromMilliseconds(80),
            ContinueDuration = TimeSpan.FromMilliseconds(300),
            // Mirror the global SpringEasing resource (Resources/Values.axaml).
            ContinueEasing = new SpringEasing(1.15, 100, 15),
        };
        _foldAnimation.PredicateFrame.Add(_predicateHeightSetter);
        _foldAnimation.RestoreFrame.Add(_restoreHeightSetter);
        _foldAnimation.ContinueFrame.Add(_continueHeightSetter);
        _foldAnimation.Completion.Add(_completionHeightSetter);
    }

    private void TriggerPredicateAnimation()
    {
        if (_foldAnimation is null || _predicateHeightSetter is null || _restoreHeightSetter is null) return;

        var seq = ++_animationSeq;
        _isTitleBarPressing = true;
        // Capture user-set Height (preserve double.NaN to mean "auto").
        _heightBeforePress = Height;

        var current = GetCurrentHeight();
        if (current <= 0) return;

        // Animation needs a finite start value; NaN can't be interpolated.
        if (!double.IsFinite(Height)) Height = current;

        // Folded => predict unfold (grow); unfolded => predict fold (shrink).
        var predicted = IsFolded ? current + PressOffset : current - PressOffset;
        _predicateHeightSetter.Value = Math.Max(0, predicted);
        _restoreHeightSetter.Value = current;

        _ = _foldAnimation.Predicate(this);
        _ = seq; // bumped to invalidate any pending Restore continuation
    }

    private void TriggerRestoreAnimation()
    {
        _ = RunRestoreAsync();
    }

    private async Task RunRestoreAsync()
    {
        if (_foldAnimation is null || _restoreHeightSetter is null) return;

        var seq = ++_animationSeq;
        var wasAuto = double.IsNaN(_heightBeforePress);
        _restoreHeightSetter.Value = wasAuto ? GetCurrentHeight() : _heightBeforePress;

        await _foldAnimation.Restore(this);

        // Skip post-process if a newer animation has started.
        if (seq != _animationSeq) return;
        // Restore "auto" sizing when the user originally had no explicit Height.
        if (wasAuto) Height = double.NaN;
    }

    private void TriggerContinueAnimation(bool folded)
    {
        if (_foldAnimation is null || _continueHeightSetter is null || _completionHeightSetter is null) return;

        ++_animationSeq;

        if (folded)
        {
            // Snapshot the user-set Height so we can restore it when unfolding.
            // During a click flow, Height is mid-predicate; use the pre-press value instead.
            _userHeightSnapshot = _isTitleBarPressing ? _heightBeforePress : Height;
        }
        _isTitleBarPressing = false;

        _foldAnimation.Cancel(this);
        // Animation needs a finite start value.
        if (!double.IsFinite(Height))
        {
            Height = Bounds.Height > 0 ? Bounds.Height : GetFoldedHeight();
        }

        double target;
        object completionValue;
        if (folded)
        {
            target = GetFoldedHeight();
            completionValue = target;
        }
        else if (double.IsFinite(_userHeightSnapshot))
        {
            // Restore to the originally-set Height.
            target = _userHeightSnapshot;
            completionValue = _userHeightSnapshot;
        }
        else
        {
            // No snapshot (was auto-sized): animate to the natural height, then drop back to auto.
            target = GetNaturalHeight();
            completionValue = double.NaN;
        }

        _continueHeightSetter.Value = target;
        _completionHeightSetter.Value = completionValue;

        _ = _foldAnimation.Continue(this);
    }
}
