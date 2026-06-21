using FlowLauncher.Components.UI;
using FlowNet.Collector;

namespace FlowLauncher.Components.FlowIntegration;

/// <summary>
/// Mark a view model class as a page, this class must have an empty constructor and implement <see cref="PageViewModel"/>.
/// </summary>
[CollectorMeta("FlowLauncher.Components.FlowIntegration.Page.RegisterFirstLoading")]
[CollectorMeta.SupportsNamedType(TypeGenerationMode.EmptyConstructor, true)]
[CollectorMeta.SupportsMethod<Func<PageViewModel>>]
[AttributeUsage(AttributeTargets.Class)]
public sealed class FirstLoadingPageAttribute : Attribute;
