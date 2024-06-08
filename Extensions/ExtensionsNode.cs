using System;

namespace GodotUtils;

using Godot;
using System;

public static class ExtensionsNode
{
    
    /// <summary>
    /// 这两个方法分别用于禁用和启用一个节点的所有子节点，统一设置它们的 SetProcess 和 SetPhysicsProcess 状态。
    /// DisableChildren 会关闭子节点的处理和物理处理功能，而 EnableChildren 则会打开这些功能。这通常用于在游戏运行时临时暂停或启用特定部分的逻辑。
    /// </summary>
    /// <param name="node"></param>
    public static void DisableChildren(this Node node)
    {
        foreach (Node child in node.GetChildren())
        {
            child.SetProcess(false);
            child.SetPhysicsProcess(false);
        }
    }


    public static void EnableChildren(this Node node)
    {
        foreach (Node child in node.GetChildren())
        {
            child.SetProcess(true);
            child.SetPhysicsProcess(true);
        }
    }

    /// <summary>
    /// AddChildDeferred 方法使用 CallDeferred 函数来延迟添加一个子节点，直到当前消息队列空闲为止。这在编辑场景树时需要避免当前执行流被中断时非常有用。
    /// </summary>
    /// <param name="node"></param>
    /// <param name="child"></param>
    public static void AddChildDeferred(this Node node, Node child) =>
        node.CallDeferred("add_child", child);

    /// <summary>
    /// Reparent 方法用于将一个节点从它当前的父节点移动到一个新的父节点。在调用这个方法时，节点首先从当前父节点移除，然后添加到新的父节点中。
    /// </summary>
    /// <param name="curParent"></param>
    /// <param name="newParent"></param>
    /// <param name="node"></param>
    public static void Reparent(this Node curParent, Node newParent, Node node)
    {
        // Remove node from current parent
        curParent.RemoveChild(node);

        // Add node to new parent
        newParent.AddChild(node);
    }

    /// <summary>
    /// 这个泛型 GetChildren<TNode> 方法允许获取一个父节点所有子节点的数组，并且这些子节点必须是 TNode 类型或其子类型。
    /// 如果类型转换失败，这个方法会在 Godot 的错误信息中记录一个错误。
    /// Get all children assuming they all extend from TNode
    /// </summary>
    public static TNode[] GetChildren<TNode>(this Node parent) where TNode : Node
    {
        Godot.Collections.Array<Node> children = parent.GetChildren();
        TNode[] arr = new TNode[children.Count];

        for (int i = 0; i < children.Count; i++)
            try
            {
                arr[i] = (TNode)children[i];
            }
            catch (InvalidCastException)
            {
                GD.PushError($"Could not get all children from parent " +
                    $"'{parent.Name}' because could not cast from " +
                    $"{children[i].GetType()} to {typeof(TNode)} for node " +
                    $"'{children[i].Name}'");
            }

        return arr;
    }

    /// <summary>
    /// QueueFreeChildren 方法用于安排所有子节点被释放，即标记为删除，当场景树处于空闲状态时再真正清除。
    /// </summary>
    /// <param name="parentNode"></param>
    public static void QueueFreeChildren(this Node parentNode)
    {
        foreach (Node node in parentNode.GetChildren())
            node.QueueFree();
    }

    /// <summary>
    /// RemoveAllGroups 方法会从一个节点中移除它所属的所有组。
    /// </summary>
    /// <param name="node"></param>
    public static void RemoveAllGroups(this Node node)
    {
        Godot.Collections.Array<StringName> groups = node.GetGroups();
        for (int i = 0; i < groups.Count; i++)
            node.RemoveFromGroup(groups[i]);
    }
}
