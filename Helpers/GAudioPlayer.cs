using Godot;

namespace GodotUtils;

public class GAudioPlayer
{
    /// <summary>
    /// <para>
    /// Set the volume from a value of 0 to 100
    /// </para>
    /// 
    /// <para>
    /// The value will be auto converted to values
    /// Godot can work with
    /// </para>
    /// </summary>
    public float Volume
    {
        get => Internal.VolumeDb.Remap(-40, 0, 0, 100);
        set
        {
            float v = value.Remap(0, 100, -40, 0);

            if (value == 0)
            {
                v = -80;
            }

            Internal.VolumeDb = v;
        }
    }

    public bool Playing
    {
        get => Internal.Playing;
        set => Internal.Playing = value;
    }

    public AudioStream Stream
    {
        get => Internal.Stream;
        set => Internal.Stream = value;
    }

    public float Pitch
    {
        get => Internal.PitchScale;
        set => Internal.PitchScale = value;
    }

    public AudioStreamPlayer Internal { get; }

    public GAudioPlayer(Node parent, bool deleteOnFinished = false)
    {
        Internal = new AudioStreamPlayer();

        if (deleteOnFinished)
        {
            Internal.Finished += () => Internal.QueueFree();
        }

        parent.AddChild(Internal);
    }

    public void Play()
    {
        Internal.Play();
    }
}
