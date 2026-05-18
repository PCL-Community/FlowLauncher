using Avalonia;
using Avalonia.Data.Converters;

namespace FlowLauncher.Controls;

public static class Converters
{
    public static readonly FuncValueConverter<bool, double?, double> BoolToOpacity = new(static (value, param) =>
    {
        if (!value) return 0;
        var opacity = param ?? 1;
        return opacity;
    });

    public static readonly FuncMultiValueConverter<object?, bool> AllEqual = new(static parts =>
    {
        using var it = parts.GetEnumerator();
        if (!it.MoveNext()) return true;
        var value = it.Current;
        while (it.MoveNext()) if (it.Current != value) return false;
        return true;
    });

    public static readonly FuncMultiValueConverter<object?, object?> IfBranch = new(static parts =>
    {
        var list = parts.ToList();
        if (list is not [bool condition, _, _]) return null;
        return condition ? list[1] : list[2];
    });

    public static readonly FuncMultiValueConverter<object?, object?> FirstNotNull = new(static parts =>
    {
        return parts.FirstOrDefault(x => x != null && x is not UnsetValueType);
    });
}
