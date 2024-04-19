namespace GodotUtils;

using Godot;

public static class ExtensionsCollisionObject2D
{
    /// <summary>
    /// Set the specified layer and mask values to true. Everything else will 
    /// be set to be false.
    /// 设置指定的图层和蒙板为true。其他的都设为假。

    /// </summary>
    public static void SetCollisionMaskLayer(this CollisionObject2D node, params int[] values)
    {
        // Reset all layer and mask values to 0
        //重置所有图层和蒙版值为0
        node.CollisionLayer = 0;
        node.CollisionMask = 0;

        foreach (int value in values)
        {
            node.SetCollisionLayerValue(value, true);
            node.SetCollisionMaskValue(value, true);
        }
    }

    /// <summary>
    /// Set the specified mask values to true. Everything else will 
    /// be set to be false.
    /// 将指定的掩码值设置为true。其他的都设为假。
    /// </summary>
    public static void SetCollisionMask(this CharacterBody2D node, params int[] values)
    {
        // Reset all mask values to 0
        node.CollisionMask = 0;

        foreach (int value in values)
        {
            node.SetCollisionMaskValue(value, true);
        }
    }

    /// <summary>
    /// Set the specified layer values to true. Everything else will 
    /// be set to be false.
    /// 将指定的图层值设置为true。其他的都设为假。
    /// </summary>
    public static void SetCollisionLayer(this CharacterBody2D node, params int[] values)
    {
        // Reset all layer values to 0
        node.CollisionLayer = 0;

        foreach (int value in values)
        {
            node.SetCollisionLayerValue(value, true);
        }
    }
}
