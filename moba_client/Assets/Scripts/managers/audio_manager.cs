using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class audio_manager : UnitySingleton<audio_manager>
{
    private AudioSource music = null;
    private int max_effects = 10;

    private AudioClip now_music_clip = null;
    private bool now_music_loop = true;
    private Queue<AudioSource> effects = new Queue<AudioSource>();

    public void play_music(AudioClip clip, bool loop = true)
    {
        if (this.music == null
            || clip == null
            || (this.music.clip != null && this.music.clip.name.Equals(clip.name))) return;

        this.now_music_clip = clip;
        this.now_music_loop = loop;

        this.music.clip = clip;
        this.music.loop = loop;
        this.music.volume = 1.0f;
        this.music.Play();
    }

    public void stop_muisc()
    {
        if (this.music == null || this.music.clip == null) return;
        this.music.Stop();
    }

    public AudioSource play_effect(AudioClip clip, bool loop = false)
    {
        AudioSource source = this.effects.Dequeue();
        source.clip = clip;
        source.loop = loop;
        source.volume = 1.0f;
        source.Play();
        this.effects.Enqueue(source);
        return source;
    }

    public void enable_music(bool enable)
    {
        if (this.music == null || this.music.enabled == enable) return;

        this.music.enabled = enable;
        if (enable)
        {
            play_music(this.now_music_clip, this.now_music_loop);
        }
    }

    public void enable_effect(bool enable)
    {
        AudioSource[] effect_arr = effects.ToArray();
        int effect_count = effect_arr.Length;
        AudioSource tmp_audio = null;
        for (int i = 0; i < effect_count; i++)
        {
            tmp_audio = effect_arr[i];
            if (tmp_audio.enabled == enable) continue;

            tmp_audio.enabled = enable;
            if (enable)
            {
                tmp_audio.clip = null;
            }
        }
    }

    #region Unity Func
    public override void Awake()
    {
        base.Awake();
        this.music = this.gameObject.AddComponent<AudioSource>();
        for (int i = 0; i < max_effects; i++)
        {
            AudioSource source = this.gameObject.AddComponent<AudioSource>();
            this.effects.Enqueue(source);
        }
    }
    #endregion
}
