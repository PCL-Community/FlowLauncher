using System.Runtime.InteropServices;
using FlowNet.Collector;

namespace FlowLauncher.Components.FlowIntegration;

// ReSharper disable InconsistentNaming
public enum FlowPlatform
{
    Windows = 0,
    Linux = 1,
    MacOS = 2,
    FreeBSD = 3,
}
// ReSharper restore InconsistentNaming

[Flow.Scope("fl:tool:platform-selector")]
public static partial class PlatformSelector
{
    private static readonly OSPlatform[] PlatformEnum = [
        OSPlatform.Windows,
        OSPlatform.Linux,
        OSPlatform.OSX,
        OSPlatform.FreeBSD
    ];

    [Flow.Task]
    private static Task<bool> StartsWith(params IEnumerable<string> prefix)
        => Task.FromResult(prefix.Any(RuntimeInformation.RuntimeIdentifier.StartsWith));

    [Flow.Task]
    private static Task<bool> Is(params IEnumerable<FlowPlatform> platform)
        => Task.FromResult(platform.Any(p => RuntimeInformation.IsOSPlatform(PlatformEnum[(int)p])));

#pragma warning disable CS9113 // Parameter is unread.

    [AttributeUsage(AttributeTargets.All)]
    [CollectionFilterMeta("fl:tool:platform-selector:starts-with")]
    public sealed class StartsWithAttribute(params string[] prefix) : Attribute;

    [AttributeUsage(AttributeTargets.All)]
    [CollectionFilterMeta("fl:tool:platform-selector:is")]
    public sealed class IsAttribute(params FlowPlatform[] platform) : Attribute;

#pragma warning restore CS9113 // Parameter is unread.
}
