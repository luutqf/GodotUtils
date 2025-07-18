using Godot;
using System;
using System.Collections.Generic;

namespace GodotUtils;

/// <summary>
/// Manages the shaking of all child sprites of a given node including the node itself if it is a sprite.
/// Both Sprite2D and AnimatedSprite2D are supported.
/// </summary>
public class SpriteShakeManager
{
    private readonly List<AnimatedSprite2D> _animatedSprites = [];
    private readonly List<Sprite2D> _sprites = [];
    private readonly RandomNumberGenerator _rng;

    public SpriteShakeManager(List<Node2D> sprites)
    {
        ArgumentNullException.ThrowIfNull(sprites);

        foreach (Node2D node in sprites)
        {
            ArgumentNullException.ThrowIfNull(node);

            if (node is Sprite2D sprite)
            {
                _sprites.Add(sprite);
            }
            else if (node is AnimatedSprite2D animatedSprite)
            {
                _animatedSprites.Add(animatedSprite);
            }
            else
            {
                throw new ArgumentException($"Node '{node.Name}' is not a {nameof(Sprite2D)} or {nameof(AnimatedSprite2D)}");
            }
        }

        _rng = new RandomNumberGenerator();
        _rng.Randomize();
    }

    public void Shake(float intensity)
    {
        float randomOffset = _rng.RandfRange(-intensity, intensity);

        foreach (Sprite2D sprite in _sprites)
        {
            sprite.Offset = new Vector2(sprite.Offset.X, randomOffset);
        }

        foreach (AnimatedSprite2D sprite in _animatedSprites)
        {
            sprite.Offset = new Vector2(sprite.Offset.X, randomOffset);
        }
    }

    public void Reset()
    {
        foreach (Sprite2D sprite in _sprites)
        {
            sprite.Offset = new Vector2(sprite.Offset.X, 0);
        }

        foreach (AnimatedSprite2D sprite in _animatedSprites)
        {
            sprite.Offset = new Vector2(sprite.Offset.X, 0);
        }
    }
}
