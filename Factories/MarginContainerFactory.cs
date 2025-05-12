using Godot;

namespace GodotUtils;

public class MarginContainerFactory
{
    public static MarginContainer Create(int padding)
    {
        return Create(padding, padding, padding, padding);
    }

    public static MarginContainer Create(int left, int right, int top, int bottom)
    {
        MarginContainer container = new();
        container.SetMarginLeft(left);
        container.SetMarginRight(right);
        container.SetMarginTop(top);
        container.SetMarginBottom(bottom);
        return container;
    }
}
