using Godot;
using GodotUtils.UI;

namespace GodotUtils;

public partial class AudioManager : Component
{
    private const float MinRandomPitch        = 0.8f;
    private const float MaxRandomPitch        = 1.2f;
    private const float RandomPitchThreshold  = 0.1f;
    private const int   MutedVolume           = -80;
    private const int   MutedVolumeNormalized = -40;

    private AudioStreamPlayer _musicPlayer;
    private ResourceOptions   _options;
    private Node              _sfxPlayersParent;
    private float             _lastPitch;

    public override void Ready()
    {
        _options = GetNode<OptionsManager>(AutoloadPaths.OptionsManager).Options;

        _musicPlayer = new AudioStreamPlayer();
        AddChild(_musicPlayer);

        _sfxPlayersParent = new Node();
        AddChild(_sfxPlayersParent);
    }

    public void PlayMusic(AudioStream song, bool instant = true, double fadeOut = 1.5, double fadeIn = 0.5)
    {
        if (!instant && _musicPlayer.Playing)
        {
            // Slowly transition to the new song
            PlayAudioCrossfade(_musicPlayer, song, _options.MusicVolume, fadeOut, fadeIn);
        }
        else
        {
            // Instantly switch to the new song
            PlayAudio(_musicPlayer, song, _options.MusicVolume);
        }
    }

    public void PlaySFX(AudioStream sound)
    {
        AudioStreamPlayer sfxPlayer = new()
        {
            Stream = sound,
            VolumeDb = NormalizeConfigVolume(_options.SFXVolume),
            PitchScale = GetRandomPitch()
        };

        sfxPlayer.Finished += sfxPlayer.QueueFree;

        _sfxPlayersParent.AddChild(sfxPlayer);
        sfxPlayer.Play();
    }

    public void FadeOutSFX(double fadeTime = 1)
    {
        foreach (AudioStreamPlayer audioPlayer in _sfxPlayersParent.GetChildren<AudioStreamPlayer>())
        {
            new GTween(audioPlayer).Animate(AudioStreamPlayer.PropertyName.VolumeDb, MutedVolume, fadeTime);
        }
    }

    public void SetMusicVolume(float volume)
    {
        _musicPlayer.VolumeDb = NormalizeConfigVolume(volume);
        _options.MusicVolume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        _options.SFXVolume = volume;

        float mappedVolume = NormalizeConfigVolume(volume);

        foreach (AudioStreamPlayer audioPlayer in _sfxPlayersParent.GetChildren())
        {
            audioPlayer.VolumeDb = mappedVolume;
        }
    }

    private void PlayAudio(AudioStreamPlayer player, AudioStream song, float volume)
    {
        player.Stream = song;
        player.VolumeDb = NormalizeConfigVolume(volume);
        player.Play();
    }

    private void PlayAudioCrossfade(AudioStreamPlayer player, AudioStream song, float volume, double fadeOut, double fadeIn)
    {
        new GTween(player)
            .SetAnimatingProp(AudioStreamPlayer.PropertyName.VolumeDb)
            .AnimateProp(MutedVolume, fadeOut).EaseIn()
            .Callback(() => PlayAudio(player, song, volume))
            .AnimateProp(NormalizeConfigVolume(volume), fadeIn).EaseIn();
    }

    private float NormalizeConfigVolume(float volume)
    {
        return volume == 0 ? MutedVolume : volume.Remap(0, 100, MutedVolumeNormalized, 0);
    }

    private float GetRandomPitch()
    {
        RandomNumberGenerator rng = new();
        rng.Randomize();

        float pitch = rng.RandfRange(MinRandomPitch, MaxRandomPitch);

        while (Mathf.Abs(pitch - _lastPitch) < RandomPitchThreshold)
        {
            rng.Randomize();
            pitch = rng.RandfRange(MinRandomPitch, MaxRandomPitch);
        }

        _lastPitch = pitch;
        return pitch;
    }
}
