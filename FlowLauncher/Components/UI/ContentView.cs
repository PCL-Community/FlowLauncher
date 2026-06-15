using Avalonia.Controls;

namespace FlowLauncher.Components.UI;

public abstract class ContentView : UserControl
{
    private static readonly Dictionary<Type, ContentView> _ViewCache = [];

    internal static ContentView GetViewCacheOrCreate<TContent>(bool bypassCache = false)
        where TContent : ContentView, new()
    {
        if (bypassCache) return new TContent();
        var type = typeof(TContent);
        if (!_ViewCache.TryGetValue(type, out var control))
            _ViewCache[type] = control = new TContent();
        return control;
    }
}
