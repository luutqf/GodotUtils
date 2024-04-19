namespace GodotUtils;

using Godot;

public static class ExtensionsAnimatedSprite2D
{
    /// <summary>
    /// There may be a small delay when switching between animations. Use this function to remove that delay.
    /// 在动画之间切换时可能会有一点延迟。使用这个函数来消除延迟。
    /// </summary>
    public static void InstantPlay(this AnimatedSprite2D sprite, string anim)
    {
        sprite.Animation = anim;
        sprite.Play(anim);
    }

    /// <summary>
    /// There may be a small delay when switching between animations. Use this function
    /// to remove that delay.
    /// 在动画之间切换时可能会有一点延迟。用这个来消除这种延迟。
    /// </summary>
    public static void InstantPlay(this AnimatedSprite2D sprite, string anim, int frame)
    {
        sprite.Animation = anim;

        int frameCount = sprite.SpriteFrames.GetFrameCount(anim);

        if (frameCount - 1 >= frame)
            sprite.Frame = frame;
        else
            GU.Services.Get<Logger>().LogWarning($"The frame '{frame}' specified for {sprite.Name} is" +
                $"lower than the frame count '{frameCount}'");

        sprite.Play(anim);
    }

    /// <summary>
    /// <para>
    /// Play a animation starting at a random frame
    /// 从随机帧开始播放动画
    /// </para>
    /// 
    /// <para>
    /// This is useful if making for example coin animations play at random frames
    /// 这在制作随机帧的硬币动画时非常有用
    /// </para>
    /// </summary>
    public static void PlayRandom(this AnimatedSprite2D sprite, string anim = "")
    {
        anim = string.IsNullOrWhiteSpace(anim) ? sprite.Animation : anim;
        sprite.InstantPlay(anim);
        sprite.Frame = GD.RandRange(0, sprite.SpriteFrames.GetFrameCount(anim));
    }

    /// <summary>
    /// Gets the scaled width of the specified sprite frame
    /// 获取指定精灵框架的缩放宽度
    /// </summary>
    public static int GetWidth(this AnimatedSprite2D sprite, string anim = "")
    {
        anim = string.IsNullOrWhiteSpace(anim) ? sprite.Animation : anim;
        return (int)(sprite.SpriteFrames.GetFrameTexture(anim, 0).GetWidth() *
            sprite.Scale.X);
    }

    /// <summary>
    /// Gets the scaled height of the specified sprite frame
    /// 获取指定精灵框架的缩放高度
    /// </summary>
    public static int GetHeight(this AnimatedSprite2D sprite, string anim = "")
    {
        anim = string.IsNullOrWhiteSpace(anim) ? sprite.Animation : anim;
        return (int)(sprite.SpriteFrames.GetFrameTexture(anim, 0).GetHeight() *
            sprite.Scale.Y);
    }

    /// <summary>
    /// Gets the scaled size of the specified sprite frame
    /// 获取指定精灵帧的缩放大小
    /// </summary>
    public static Vector2 GetSize(this AnimatedSprite2D sprite, string anim = "")
    {
        anim = string.IsNullOrWhiteSpace(anim) ? sprite.Animation : anim;
        return new Vector2(GetWidth(sprite, anim), GetHeight(sprite, anim));
    }

    /// <summary>
    /// <para>
    /// Gets the actual pixel size of the sprite. All rows and columns 
    /// consisting of transparent pixels are subtracted from the size.
    /// 获取精灵的实际像素大小。所有由透明像素组成的行和列都从大小中减去。
    /// </para>
    /// 
    /// <para>
    /// This is useful to know if dynamically creating collision
    /// shapes at runtime.
    /// 这对于了解是否在运行时动态创建形状很有用。
    /// </para>
    /// </summary>
    public static Vector2 GetPixelSize(this AnimatedSprite2D sprite, string anim = "")
    {
        anim = string.IsNullOrWhiteSpace(anim) ? sprite.Animation : anim;
        return new Vector2(GetPixelWidth(sprite, anim), GetPixelHeight(sprite, anim));
    }

    /// <summary>
    /// <para>
    /// Gets the actual pixel width of the sprite. All columns consisting of 
    /// transparent pixels are subtracted from the width.
    /// 获取精灵的实际像素宽度。所有包含透明像素的列都从宽度中减去。
    /// </para>
    /// 
    /// <para>
    /// This is useful to know if dynamically creating collision
    /// shapes at runtime.
    /// 如果在运行时动态创建碰撞形状，这是很有用的。
    /// </para>
    /// </summary>
    public static int GetPixelWidth(this AnimatedSprite2D sprite, string anim = "")
    {
        anim = string.IsNullOrWhiteSpace(anim) ? sprite.Animation : anim;

        Texture2D tex = sprite.SpriteFrames.GetFrameTexture(anim, 0);
        Image img = tex.GetImage();
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
    /// 获取精灵的实际像素高度。所有由透明像素组成的行都从高度中减去。
    /// </para>
    /// 
    /// <para>
    /// This is useful to know if dynamically creating collision
    /// shapes at runtime.
    /// 如果在运行时动态创建碰撞形状，这是很有用的。
    /// </para>
    /// </summary>
    public static int GetPixelHeight(this AnimatedSprite2D sprite, string anim = "")
    {
        anim = string.IsNullOrWhiteSpace(anim) ? sprite.Animation : anim;

        Texture2D tex = sprite.SpriteFrames.GetFrameTexture(anim, 0);
        Image img = tex.GetImage();
        Vector2I size = img.GetSize();

        int transRowsTop = GU.GetTransparentRowsTop(img, size);
        int transRowsBottom = GU.GetTransparentRowsBottom(img, size);

        int pixelHeight = size.Y - transRowsTop - transRowsBottom;

        return (int)(pixelHeight * sprite.Scale.Y);
    }

    public static int GetPixelBottomY(this AnimatedSprite2D sprite, string anim = "")
    {
        anim = string.IsNullOrWhiteSpace(anim) ? sprite.Animation : anim;

        Texture2D tex = sprite.SpriteFrames.GetFrameTexture(anim, 0);
        Image img = tex.GetImage();
        Vector2I size = img.GetSize();

        // Might not work with all sprites but works with ninja.
        //可能并不适用于所有精灵，但适用于忍者。
        // 
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
