namespace GodotUtils;

using Godot;
using System;

/// <summary>
/// The created GTween should be defined in _Ready() if it is going to be re-used
/// several times with GTween.Create()
/// </summary>
public class GTween
{
    //储存一个关于Tween实例的引用，即控制动画的对象。
    Tween tween;
    
    //储存要被动画化的Godot节点的引用。
    Node node;

    /// <summary>
    /// 接受Node类型的参数，并将其赋值给类的node属性。
    /// 调用Kill()方法来确保没有现存的动画在运行。
    /// 使用CreateTween()方法，基于节点初始化一个新的Tween实例。
    /// 设置tween的处理模式为物理（Physics），这样可以保持摄像机追踪移动物体时不会出现滞后。
    /// </summary>
    /// <param name="node"></param>
    public GTween(Node node)
    {
        this.node = node;
        Kill();
        tween = node.CreateTween();

        // This helps to prevent the camera from lagging behind the players movement
        tween.SetProcessMode(Tween.TweenProcessMode.Physics);
    }

    //允许用户改变tween的处理模式。
    public void SetProcessMode(Tween.TweenProcessMode mode) =>
        tween.SetProcessMode(mode);

    //将循环次数设置为1，用以停止循环动画
    public void StopLooping() => tween.SetLoops(1);
    
    //设置tween的循环次数，如果不指明则默认为无限循环。
    public void Loop(int loops = 0) => tween.SetLoops(loops);

    //提供节点颜色变化的动画功能。如果节点是ColorRect类型则改变其color属性，否则改变modulate或self_modulate属性，取决于是否希望这种颜色变化影响节点的子节点。这个方法也允许动画并行执行。
    public PropertyTweener AnimateColor(Color color, double duration, bool modulateChildren = false, bool parallel = false)
    {
        if (node is ColorRect)
            return Animate("color", color, duration, parallel);
        else
        {
            if (modulateChildren)
            {
                return Animate("modulate", color, duration, parallel);
            }
            else
            {
                return Animate("self_modulate", color, duration, parallel);
            }
        }
    }

    //提供对任何可动画属性的动画。允许设置属性、目标值、动画持续时间，并可以选择是否并行动画。
    public PropertyTweener Animate(string prop, Variant finalValue, double duration, bool parallel = false) =>
        parallel ?
            tween.Parallel().TweenProperty(node, prop, finalValue, duration) :
            tween.TweenProperty(node, prop, finalValue, duration);

    //用于在tween上添加一个回调，当某个点达到时执行。这个方法也支持并行。
    public CallbackTweener Callback(Action callback, bool parallel = false)
    {
        if (!parallel)
            return tween.TweenCallback(Callable.From(callback));
        else
            return tween.Parallel().TweenCallback(Callable.From(callback));
    }

    //用于设置动画执行前的延时。
    public void Delay(double duration) =>
        tween.TweenCallback(Callable.From(() => { /* Empty Action */ })).SetDelay(duration);

    //添加一个当tween完成时触发的回调。
    public void Finished(Action callback) => tween.Finished += callback;
    //返回一个布尔值，指示tween是否正在运行中。
    public bool IsRunning() => tween.IsRunning();
    
    //停止tween的执行。
    public void Stop() => tween.Stop();
    
    //暂停tween。
    public void Pause() => tween.Pause();
    
    //重新开始或继续tween。
    public void Resume() => tween.Play();
    
    //安全地销毁现有的tween。
    public void Kill() => tween?.Kill();
}
