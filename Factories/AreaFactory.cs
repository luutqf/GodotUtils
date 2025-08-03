using Godot;
using System;

namespace GodotUtils;

public class AreaRectBuilder : AreaBuilder
{
    protected override RectangleShape2D Shape { get; }

    public AreaRectBuilder(Vector2 size, bool transparent = false) : this(size, DefaultColor, transparent) { }

    public AreaRectBuilder(Vector2 size, Color color, bool transparent = false) : base(color, transparent)
    {
        Shape = new RectangleShape2D { Size = size };
    }

    public void UpdateSize(Vector2 size)
    {
        Shape.Size = size;
    }
}

public class AreaCircleBuilder : AreaBuilder
{
    protected override CircleShape2D Shape { get; }

    public AreaCircleBuilder(float radius, bool transparent = false) : this(radius, DefaultColor, transparent) { }

    public AreaCircleBuilder(float radius, Color color, bool transparent = false) : base(color, transparent)
    {
        Shape = new CircleShape2D { Radius = radius };
    }

    public void UpdateSize(float radius)
    {
        Shape.Radius = radius;
    }
}

public abstract class AreaBuilder(Color color, bool transparent)
{
    protected static readonly Color DefaultColor = Color.Color8(0, 153, 179, 40); // Semi transparent blue

    protected abstract Shape2D Shape { get; }

    protected Color _color = color;
    protected bool _transparent = transparent;

    public Area2D Build()
    {
        if (_transparent)
            _color.A = 0;

        Area2D area = new();

        CollisionShape2D areaCollision = new()
        {
            DebugColor = _color,
            Shape = Shape
        };

        area.AddChild(areaCollision);

        return area;
    }
}
