﻿namespace GodotUtils;

using Godot;
using System.Threading.Tasks;

public static class GU
{
    public static ServiceProvider Services { get; private set; }

    // Must be called externally from Game repository
    public static void Init(ServiceProvider services)
    {
        Services = services;
        Services.Add<Logger>();
    }

    //等待一帧的简化写法，常用于延迟操作。
    public async static Task WaitOneFrame(Node parent) =>
        await parent.ToSignal(
            source: parent.GetTree(),
            signal: SceneTree.SignalName.ProcessFrame);

    //生成指定范围的随机数，有两个重载，接受double和int类型的参数。
    public static float RandRange(double min, double max) =>
        (float)GD.RandRange(min, max);

    public static int RandRange(int min, int max) => GD.RandRange(min, max);

    //根据输入获取移动方向向量，可以处理控制器灵敏度，并标准化向量以防止沿对角线快速移动。
    public static Vector2 GetMovementInput(string prefix = "")
    {
        if (!string.IsNullOrWhiteSpace(prefix))
            prefix += "_";

        // GetActionStrength(...) supports controller sensitivity
        float inputHorz =
            Input.GetActionStrength($"{prefix}move_right") -
            Input.GetActionStrength($"{prefix}move_left");

        float inputVert =
            Input.GetActionStrength($"{prefix}move_down") -
            Input.GetActionStrength($"{prefix}move_up");

        // Normalize vector to prevent fast diagonal strafing
        return new Vector2(inputHorz, inputVert).Normalized();
    }

    //生成随机方向向量，长度可以指定。
    public static Vector2 RandDir(float dist = 1)
    {
        float theta = RandAngle();
        return new Vector2(Mathf.Cos(theta) * dist, Mathf.Sin(theta) * dist);
    }

    //生成随机角度。
    public static float RandAngle() => GU.RandRange(0, Mathf.Pi * 2);

    //创建矩形形状的Area2D节点。
    public static Area2D CreateAreaRect(Node parent, Vector2 size, string debugColor = "ff001300") =>
        CreateArea(parent, new RectangleShape2D { Size = size }, debugColor);

    //创建圆形形状的Area2D节点。
    public static Area2D CreateAreaCircle(Node parent, float radius, string debugColor = "ff001300") =>
        CreateArea(parent, new CircleShape2D { Radius = radius }, debugColor);

    //基本方法，创建带有指定形状的Area2D节点。
    public static Area2D CreateArea(Node parent, Shape2D shape, string debugColor = "ff001300")
    {
        var area = new Area2D();
        var areaCollision = new CollisionShape2D
        {
            DebugColor = new Color(debugColor),
            Shape = shape
        };

        area.AddChild(areaCollision);
        parent.AddChild(area);

        return area;
    }

    //验证字符串是否为指定范围内的有效数字，并更新LineEdit控件中的文本。
    public static void ValidateNumber(this string value, LineEdit input, int min, int max, ref int prevNum)
    {
        // do NOT use text.Clear() as it will trigger _on_NumAttempts_text_changed and cause infinite loop -> stack overflow
        if (string.IsNullOrEmpty(value))
        {
            prevNum = 0;
            EditInputText(input, "");
            return;
        }

        if (!int.TryParse(value.Trim(), out int num))
        {
            EditInputText(input, $"{prevNum}");
            return;
        }

        if (value.Length > max.ToString().Length && num <= max)
        {
            string spliced = value.Remove(value.Length - 1);
            prevNum = int.Parse(spliced);
            EditInputText(input, spliced);
            return;
        }

        if (num < min)
        {
            num = min;
            EditInputText(input, $"{min}");
        }

        if (num > max)
        {
            num = max;
            EditInputText(input, $"{max}");
        }

        prevNum = num;
    }

    //返回位于图片左侧的透明列数。
    public static int GetTransparentColumnsLeft(Image img, Vector2 size)
    {
        int columns = 0;

        for (int x = 0; x < size.X; x++)
        {
            for (int y = 0; y < size.Y; y++)
                if (img.GetPixel(x, y).A != 0)
                    return columns;

            columns++;
        }

        return columns;
    }

    public static int GetTransparentColumnsRight(Image img, Vector2 size)
    {
        int columns = 0;

        for (int x = (int)size.X - 1; x >= 0; x--)
        {
            for (int y = 0; y < size.Y; y++)
                if (img.GetPixel(x, y).A != 0)
                    return columns;

            columns++;
        }

        return columns;
    }

    public static int GetTransparentRowsTop(Image img, Vector2 size)
    {
        int rows = 0;

        for (int y = 0; y < size.Y; y++)
        {
            for (int x = 0; x < size.X; x++)
                if (img.GetPixel(x, y).A != 0)
                    return rows;

            rows++;
        }

        return rows;
    }

    public static int GetTransparentRowsBottom(Image img, Vector2 size)
    {
        int rows = 0;

        for (int y = (int)size.Y - 1; y >= 0; y--)
        {
            for (int x = 0; x < size.X; x++)
                if (img.GetPixel(x, y).A != 0)
                    return rows;

            rows++;
        }

        return rows;
    }

    static void EditInputText(LineEdit input, string text)
    {
        input.Text = text;
        input.CaretColumn = input.Text.Length;
    }
}
