namespace FlowLauncher.Components.Extensions;

public static class ReactiveExtensions
{
    private sealed class Observer<T> : IObserver<T>
    {
        public Action? Completed { get; init; }
        public Action<Exception>? Error { get; init; }
        public Action<T>? Next { get; init; }

        public void OnCompleted() => Completed?.Invoke();
        public void OnError(Exception error) => Error?.Invoke(error);
        public void OnNext(T value) => Next?.Invoke(value);
    }

    extension<T>(IObservable<T> observable)
    {
        public IDisposable Subscribe(Action<T> action) => observable.Subscribe(new Observer<T> { Next = action });
    }
}
