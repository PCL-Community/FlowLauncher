using Avalonia.Data.Converters;

namespace FlowLauncher.Controls;

public static class Converters
{
    public static readonly FuncValueConverter<bool, double> BoolToOpacity = new(static value => value ? 1 : 0);

    public static readonly FuncMultiValueConverter<string?, bool> AllEqual = new(static parts =>
    {
        using var it = parts.GetEnumerator();
        if (!it.MoveNext()) return true;
        var value = it.Current;
        while (it.MoveNext()) if (it.Current != value) return false;
        return true;
    });
}
