namespace GodotUtils;

using Godot;

public static class ExtensionsNode2D
{
    /// <summary>
    /// 这段代码中包含的扩展方法 Reparent 专为 Godot 引擎的 Node2D 类型节点设计，用于将 Node2D 类型节点重新指定父节点。
    /// 这个过程中会保持节点的全局位置（GlobalPosition）和全局旋转（GlobalRotation）状态不变。
    /// 这个方法是针对 Node2D 节点的一种便捷操作，在祖父母节点层次结构中挪动节点时经常会用到。这在游戏开发中很有用，比如：
    ///     当你想从飞船上拆卸导弹，并希望导弹能独立于飞船的位置和旋转之后，这个功能非常实用。
    /// 也可以用来在动态的游戏环境中重新分配场景节点，而无需手动计算旋转和位置的转换来保持一致的视觉效果。
    /// 该方法的工作流程如下：
    /// 1.记录节点当前的全局位置和旋转。
    /// 2.将节点从其父节点移除。
    /// 3.将节点添加到目标父节点（targetParent）中。
    /// 4.恢复节点在新父节点下的全局位置和旋转。
    /// 通过这样做，无论在场景树中如何移动节点，节点的屏幕上的表示（位置和朝向）看起来都会保持不变。这保证了游戏中的元素能够在逻辑上转移位置而不会引起用户的视觉上的混淆。
    /// <para>
    /// Reparent a node to a new parent. The rotation and position will be
    /// preserved.
    /// </para>
    /// 
    /// <para>
    /// Useful for if example you want to detach missiles connected from a
    /// spaceship. The missiles position and rotation will no longer be
    /// influenced by the spaceship.
    /// </para>
    /// </summary>
    public static void Reparent(this Node2D node, Node targetParent)
    {
        Vector2 pos = node.GlobalPosition;
        float rot = node.GlobalRotation;

        node.GetParent().RemoveChild(node);
        targetParent.AddChild(node);

        node.GlobalPosition = pos;
        node.GlobalRotation = rot;
    }
}
