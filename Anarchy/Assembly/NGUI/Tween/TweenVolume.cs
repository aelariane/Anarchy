using UnityEngine;

[AddComponentMenu("NGUI/Tween/Volume")]
public class TweenVolume : UITweener
{
    private AudioSource mSource;
    public float from;

    public float to = 1f;

    public AudioSource audioSource
    {
        get
        {
            if (this.mSource == null)
            {
                this.mSource = base.audio;
                if (this.mSource == null)
                {
                    this.mSource = base.GetComponentInChildren<AudioSource>();
                    if (this.mSource == null)
                    {
                        Debug.LogError("TweenVolume needs an AudioSource to work with", this);
                        base.enabled = false;
                    }
                }
            }
            return this.mSource;
        }
    }

    public float volume
    {
        get
        {
            return this.audioSource.volume;
        }
        set
        {
            this.audioSource.volume = value;
        }
    }

    protected override void OnUpdate(float factor, bool isFinished)
    {
        this.volume = this.from * (1f - factor) + this.to * factor;
        this.mSource.enabled = (this.mSource.volume > 0.01f);
    }

    public static TweenVolume Begin(GameObject go, float duration, float targetVolume)
    {
        TweenVolume tweenVolume = UITweener.Begin<TweenVolume>(go, duration);
        tweenVolume.from = tweenVolume.volume;
        tweenVolume.to = targetVolume;
        if (duration <= 0f)
        {
            tweenVolume.Sample(1f, true);
            tweenVolume.enabled = false;
        }
        return tweenVolume;
    }
}