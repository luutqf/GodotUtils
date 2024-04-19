namespace GodotUtils;

using Godot;
using System.Collections.Generic;

public static class ExtensionsRayCast2D
{
    /// <summary>
    /// <para>
    /// Get the tile from a tilemap that a raycast is colliding with.
    /// Use tileData.Equals(default(Variant)) to check if no tile data exists
    /// here.
    /// </para>
    /// 
    /// <para>
    /// Useful if trying to detect what tile the player is standing on
    /// </para>
    /// 
    /// <para>
    /// To get the tile the player is currently in see TileMap.GetTileData(...)
    /// </para>
    /// 这个方法能够从 TileMap 获取射线（raycast）所碰撞到的瓷砖（tile）的数据。
    /// 该方法检查射线是否碰撞，并且碰撞对象是否为 TileMap 类型。
    /// 如果是，它将获取碰撞点，并将此点转换为瓷砖坐标，然后获取该位置的瓷砖数据。此方法对于想要检测玩家脚下所站瓷砖类型的情况非常有用。
    /// </summary>
    public static Variant GetTileData(this RayCast2D raycast, string layerName)
    {
        if (!raycast.IsColliding() || raycast.GetCollider() is not TileMap tileMap)
            return default;

        Vector2 collisionPos = raycast.GetCollisionPoint();
        Vector2I tilePos = tileMap.LocalToMap(tileMap.ToLocal(collisionPos));

        TileData tileData = tileMap.GetCellTileData(0, tilePos);

        if (tileData == null)
            return default;

        return tileData.GetCustomData(layerName);
    }

    /// <summary>
    /// Set the provided mask values to true. Everything else will be set to be false.
    /// 这个方法设置 RayCast2D 的碰撞层掩码（CollisionMask），允许指定哪些层应该被射线检测到。
    /// 方法会重置所有层掩码为 0（即忽略所有层），然后根据提供的值设置对应的层为 true。这使得根据需要动态调整射线的碰撞检测目标变得简单。
    /// </summary>
    public static void SetCollisionMask(this RayCast2D node, params int[] values)
    {
        // Reset all mask values to 0
        node.CollisionMask = 0;

        foreach (int value in values)
            node.SetCollisionMaskValue(value, true);
    }

    /// <summary>
    /// A convience function to tell the raycast to exlude all parents that
    /// are of type CollisionObject2D (for example a ground raycast should
    /// only check for the ground, not the player itself)
    /// 这个方法用于从射线的碰撞检测中排除所有的 CollisionObject2D 类型的父节点，
    /// 这对于那些不希望射线与其发射源本身发生碰撞的情况非常有用（例如，一个用于检测地面的射线不希望与玩家自身碰撞）。
    /// </summary>
    public static void ExcludeRaycastParents(this RayCast2D raycast, Node parent)
    {
        if (parent != null)
        {
            if (parent is CollisionObject2D collision)
                raycast.AddException(collision);

            ExcludeRaycastParents(raycast, parent.GetParentOrNull<Node>());
        }
    }

    /// <summary>
    /// Checks if any raycasts in a collection is colliding
    /// 这是一个用于检查一个 RayCast2D 集合中的任何一个射线是否正在发生碰撞的方法。
    /// 如果集合中任何一个射线检测到碰撞，该方法将返回 true。
    /// </summary>
    /// <param name="raycasts">Collection of raycasts to check</param>
    /// <returns>True if any ray cast is colliding, else false</returns>
    public static bool IsAnyRayCastColliding(this List<RayCast2D> raycasts)
    {
        foreach (RayCast2D raycast in raycasts)
            if (raycast.IsColliding())
                return true;

        return false;
    }

    /// <summary>
    /// Returns the first raycasts in a collection which is colliding
    /// 类似于 IsAnyRayCastColliding，这个方法检查 RayCast2D 集合并返回第一个发生碰撞的射线节点。
    /// 如果没有任何射线发生碰撞，则返回 null（或默认值）。
    /// </summary>
    /// <param name="raycasts">Collection of raycasts to check</param>
    /// <returns>Raycast which is colliding, else default</returns>
    public static RayCast2D GetAnyRayCastCollider(this List<RayCast2D> raycasts)
    {
        foreach (RayCast2D raycast in raycasts)
            if (raycast.IsColliding())
                return raycast;

        return default;
    }
}
