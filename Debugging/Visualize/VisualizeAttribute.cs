#if DEBUG
using System;

namespace GodotUtils.Debugging.Visualize;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Method)]
public class VisualizeAttribute : Attribute
{
}
#endif
