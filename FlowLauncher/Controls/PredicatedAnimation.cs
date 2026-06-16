using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Collections;
using Avalonia.Styling;

namespace FlowLauncher.Controls;

public class PredicatedAnimation : AvaloniaObject
{
    private readonly ConditionalWeakTable<Animatable, AnimationState> _states = new();

    public static readonly StyledProperty<TimeSpan> PredicateDurationProperty =
        AvaloniaProperty.Register<PredicatedAnimation, TimeSpan>(nameof(PredicateDuration));

    public TimeSpan PredicateDuration
    {
        get => GetValue(PredicateDurationProperty);
        set => SetValue(PredicateDurationProperty, value);
    }

    public static readonly StyledProperty<TimeSpan> RestoreDurationProperty =
        AvaloniaProperty.Register<PredicatedAnimation, TimeSpan>(nameof(RestoreDuration));

    public TimeSpan RestoreDuration
    {
        get => GetValue(RestoreDurationProperty);
        set => SetValue(RestoreDurationProperty, value);
    }

    public static readonly StyledProperty<TimeSpan> ContinueDurationProperty =
        AvaloniaProperty.Register<PredicatedAnimation, TimeSpan>(nameof(ContinueDuration));

    public TimeSpan ContinueDuration
    {
        get => GetValue(ContinueDurationProperty);
        set => SetValue(ContinueDurationProperty, value);
    }

    public static readonly StyledProperty<Easing?> PredicateEasingProperty =
        AvaloniaProperty.Register<PredicatedAnimation, Easing?>(nameof(PredicateEasing));

    public Easing? PredicateEasing
    {
        get => GetValue(PredicateEasingProperty);
        set => SetValue(PredicateEasingProperty, value);
    }

    public static readonly StyledProperty<Easing?> RestoreEasingProperty =
        AvaloniaProperty.Register<PredicatedAnimation, Easing?>(nameof(RestoreEasing));

    public Easing? RestoreEasing
    {
        get => GetValue(RestoreEasingProperty);
        set => SetValue(RestoreEasingProperty, value);
    }

    public static readonly StyledProperty<Easing?> ContinueEasingProperty =
        AvaloniaProperty.Register<PredicatedAnimation, Easing?>(nameof(ContinueEasing));

    public Easing? ContinueEasing
    {
        get => GetValue(ContinueEasingProperty);
        set => SetValue(ContinueEasingProperty, value);
    }

    public KeyFrames PredicateFrames { get; } = [];
    public KeyFrames RestoreFrames { get; } = [];
    public KeyFrames ContinueFrames { get; } = [];

    private AvaloniaList<IAnimationSetter> _ReturnOrAllocateFrame(
        ref AvaloniaList<IAnimationSetter>? field, KeyFrames targetFrames)
    {
        if (field != null) return field;
        var frame = new KeyFrame { Cue = new Cue(1) };
        targetFrames.Add(frame);
        return field = frame.Setters;
    }

    public AvaloniaList<IAnimationSetter> PredicateFrame { get => _ReturnOrAllocateFrame(ref field, PredicateFrames); } = null!;
    public AvaloniaList<IAnimationSetter> RestoreFrame { get => _ReturnOrAllocateFrame(ref field, RestoreFrames); } = null!;
    public AvaloniaList<IAnimationSetter> ContinueFrame { get => _ReturnOrAllocateFrame(ref field, ContinueFrames); } = null!;

    // ReSharper disable once CollectionNeverUpdated.Global
    public AvaloniaList<Setter> Completion { get; } = [];

    public void Cancel(Animatable target)
    {
        if (!_states.TryGetValue(target, out var state)) return;
        state.TokenSource?.Cancel();
        state.TokenSource?.Dispose();
        state.TokenSource = null;
    }

    private void ApplyCompletionSetters(Animatable target)
    {
        foreach (var setter in Completion)
        {
            if (setter.Property is null) continue;
            target.SetValue(setter.Property, setter.Value);
        }
    }

    private async Task Run(Animatable target, KeyFrames keyFrames, TimeSpan duration, Easing? easing, bool applyCompletionSetters = false)
    {
        Cancel(target);
        var state = _states.GetOrCreateValue(target);
        var tokenSource = new CancellationTokenSource();
        state.TokenSource = tokenSource;

        try
        {
            var animation = new Animation
            {
                Duration = duration,
                FillMode = FillMode.Forward
            };
            if (easing is not null) animation.Easing = easing;

            foreach (var keyFrame in keyFrames) animation.Children.Add(keyFrame);
            await animation.RunAsync(target, tokenSource.Token);

            if (state.TokenSource != tokenSource || tokenSource.IsCancellationRequested) return;
            if (applyCompletionSetters) ApplyCompletionSetters(target);
        }
        catch (OperationCanceledException) { }
        finally
        {
            if (state.TokenSource == tokenSource)
                state.TokenSource = null;

            tokenSource.Dispose();
        }
    }

    public Task Predicate(Animatable target) => Run(target, PredicateFrames, PredicateDuration, PredicateEasing);
    public Task Restore(Animatable target) => Run(target, RestoreFrames, RestoreDuration, RestoreEasing);
    public Task Continue(Animatable target) => Run(target, ContinueFrames, ContinueDuration, ContinueEasing, true);

    private sealed class AnimationState
    {
        public CancellationTokenSource? TokenSource { get; set; }
    }
}
