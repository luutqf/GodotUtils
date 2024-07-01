namespace GodotUtils;

using Godot;

public static class ExtensionsTileMap
{
    // enable a layer with Mathf.Pow(2, x - 1) where x is the layer you want enabled
    // if you wanted to enable multiple then add the sum of the powers
    // e.g. Mathf.Pow(2, 1) + Mathf.Pow(2, 3) to enable layers 0 and 2
    //这个方法允许一次性启用 TileMap 中的多个图层。它通过接受一个或多个图层索引（通过参数 layers），
    //并计算出一个用于 PhysicsLayerCollisionLayer 和 PhysicsLayerCollisionMask 的掩码值。
    //这个方法使用自定义的 GUMath.UIntPow 函数以 2 的幂形式计算图层的值，这对于处理物理图层尤其有用。
    [Obsolete("Obsolete")]
    public static void EnableLayers(this TileMap tileMap, params uint[] layers)
    {
        uint result = 0;

        foreach (uint layer in layers)
            result += GUMath.UIntPow(2, layer - 1);

        tileMap.TileSet.SetPhysicsLayerCollisionLayer(0, result);
        tileMap.TileSet.SetPhysicsLayerCollisionMask(0, result);
    }

    /// <summary>
    /// <para>
    /// Get the tile data from a global position. Use 
    /// tileData.Equals(default(Variant)) to check if no tile data exists here.
    /// </para>
    /// 
    /// <para>
    /// Useful if trying to get the tile the player is currently inside.
    /// </para>
    /// 用于从全局位置获取特定图层的瓦片数据。这对于检测玩家当前所处的瓦片或是获取瓦片上的自定义数据（如“layerName”指定的数据）特别有用。
    /// <para>
    /// To get the tile the player is standing on see RayCast2D.GetTileData(...)
    /// </para>
    /// </summary>
    [Obsolete("Obsolete")]
    public static Variant GetTileData(this TileMap tilemap, Vector2 pos, string layerName)
    {
        if (tilemap == null) throw new ArgumentNullException(nameof(tilemap));
        Vector2I tilePos = tilemap.LocalToMap(tilemap.ToLocal(pos));

        TileData tileData = tilemap.GetCellTileData(0, tilePos);

        if (tileData == null)
            return default;

        return tileData.GetCustomData(layerName);
    }

    /// <summary>
    /// 检查给定的全局位置是否在 TileMap 的范围内。通过将全局位置转换为瓦片地图内的局部瓦片位置，并检查该位置是否有瓦片存在来实现。
    /// </summary>
    /// <param name="tilemap"></param>
    /// <param name="pos"></param>
    /// <returns></returns>
    [Obsolete("Obsolete")]
    public static bool InTileMap(this TileMap tilemap, Vector2 pos)
    {
        if (tilemap == null) throw new ArgumentNullException(nameof(tilemap));
        Vector2I tilePos = tilemap.LocalToMap(tilemap.ToLocal(pos));

        return tilemap.GetCellSourceId(0, tilePos) != -1;
    }

    /// <summary>
    /// 获取位于给定位置的瓦片的名称（如果存在）。首先检查该位置是否存在瓦片，然后获取瓦片数据并读取其“Name”自定义数据。
    /// </summary>
    /// <param name="tilemap"></param>
    /// <param name="pos"></param>
    /// <param name="layer"></param>
    /// <returns></returns>
    [Obsolete("Obsolete")]
    public static string GetTileName(this TileMap tilemap, Vector2 pos, int layer = 0)
    {
        if (tilemap == null) throw new ArgumentNullException(nameof(tilemap));
        if (!tilemap.TileExists(pos))
            return "";

        TileData tileData = tilemap.GetCellTileData(layer, tilemap.LocalToMap(pos));

        if (tileData == null)
            return "";

        Variant data = tileData.GetCustomData("Name");

        return data.AsString();
    }

    //检查给定位置是否存在瓦片。这是通过检查该位置的 SourceId 是否不等于 -1 来实现的，如果是则说明位置上有瓦片存在。
    [Obsolete("Obsolete")]
    public static bool TileExists(this TileMap tilemap, Vector2 pos, int layer = 0) =>
        tilemap.GetCellSourceId(layer, tilemap.LocalToMap(pos)) != -1;

    [Obsolete("Obsolete")]
    static int GetCurrentTileId(this TileMap tilemap, Vector2 pos)
    {
        Vector2I cellPos = tilemap.LocalToMap(pos);
        return 0;
        //return tilemap.GetCellv(cellPos); // TODO: Godot 4 conversion
    }
}
