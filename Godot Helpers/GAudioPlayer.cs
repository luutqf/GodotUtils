namespace GodotUtils;

using Godot;

public class GAudioPlayer
{
    /// <summary>
    /// <para>
    /// Set the volume from a value of 0 to 100
    /// </para>
    /// 这个属性允许开发者设置音量，范围在 0 到 100 之间。实际的音量值会从这个范围映射到 Godot 可以接受的分贝值范围内 (-40 到 0 分贝)。如果设定的值为 0，则会将音量设为 -80 分贝，相当于静音。
    /// <para>
    /// The value will be auto converted to values
    /// Godot can work with
    /// </para>
    /// </summary>
    public float Volume
    {
        get => StreamPlayer.VolumeDb.Remap(-40, 0, 0, 100);
        set
        {
            float v = value.Remap(0, 100, -40, 0);

            if (value == 0)
                v = -80;

            StreamPlayer.VolumeDb = v;
        }
    }

    /// <summary>
    /// 这个属性用于检查音频是否正在播放，或者控制音频的播放和停止。
    /// </summary>
    public bool Playing
    {
        get => StreamPlayer.Playing;
        set => StreamPlayer.Playing = value;
    }

    /// <summary>
    /// 这个属性用于获取或设置音频流对象。
    /// </summary>
    public AudioStream Stream
    {
        get => StreamPlayer.Stream;
        set => StreamPlayer.Stream = value;
    }

    /// <summary>
    /// 这个属性用于获取或改变音频播放的音调。
    /// </summary>
    public float Pitch
    {
        get => StreamPlayer.PitchScale;
        set => StreamPlayer.PitchScale = value;
    }

    /// <summary>
    /// 这是一个公共属性，存储了内部的 AudioStreamPlayer 对象实例引用。
    /// </summary>
    public AudioStreamPlayer StreamPlayer { get; }

    /// <summary>
    /// 在类的构造函数中，它会创建一个新的 AudioStreamPlayer 对象，并且如果 deleteOnFinished 参数为 true，还会将该对象连接到一个当音频播放完成时将自己从场景中移除的回调方法。
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="deleteOnFinished"></param>
    public GAudioPlayer(Node parent, bool deleteOnFinished = false)
    {
        StreamPlayer = new AudioStreamPlayer();

        if (deleteOnFinished)
            StreamPlayer.Finished += () => StreamPlayer.QueueFree();

        parent.AddChild(StreamPlayer);
    }

    /// <summary>
    /// 这个方法封装了 AudioStreamPlayer 的 Play 方法，用于开始播放音频。
    /// </summary>
    public void Play() => StreamPlayer.Play();
}
