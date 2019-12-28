using UnityEngine;

[AddComponentMenu("NGUI/Interaction/Button Sound")]
public class UIButtonSound : MonoBehaviour
{
    public AudioClip audioClip;

    public float pitch = 1f;
    public UIButtonSound.Trigger trigger;

    public float volume = 1f;

    public enum Trigger
    {
        OnClick,
        OnMouseOver,
        OnMouseOut,
        OnPress,
        OnRelease
    }

    private void OnClick()
    {
        if (base.enabled && this.trigger == UIButtonSound.Trigger.OnClick)
        {
            NGUITools.PlaySound(this.audioClip, this.volume, this.pitch);
        }
    }

    private void OnHover(bool isOver)
    {
        if (base.enabled && ((isOver && this.trigger == UIButtonSound.Trigger.OnMouseOver) || (!isOver && this.trigger == UIButtonSound.Trigger.OnMouseOut)))
        {
            NGUITools.PlaySound(this.audioClip, this.volume, this.pitch);
        }
    }

    private void OnPress(bool isPressed)
    {
        if (base.enabled && ((isPressed && this.trigger == UIButtonSound.Trigger.OnPress) || (!isPressed && this.trigger == UIButtonSound.Trigger.OnRelease)))
        {
            NGUITools.PlaySound(this.audioClip, this.volume, this.pitch);
        }
    }
}