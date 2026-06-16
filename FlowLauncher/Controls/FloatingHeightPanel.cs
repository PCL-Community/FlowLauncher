using Avalonia;
using Avalonia.Controls;

namespace FlowLauncher.Controls;

/// <summary>
/// A panel that decouples its children's layout from its own height.
/// Children are always measured with infinite vertical space and arranged
/// at their natural <see cref="Control.DesiredSize"/>, regardless of
/// what the panel itself is constrained to by its parent (e.g. by a
/// fold/unfold height animation). Combine with <c>ClipToBounds="True"</c>
/// on an ancestor to hide the overflow.
/// </summary>
public class FloatingHeightPanel : Panel
{
    protected override Size MeasureOverride(Size availableSize)
    {
        var infinite = new Size(availableSize.Width, double.PositiveInfinity);
        var maxWidth = 0.0;
        var maxHeight = 0.0;
        foreach (var child in Children)
        {
            child.Measure(infinite);
            maxWidth = Math.Max(maxWidth, child.DesiredSize.Width);
            maxHeight = Math.Max(maxHeight, child.DesiredSize.Height);
        }
        return new Size(maxWidth, maxHeight);
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        foreach (var child in Children)
        {
            // Arrange children at their natural DesiredSize, ignoring the
            // panel's own finalSize.Height. Overflow is the caller's problem
            // (use ClipToBounds on an ancestor).
            child.Arrange(new Rect(0, 0, finalSize.Width, child.DesiredSize.Height));
        }
        return finalSize;
    }
}
