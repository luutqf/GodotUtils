using Godot;

namespace GodotUtils;

public static class VectorUtils
{
    /// <summary>
    /// Returns a random vector between 0 and 1 (inclusive) for X and Y.
    /// </summary>
    public static Vector2 Random()
    {
        return new Vector2(MathUtils.RandRange(-1.0, 1.0), MathUtils.RandRange(-1.0, 1.0)).Normalized();
    }
}
