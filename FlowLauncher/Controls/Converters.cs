using Avalonia.Data.Converters;

namespace FlowLauncher.Controls;

public static class Converters
{
    public static readonly FuncMultiValueConverter<string?, bool> AllEqual = new(parts =>
    {
        using var it = parts.GetEnumerator();
        if (!it.MoveNext()) return true;
        var value = it.Current;
        while (it.MoveNext()) if (it.Current != value) return false;
        return true;
    });
}
