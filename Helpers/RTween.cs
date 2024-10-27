using Godot;
using static Godot.Tween;
using System;

namespace RedotUtils;

public class RTween
{
    private PropertyTweener _tweener;
    private Tween _tween;
    private Node _node;
    private string _animatingProperty;
    private ShaderMaterial _animatingShaderMaterial;

    public RTween(Node node)
    {
        _node = node;

        // Ensure the Tween is fresh when re-creating it
        Kill();
        _tween = node.CreateTween();

        // This helps to prevent the camera from lagging behind the players movement
        _tween.SetProcessMode(Tween.TweenProcessMode.Physics);
    }

    /// <summary>
    /// Creates a delay of <paramref name="duration"/> seconds followed by a
    /// <paramref name="callback"/>
    /// </summary>
    public static RTween Delay(Node node, double duration, Action callback)
    {
        RTween tween = new(node);

        tween.Delay(duration)
            .Callback(callback);

        return tween;
    }

    /// <summary>
    /// Animates the property that was set with SetAnimatingProp(string prop)
    /// 
    /// <code>
    /// tween.SetAnimatingProp(ColorRect.PropertyName.Color);
    /// tween.AnimateProp(Colors.Transparent, 0.5);
    /// </code>
    /// </summary>
    public RTween AnimateProp(Variant finalValue, double duration)
    {
        if (string.IsNullOrWhiteSpace(_animatingProperty))
        {
            throw new Exception("No animation property has been set with tween.SetAnimatingProp(...)");
        }

        return Animate(_animatingProperty, finalValue, duration);
    }

    /// <summary>
    /// Animates a specified <paramref name="shaderParam"/>. All tweens use the Sine transition by default.
    /// 
    /// <code>
    /// tween.SetAnimatingShaderMaterial(shaderMaterial);
    /// tween.AnimateShader("blend_intensity", 1.0f, 2.0);
    /// </code>
    /// </summary>
    public RTween AnimateShader(string shaderParam, Variant finalValue, double duration)
    {
        if (_animatingShaderMaterial == null)
        {
            throw new Exception("Animating shader material has not been set");
        }

        _tweener = _tween
            .TweenProperty(_animatingShaderMaterial, $"shader_parameter/{shaderParam}", finalValue, duration)
            .SetTrans(Tween.TransitionType.Sine);

        return this;
    }

    /// <summary>
    /// Animates a specified <paramref name="property"/>. All tweens use the 
    /// Sine transition by default.
    /// 
    /// <code>
    /// tween.Animate(ColorRect.PropertyName.Color, Colors.Transparent, 0.5);
    /// </code>
    /// </summary>
    public RTween Animate(string property, Variant finalValue, double duration)
    {
        _tweener = _tween
            .TweenProperty(_node, property, finalValue, duration)
            .SetTrans(Tween.TransitionType.Sine);

        return this;
    }

    /// <summary>
    /// Sets the <paramref name="property"/> to be animated on
    /// 
    /// <code>
    /// tween.SetAnimatingProp(ColorRect.PropertyName.Color);
    /// tween.AnimateProp(Colors.Transparent, 0.5);
    /// </code>
    /// </summary>
    public RTween SetAnimatingProp(string property)
    {
        _animatingProperty = property;
        return this;
    }

    /// <summary>
    /// Sets the <paramref name="shaderMaterial"/> to be animated on
    /// 
    /// <code>
    /// tween.SetAnimatingShaderMaterial(shaderMaterial);
    /// tween.AnimateShader("blend_intensity", 1.0f, 2.0);
    /// </code>
    /// </summary>
    public RTween SetAnimatingShaderMaterial(ShaderMaterial shaderMaterial)
    {
        _animatingShaderMaterial = shaderMaterial;
        return this;
    }

    public RTween SetProcessMode(TweenProcessMode mode)
    {
        _tween = _tween.SetProcessMode(mode);
        return this;
    }

    /// <summary>
    /// Sets the animation to repeat
    /// </summary>
    public RTween Loop(int loops = 0)
    {
        _tween = _tween.SetLoops(loops);
        return this;
    }

    /// <summary>
    /// <para>Makes the next <see cref="Tweener"/> run parallelly to the previous one.</para>
    /// <para><b>Example:</b></para>
    /// <para><code>
    /// GTween tween = new(...);
    /// tween.Animate(...);
    /// tween.Parallel().Animate(...);
    /// tween.Parallel().Animate(...);
    /// </code></para>
    /// <para>All <see cref="Tweener"/>s in the example will run at the same time.</para>
    /// <para>You can make the <see cref="Tween"/> parallel by default by using <see cref="Tween.SetParallel(bool)"/>.</para>
    /// </summary>
    public RTween Parallel()
    {
        _tween = _tween.Parallel();
        return this;
    }

    /// <summary>
    /// <para>If <paramref name="parallel"/> is <see langword="true"/>, the <see cref="Tweener"/>s appended after this method will by default run simultaneously, as opposed to sequentially.</para>
    /// <para><code>
    /// tween.SetParallel()
    /// tween.Animate(...)
    /// tween.Animate(...)
    /// </code></para>
    /// </summary>
    public RTween SetParallel(bool parallel = true)
    {
        _tween = _tween.SetParallel(parallel);
        return this;
    }

    public RTween Callback(Action callback)
    {
        _tween.TweenCallback(Callable.From(callback));
        return this;
    }

    public RTween Delay(double duration)
    {
        _tween.TweenCallback(Callable.From(() => { /* Empty Action */ })).SetDelay(duration);
        return this;
    }

    /// <summary>
    /// A <paramref name="callback"/> is executed when the tween has finished
    /// </summary>
    public RTween Finished(Action callback)
    {
        _tween.Finished += callback;
        return this;
    }

    /// <summary>
    /// If the tween is looping, this can be used to stop it
    /// </summary>
    public RTween Stop()
    {
        _tween.Stop();
        return this;
    }

    /// <summary>
    /// Pause the tween
    /// </summary>
    public RTween Pause()
    {
        _tween.Pause();
        return this;
    }

    /// <summary>
    /// If the tween was paused with Pause(), resume it with Resume()
    /// </summary>
    public RTween Resume()
    {
        _tween.Play();
        return this;
    }

    /// <summary>
    /// Kill the tween
    /// </summary>
    public RTween Kill()
    {
        _tween?.Kill();
        return this;
    }

    public RTween SetTrans(TransitionType transType)
    {
        return UpdateTweener(nameof(SetTrans), () => _tweener.SetTrans(transType));
    }

    public RTween SetEase(EaseType easeType)
    {
        return UpdateTweener(nameof(SetEase), () => _tweener.SetEase(easeType));
    }

    public RTween TransLinear() => SetTrans(TransitionType.Linear);
    public RTween TransBack() => SetTrans(TransitionType.Back);
    public RTween TransSine() => SetTrans(TransitionType.Sine);
    public RTween TransBounce() => SetTrans(TransitionType.Bounce);
    public RTween TransCirc() => SetTrans(TransitionType.Circ);
    public RTween TransCubic() => SetTrans(TransitionType.Cubic);
    public RTween TransElastic() => SetTrans(TransitionType.Elastic);
    public RTween TransExpo() => SetTrans(TransitionType.Expo);
    public RTween TransQuad() => SetTrans(TransitionType.Quad);
    public RTween TransQuart() => SetTrans(TransitionType.Quart);
    public RTween TransQuint() => SetTrans(TransitionType.Quint);
    public RTween TransSpring() => SetTrans(TransitionType.Spring);

    public RTween EaseIn() => SetEase(EaseType.In);
    public RTween EaseOut() => SetEase(EaseType.Out);
    public RTween EaseInOut() => SetEase(EaseType.InOut);
    public RTween EaseOutIn() => SetEase(EaseType.OutIn);

    /// <summary>
    /// Checks if the tween is still playing
    /// </summary>
    public bool IsRunning()
    {
        return _tween.IsRunning();
    }

    private RTween UpdateTweener(string methodName, Action action)
    {
        if (_tweener == null)
        {
            throw new Exception($"Cannot call {methodName}() because no tweener has been set with tween.Animate(...)");
        }

        action();
        return this;
    }
}

