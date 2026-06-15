using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace FlowLauncher.Controls;

[PseudoClasses(":pressing", ":checked")]
public abstract class FlowClickable : TemplatedControl
{
    public static readonly StyledProperty<bool> IsCheckedProperty =
        AvaloniaProperty.Register<FlowButton, bool>(nameof(IsChecked));

    public bool IsChecked
    {
        get => GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }

    public static readonly DirectProperty<FlowClickable, bool> IsPressingProperty =
        AvaloniaProperty.RegisterDirect<FlowClickable, bool>(nameof(IsPressing), i => i.IsPressing);

    public bool IsPressing { get; private set => SetAndRaise(IsPressingProperty, ref field, value); } = false;

    public static readonly StyledProperty<double> BackgroundOpacityProperty =
        AvaloniaProperty.Register<FlowButton, double>(nameof(BackgroundOpacity));

    public double BackgroundOpacity
    {
        get => GetValue(BackgroundOpacityProperty);
        set => SetValue(BackgroundOpacityProperty, value);
    }

    public static readonly StyledProperty<double?> PriorityBackgroundOpacityProperty =
        AvaloniaProperty.Register<FlowClickable, double?>(nameof(PriorityBackgroundOpacity));

    public double? PriorityBackgroundOpacity
    {
        get => GetValue(PriorityBackgroundOpacityProperty);
        set => SetValue(PriorityBackgroundOpacityProperty, value);
    }

    public static readonly StyledProperty<IControlTemplate?> ContentTemplateProperty =
        AvaloniaProperty.Register<FlowClickable, IControlTemplate?>(nameof(ContentTemplate));

    public IControlTemplate? ContentTemplate
    {
        get => GetValue(ContentTemplateProperty);
        set => SetValue(ContentTemplateProperty, value);
    }

    public static readonly StyledProperty<PredicatedAnimation?> PredicatedAnimationProperty =
        AvaloniaProperty.Register<FlowClickable, PredicatedAnimation?>(nameof(PredicatedAnimation));

    public PredicatedAnimation? PredicatedAnimation
    {
        get => GetValue(PredicatedAnimationProperty);
        set => SetValue(PredicatedAnimationProperty, value);
    }

    public static readonly StyledProperty<ICommand?> CommandProperty =
        AvaloniaProperty.Register<FlowClickable, ICommand?>(nameof(Command));

    public ICommand? Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public static readonly StyledProperty<object?> CommandParameterProperty =
        AvaloniaProperty.Register<FlowClickable, object?>(nameof(CommandParameter));

    public object? CommandParameter
    {
        get => GetValue(CommandParameterProperty);
        set => SetValue(CommandParameterProperty, value);
    }

    private void OnCanExecuteChanged(object? sender, EventArgs? e)
    {
        if (Command == null) return;
        IsEnabled = Command.CanExecute(CommandParameter);
    }

    static FlowClickable()
    {
        IsCheckedProperty.Changed.AddClassHandler<FlowClickable, bool>((sender, e) =>
            sender.PseudoClasses.Set(":checked", e.NewValue.Value));
        IsPressingProperty.Changed.AddClassHandler<FlowClickable, bool>((sender, e) =>
            sender.PseudoClasses.Set(":pressing", e.NewValue.Value));
        CommandProperty.Changed.AddClassHandler<FlowClickable, ICommand?>((sender, e) =>
        {
            if (e.OldValue is { HasValue: true, Value: not null })
                e.OldValue.Value.CanExecuteChanged -= sender.OnCanExecuteChanged;
            if (e.NewValue is { HasValue: true, Value: not null })
                e.NewValue.Value.CanExecuteChanged += sender.OnCanExecuteChanged;
        });
    }

    public static readonly RoutedEvent<RoutedEventArgs> ClickEvent =
        RoutedEvent.Register<FlowClickable, RoutedEventArgs>(nameof(Click), RoutingStrategies.Direct);

    public event EventHandler<RoutedEventArgs>? Click
    {
        add => AddHandler(ClickEvent, value);
        remove => RemoveHandler(ClickEvent, value);
    }

    public void InjectPointerPressed(PointerPressedEventArgs e) => OnPointerPressed(e);
    public void InjectPointerReleased(PointerReleasedEventArgs e) => OnPointerReleased(e);
    public void InjectPointerExited(PointerEventArgs e) => OnPointerExited(e);

    protected virtual void OnClick()
    {
        RaiseEvent(new RoutedEventArgs { RoutedEvent = ClickEvent });
        Command?.Execute(CommandParameter);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (!(IsEnabled && e.Properties.IsLeftButtonPressed)) return;
        IsPressing = true;
        e.Handled = true;
        PredicatedAnimation?.Predicate(this);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        var wasPredictiveAnimationActive = IsPressing;
        var shouldCompleteAnimation = wasPredictiveAnimationActive && IsEnabled;

        base.OnPointerReleased(e);
        if (IsPressing)
        {
            IsPressing = false;
            e.Handled = true;
            if (IsEnabled) OnClick();
        }

        if (shouldCompleteAnimation) PredicatedAnimation?.Continue(this);
        else if (wasPredictiveAnimationActive) PredicatedAnimation?.Restore(this);
    }

    protected override void OnPointerExited(PointerEventArgs e)
    {
        var shouldRestoreAnimation = IsPressing;

        base.OnPointerExited(e);
        IsPressing = false;

        if (shouldRestoreAnimation) PredicatedAnimation?.Restore(this);
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        PredicatedAnimation?.Cancel(this);
    }
}
