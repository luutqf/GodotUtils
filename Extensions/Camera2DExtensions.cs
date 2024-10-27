using Godot;

namespace RedotUtils;

public static class Camera2DExtensions
{
    public static void SetPositionIgnoreSmoothing(this Camera2D camera, Vector2 position)
    {
        bool smoothEnabled = camera.PositionSmoothingEnabled;

        if (smoothEnabled)
        {
            camera.PositionSmoothingEnabled = false;
        }

        camera.Position = position;

        if (smoothEnabled)
        {
            RTween.Delay(camera, 0.01, () => camera.PositionSmoothingEnabled = true);
        }
    }
}

