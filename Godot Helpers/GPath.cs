namespace GodotUtils;

using Godot;
using System.Linq;
using EaseType = Godot.Tween.EaseType;
using TransType = Godot.Tween.TransitionType;

/*
 * Create a path from a set of points with options to add curvature and
 * animate the attached sprite.
 * 创建一个路径从一组点与选项添加曲率和动画附加的精灵。
 */
public partial class GPath : Path2D
{
    public bool Rotates
    {
        get => pathFollow.Rotates;
        set => pathFollow.Rotates = value;
    }

    //一个 PathFollow2D 对象，用于在路径上跟随和导航。
    PathFollow2D pathFollow;
    
    //存储定义路径的点的数组。
    Vector2[] points;
    
    //一个 Sprite2D 对象，可以被附加到路径上。
    Sprite2D sprite;

    //一个 GTween 对象，用于执行动画。
    GTween tween;
    
    //存储每个点的补间（Tween）值的数组。
    float[] tweenValues;
    
    int tweenIndex;
    
    //分别定义动画的变换类型和缓动类型。
    TransType transType = TransType.Sine;
    EaseType easeType = EaseType.Out;
    
    //动画播放速度
    double animSpeed;
    
    //定义绘制路径线条的颜色、宽度和虚线样式。
    Color color;
    float width;
    int dashes;

    //构造函数接受一系列点、颜色、宽度、虚线数量和动画速度作为参数。这些参数被用来初始化路径和 PathFollow2D 对象，并将点添加到路径的曲线上。
    public GPath(Vector2[] points, Color color, int width = 5, int dashes = 0, double animSpeed = 1)
    {
        this.points = points;
        Curve = new Curve2D();
        pathFollow = new PathFollow2D { Rotates = false };
        tween = new GTween(pathFollow);
        AddChild(pathFollow);

        this.color = color;
        this.width = width;
        this.dashes = dashes;
        this.animSpeed = animSpeed;

        // Add points to the path
        for (int i = 0; i < points.Length; i++)
            Curve.AddPoint(points[i]);

        CalculateTweenValues();
    }

    //重写 Godot 的 _Draw 方法，在视图上绘制路径。路径可以是直线或虚线，取决于 dashes 属性。
    public override void _Draw()
    {
        Vector2[] points = Curve.GetBakedPoints();

        for (int i = 0; i < points.Length - 1; i += (dashes + 1))
        {
            Vector2 A = points[i];
            Vector2 B = points[i + 1];

            DrawLine(A, B, color, width, true);
        }
    }

    //设置 pathFollow 对象的进度，基于 tweenValues。
    public void SetLevelProgress(int v) => pathFollow.Progress = tweenValues[v - 1];

    // 使路径上的对象向目标索引处的位置动画化移动
    public void AnimateTo(int targetIndex)
    {
        if (targetIndex > tweenIndex)
            AnimateForwards(targetIndex - tweenIndex);
        else
            AnimateBackwards(tweenIndex - targetIndex);
    }

    //分别向前或向后动画化 pathFollow 对象。这取决于目标索引相对于当前索引的位置。
    public int AnimateForwards(int step = 1)
    {
        tweenIndex = Mathf.Min(tweenIndex + step, tweenValues.Count() - 1);
        Animate(true);
        return tweenIndex;
    }

    public int AnimateBackwards(int step = 1)
    {
        tweenIndex = Mathf.Max(tweenIndex - step, 0);
        Animate(false);
        return tweenIndex;
    }

    // 在路径上添加一个 Sprite2D 对象。
    public void AddSprite(Sprite2D sprite)
    {
        this.sprite = sprite;
        pathFollow.AddChild(sprite);
    }

    /// <summary>
    /// Add curves to the path. The curve distance is how far each curve is pushed
    /// out.
    /// 为路径添加曲线。这通过计算新的曲线点并插入到原有点之间来实现。
    /// </summary>
    public void AddCurves(int curveSize = 50, int curveDistance = 50)
    {
        // Add aditional points to make each line be curved
        int invert = 1;

        for (int i = 0; i < points.Length - 1; i++)
        {
            Vector2 A = points[i];
            Vector2 B = points[i + 1];

            Vector2 center = (A + B) / 2;
            Vector2 offset = ((B - A).Orthogonal().Normalized() * curveDistance * invert);
            Vector2 newPos = center + offset;

            // Switch between sides so curves flow more naturally
            invert *= -1;

            Vector4 v;

            // These values were found through trial and error
            // If you see a simpler pattern than this, please tell me lol
            if (B.Y >= A.Y)
                if (B.X >= A.X)
                    // Next point is under and after first point
                    v = new Vector4(-1, -1, 1, 1);
                else
                    // Next point is under and before first point
                    v = new Vector4(1, -1, -1, 1);
            else
                if (B.X <= A.X)
                // Next point is over and before first point
                v = new Vector4(1, 1, -1, -1);
            else
                // Next point is over and after first point
                v = new Vector4(-1, 1, 1, -1);

            int index = 1 + i * 2;

            // Insert the curved point at the index in the curve
            Curve.AddPoint(newPos,
                new Vector2(v.X, v.Y) * curveSize,
                new Vector2(v.Z, v.W) * curveSize, index);
        }

        // Since new points were added, the tween values need to be re-calulcated
        CalculateTweenValues();
    }

    //计算每个点的补间值，这些值用于控制 pathFollow 对象的动画。
    void CalculateTweenValues()
    {
        tweenValues = new float[points.Length];
        for (int i = 0; i < points.Length; i++)
            tweenValues[i] = Curve.GetClosestOffset(points[i]);
    }

    //执行动画，根据 transType、easeType 和动画方向进行调整。
    void Animate(bool forwards)
    {
        tween = new(this);
        tween.Animate("progress", tweenValues[tweenIndex],
            CalculateDuration(forwards)).SetTrans(transType).SetEase(easeType);
    }

    //计算动画的持续时间，基于剩余距离、动画速度和剩余等级图标的数量。
    double CalculateDuration(bool forwards)
    {
        // The remaining distance left to go from the current sprites progress
        float remainingDistance = Mathf.Abs(
            tweenValues[tweenIndex] - pathFollow.Progress);

        int startIndex = 0;

        // Dynamically calculate the start index
        for (int i = 0; i < tweenValues.Length; i++)
            if (pathFollow.Progress <= tweenValues[i])
            {
                startIndex = i;
                break;
            }

        // The number of level icons left to pass
        int levelIconsLeft = Mathf.Max(1, Mathf.Abs(tweenIndex - startIndex));

        double duration = remainingDistance / 150 / animSpeed / levelIconsLeft;

        return duration;
    }
}
