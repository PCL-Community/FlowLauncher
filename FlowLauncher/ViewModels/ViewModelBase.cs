using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FlowLauncher.ViewModels;

public abstract class ViewModelBase : ObservableObject
{
    private Dictionary<string, Action>? _changingObservers = null;
    private Dictionary<string, Action>? _changedObservers = null;

    public void RegisterPropertyChanging(string propertyName, Action action)
    {
        (_changingObservers ??= [])[propertyName] = action;
    }

    public void RegisterPropertyChanged(string propertyName, Action action)
    {
        (_changedObservers ??= [])[propertyName] = action;
    }

    protected override void OnPropertyChanging(PropertyChangingEventArgs e)
    {
        base.OnPropertyChanging(e);
        if (_changingObservers == null || e.PropertyName == null) return;
        if (_changingObservers.TryGetValue(e.PropertyName, out var action)) action();
    }

    protected override void OnPropertyChanged(PropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (_changedObservers == null || e.PropertyName == null) return;
        if (_changedObservers.TryGetValue(e.PropertyName, out var action)) action();
    }
}
