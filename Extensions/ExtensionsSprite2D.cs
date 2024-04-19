namespace GodotUtils;

using Godot;

public static class ExtensionsSprite2D
{
    /// <summary>
    /// GetSize 方法简单地返回 Sprite2D 对象的纹理尺寸，这个尺寸来自纹理自身的大小。
    /// </summary>
    /// <param name="sprite"></param>
    /// <returns></returns>
    public static Vector2 GetSize(this Sprite2D sprite) => sprite.Texture.GetSize();

    /// <summary>
    /// <para>
    /// Gets the actual pixel size of the sprite. All rows and columns 
    /// consisting of transparent pixels are subtracted from the size.
    /// </para>
    /// GetPixelSize 方法计算 Sprite2D 对象实际的像素尺寸，考虑了透明像素行和列的剔除。
    /// 具体的尺寸通过调用 GetPixelWidth 和 GetPixelHeight 方法计算得出，这是为了动态创建碰撞形状时能获得准确的非透明区域的尺寸。
    /// <para>
    /// This is useful to know if dynamically creating collision
    /// shapes at runtime.
    /// </para>
    /// </summary>
    public static Vector2 GetPixelSize(this Sprite2D sprite) =>
        new Vector2(GetPixelWidth(sprite), GetPixelHeight(sprite));

    /// <summary>
    /// <para>
    /// Gets the actual pixel width of the sprite. All columns consisting of 
    /// transparent pixels are subtracted from the width.
    /// </para>
    /// 
    /// <para>
    /// This is useful to know if dynamically creating collision
    /// shapes at runtime.
    /// </para>
    /// </summary>
    public static int GetPixelWidth(this Sprite2D sprite)
    {
        Image img = sprite.Texture.GetImage();
        Vector2I size = img.GetSize();

        int transColumnsLeft = GU.GetTransparentColumnsLeft(img, size);
        int transColumnsRight = GU.GetTransparentColumnsRight(img, size);

        int pixelWidth = size.X - transColumnsLeft - transColumnsRight;

        return (int)(pixelWidth * sprite.Scale.X);
    }

    /// <summary>
    /// <para>
    /// Gets the actual pixel height of the sprite. All rows consisting of 
    /// transparent pixels are subtracted from the height.
    /// </para>
    /// 
    /// <para>
    /// This is useful to know if dynamically creating collision
    /// shapes at runtime.
    /// </para>
    /// </summary>
    public static int GetPixelHeight(this Sprite2D sprite)
    {
        Image img = sprite.Texture.GetImage();
        Vector2I size = img.GetSize();

        int transRowsTop = GU.GetTransparentRowsTop(img, size);
        int transRowsBottom = GU.GetTransparentRowsBottom(img, size);

        int pixelHeight = size.Y - transRowsTop - transRowsBottom;

        return (int)(pixelHeight * sprite.Scale.Y);
    }

    /// <summary>
    /// GetPixelBottomY 方法检查 Sprite2D 对象纹理底部的透明像素行数。它通过检查纹理中心列自底向上的每个像素直到找到第一个不透明像素。
    /// 这个方法可能按照注释所述不会适用于所有的 Sprite2D 对象，但适用于特定情况（如示例中所述的忍者）。
    /// </summary>
    /// <param name="sprite"></param>
    /// <returns></returns>
    public static int GetPixelBottomY(this Sprite2D sprite)
    {
        Image img = sprite.Texture.GetImage();
        Vector2I size = img.GetSize();

        // Might not work with all sprites but works with ninja.
        // The -2 offset that is
        int diff = 0;

        for (int y = (int)size.Y - 1; y >= 0; y--)
        {
            if (img.GetPixel((int)size.X / 2, y).A != 0)
                break;

            diff++;
        }

        return diff;
    }
}
