using Avalonia.Controls;
using FlowNet.Collector;

namespace FlowLauncher.Components.FlowIntegration;

public static class PlatformWindow
{

    public static Task SetCurrentSessionWindowFactory(Func<Window> window)
    {
        Program.CurrentSessionWindowFactory = window;
        return Task.CompletedTask;
    }
}

[CollectorMeta("FlowLauncher.Components.FlowIntegration.PlatformWindow.SetCurrentSessionWindowFactory")]
[CollectorMeta.RequiresAttr(typeof(PlatformSelector.IsAttribute))]
[CollectorMeta.SupportsMethod<Func<Window>>]
[CollectorMeta.SupportsNamedType(TypeGenerationMode.EmptyConstructor, true)]
[AttributeUsage(AttributeTargets.Class)]
public sealed class PlatformWindowAttribute : Attribute;
