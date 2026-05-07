using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;

namespace FlowLauncher.Controls;

[PseudoClasses(":pressing", ":checked")]
public class FlowRadioButton : TemplatedControl
{
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

    public static readonly StyledProperty<bool> IsCheckedProperty =
        AvaloniaProperty.Register<FlowRadioButton, bool>(nameof(IsChecked));

    public bool IsChecked
    {
        get => GetValue(IsCheckedProperty);
        set => SetValue(IsCheckedProperty, value);
    }

    public static readonly StyledProperty<ICommand?> CommandProperty =
        AvaloniaProperty.Register<FlowRadioButton, ICommand?>(nameof(Command));

    public ICommand? Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public static readonly StyledProperty<object?> CommandParameterProperty =
        AvaloniaProperty.Register<FlowRadioButton, object?>(nameof(CommandParameter));

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

    static FlowRadioButton()
    {
        IsCheckedProperty.Changed.AddClassHandler<FlowRadioButton, bool>((sender, e) =>
        {
            sender.PseudoClasses.Set(":checked", e.NewValue.Value);
        });
        CommandProperty.Changed.AddClassHandler<FlowRadioButton, ICommand?>((sender, e) =>
        {
            if (e.OldValue is { HasValue: true, Value: not null })
                e.OldValue.Value.CanExecuteChanged -= sender.OnCanExecuteChanged;
            if (e.NewValue is { HasValue: true, Value: not null })
                e.NewValue.Value.CanExecuteChanged += sender.OnCanExecuteChanged;
        });
    }

    public static readonly RoutedEvent<RoutedEventArgs> ClickEvent =
        RoutedEvent.Register<FlowRadioButton, RoutedEventArgs>(nameof(Click), RoutingStrategies.Bubble);

    public event EventHandler<RoutedEventArgs>? Click
    {
        add => AddHandler(ClickEvent, value);
        remove => RemoveHandler(ClickEvent, value);
    }

    protected virtual void OnClick()
    {
        RaiseEvent(new RoutedEventArgs { RoutedEvent = ClickEvent });
        Command?.Execute(CommandParameter);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        if (!(IsEnabled && e.Properties.IsLeftButtonPressed)) return;
        PseudoClasses.Add(":pressing");
        e.Handled = true;
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
        if (PseudoClasses.Remove(":pressing") && IsEnabled) OnClick();
    }

    protected override void OnPointerExited(PointerEventArgs e)
    {
        base.OnPointerExited(e);
        PseudoClasses.Remove(":pressing");
    }
}
