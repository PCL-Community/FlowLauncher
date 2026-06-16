using System.Runtime.CompilerServices;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Collections;
using Avalonia.Styling;

namespace FlowLauncher.Controls;

public class PredicatedAnimation : AvaloniaObject
{
    // Per-target state, keyed by the animation target.
    //
    // STATIC on purpose: a click cycle is `Predicate` (on press) followed by
    // `Continue`/`Restore` (on release). Between those two calls, the styling
    // system can swap the `PredicatedAnimation` instance assigned to a control
    // (e.g. when the click handler toggles a property that drives the style
    // selector). If the state lived on the instance, the new instance wouldn't
    // see the session opened by the old one and `Continue`/`Restore` would
    // play with the wrong (new style's) KeyFrames - visually a reversed or
    // mis-targeted animation. Keying by target instead makes the session
    // survive instance swaps.
    private static readonly ConditionalWeakTable<Animatable, AnimationState> _states = new();

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

    public void Cancel(Animatable target) => CancelInternal(target, clearSession: true);

    private static void CancelInternal(Animatable target, bool clearSession)
    {
        if (!_states.TryGetValue(target, out var state)) return;
        state.TokenSource?.Cancel();
        state.TokenSource?.Dispose();
        state.TokenSource = null;
        if (clearSession) state.Session = null;
    }

    private static void ApplyCompletionSetters(Animatable target, IReadOnlyList<Setter> setters)
    {
        foreach (var setter in setters)
        {
            if (setter.Property is null) continue;
            target.SetValue(setter.Property, setter.Value);
        }
    }

    private static async Task Run(
        Animatable target,
        KeyFrames keyFrames,
        TimeSpan duration,
        Easing? easing,
        IReadOnlyList<Setter>? completionSetters)
    {
        // Cancel only the in-flight animation; KEEP the session intact because
        // the caller (Predicate/Continue/Restore) has just opened/consumed it
        // and we must not wipe what we're about to use.
        CancelInternal(target, clearSession: false);

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
            if (completionSetters is { Count: > 0 }) ApplyCompletionSetters(target, completionSetters);
        }
        catch (OperationCanceledException) { }
        finally
        {
            if (state.TokenSource == tokenSource)
                state.TokenSource = null;

            tokenSource.Dispose();
        }
    }

    public Task Predicate(Animatable target)
    {
        // Open a session: snapshot the data the upcoming Continue/Restore will
        // need. This freezes the values against any later instance swap caused
        // by style re-evaluation between press and release.
        var state = _states.GetOrCreateValue(target);
        state.Session = new SessionSnapshot(
            RestoreFrames, RestoreDuration, RestoreEasing,
            ContinueFrames, ContinueDuration, ContinueEasing,
            SnapshotCompletion(Completion));
        return Run(target, PredicateFrames, PredicateDuration, PredicateEasing, completionSetters: null);
    }

    public Task Restore(Animatable target)
    {
        var session = TakeSession(target);
        return session is not null
            ? Run(target, session.RestoreFrames, session.RestoreDuration, session.RestoreEasing, completionSetters: null)
            : Run(target, RestoreFrames, RestoreDuration, RestoreEasing, completionSetters: null);
    }

    public Task Continue(Animatable target)
    {
        var session = TakeSession(target);
        return session is not null
            ? Run(target, session.ContinueFrames, session.ContinueDuration, session.ContinueEasing, session.Completion)
            : Run(target, ContinueFrames, ContinueDuration, ContinueEasing, SnapshotCompletion(Completion));
    }

    private static SessionSnapshot? TakeSession(Animatable target)
    {
        if (!_states.TryGetValue(target, out var state)) return null;
        var session = state.Session;
        state.Session = null;
        return session;
    }

    private static IReadOnlyList<Setter> SnapshotCompletion(AvaloniaList<Setter> source)
        => source.Count == 0 ? [] : source.ToArray();

    private sealed record SessionSnapshot(
        KeyFrames RestoreFrames,
        TimeSpan RestoreDuration,
        Easing? RestoreEasing,
        KeyFrames ContinueFrames,
        TimeSpan ContinueDuration,
        Easing? ContinueEasing,
        IReadOnlyList<Setter> Completion);

    private sealed class AnimationState
    {
        public CancellationTokenSource? TokenSource { get; set; }
        public SessionSnapshot? Session { get; set; }
    }
}
