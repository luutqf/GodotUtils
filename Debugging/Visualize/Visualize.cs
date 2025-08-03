using Godot;

namespace GodotUtils.Debugging.Visualize;

/// <summary>
/// This logger shows all messages in game making it easier to debug
/// </summary>
public class Visualize
{
    private const int MaxLabelsVisible = 5;

    public static void Log(object message, Node node, double fadeTime = 5)
    {
        VBoxContainer vbox = GetOrCreateVBoxContainer(node);

        if (vbox != null)
        {
            AddLabel(vbox, message, fadeTime);
        }
    }

    private static VBoxContainer GetOrCreateVBoxContainer(Node node)
    {
        if (VisualizeAutoload.Instance.VisualNodes != null && VisualizeAutoload.Instance.VisualNodes.TryGetValue(node, out VBoxContainer vbox))
        {
            return vbox;
        }

        if (node is not Control and not Node2D)
        {
            return null;
        }

        if (!VisualizeAutoload.Instance.VisualNodesWithoutVisualAttribute.TryGetValue(node, out vbox))
        {
            vbox = new VBoxContainer
            {
                Scale = Vector2.One * VisualUI.VisualUiScaleFactor
            };

            node.AddChild(vbox);
            VisualizeAutoload.Instance.VisualNodesWithoutVisualAttribute[node] = vbox;
        }

        return vbox;
    }

    private static void AddLabel(VBoxContainer vbox, object message, double fadeTime)
    {
        Label label = new() { Text = message?.ToString() };

        vbox.AddChild(label);
        vbox.MoveChild(label, 0);

        if (vbox.GetChildCount() > MaxLabelsVisible)
        {
            vbox.RemoveChild(vbox.GetChild(vbox.GetChildCount() - 1));
        }

        _ = new GTween(label)
            .SetAnimatingProp(CanvasItem.PropertyName.Modulate)
            .AnimateProp(Colors.Transparent, fadeTime)
            .Callback(label.QueueFree);
    }
}
