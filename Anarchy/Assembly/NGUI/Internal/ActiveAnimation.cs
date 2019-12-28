using NGUI;
using UnityEngine;

[AddComponentMenu("NGUI/Internal/Active Animation")]
[RequireComponent(typeof(Animation))]
public class ActiveAnimation : IgnoreTimeScale
{
    private Animation mAnim;
    private Direction mDisableDirection;
    private Direction mLastDirection;
    private bool mNotify;
    public string callWhenFinished;
    public GameObject eventReceiver;
    public ActiveAnimation.OnFinished onFinished;

    public delegate void OnFinished(ActiveAnimation anim);

    public bool isPlaying
    {
        get
        {
            if (this.mAnim == null)
            {
                return false;
            }
            foreach (object obj in this.mAnim)
            {
                AnimationState animationState = (AnimationState)obj;
                if (this.mAnim.IsPlaying(animationState.name))
                {
                    if (this.mLastDirection == Direction.Forward)
                    {
                        if (animationState.time < animationState.length)
                        {
                            return true;
                        }
                    }
                    else
                    {
                        if (this.mLastDirection != Direction.Reverse)
                        {
                            return true;
                        }
                        if (animationState.time > 0f)
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }

    private void Play(string clipName, Direction playDirection)
    {
        if (this.mAnim != null)
        {
            base.enabled = true;
            this.mAnim.enabled = false;
            if (playDirection == Direction.Toggle)
            {
                playDirection = ((this.mLastDirection == Direction.Forward) ? Direction.Reverse : Direction.Forward);
            }
            bool flag = string.IsNullOrEmpty(clipName);
            if (flag)
            {
                if (!this.mAnim.isPlaying)
                {
                    this.mAnim.Play();
                }
            }
            else if (!this.mAnim.IsPlaying(clipName))
            {
                this.mAnim.Play(clipName);
            }
            foreach (object obj in this.mAnim)
            {
                AnimationState animationState = (AnimationState)obj;
                if (string.IsNullOrEmpty(clipName) || animationState.name == clipName)
                {
                    float num = Mathf.Abs(animationState.speed);
                    animationState.speed = num * (float)playDirection;
                    if (playDirection == Direction.Reverse && animationState.time == 0f)
                    {
                        animationState.time = animationState.length;
                    }
                    else if (playDirection == Direction.Forward && animationState.time == animationState.length)
                    {
                        animationState.time = 0f;
                    }
                }
            }
            this.mLastDirection = playDirection;
            this.mNotify = true;
            this.mAnim.Sample();
        }
    }

    private void Update()
    {
        float num = base.UpdateRealTimeDelta();
        if (num == 0f)
        {
            return;
        }
        if (this.mAnim != null)
        {
            bool flag = false;
            foreach (object obj in this.mAnim)
            {
                AnimationState animationState = (AnimationState)obj;
                if (this.mAnim.IsPlaying(animationState.name))
                {
                    float num2 = animationState.speed * num;
                    animationState.time += num2;
                    if (num2 < 0f)
                    {
                        if (animationState.time > 0f)
                        {
                            flag = true;
                        }
                        else
                        {
                            animationState.time = 0f;
                        }
                    }
                    else if (animationState.time < animationState.length)
                    {
                        flag = true;
                    }
                    else
                    {
                        animationState.time = animationState.length;
                    }
                }
            }
            this.mAnim.Sample();
            if (flag)
            {
                return;
            }
            base.enabled = false;
            if (this.mNotify)
            {
                this.mNotify = false;
                if (this.onFinished != null)
                {
                    this.onFinished(this);
                }
                if (this.eventReceiver != null && !string.IsNullOrEmpty(this.callWhenFinished))
                {
                    this.eventReceiver.SendMessage(this.callWhenFinished, this, SendMessageOptions.DontRequireReceiver);
                }
                if (this.mDisableDirection != Direction.Toggle && this.mLastDirection == this.mDisableDirection)
                {
                    NGUITools.SetActive(base.gameObject, false);
                }
            }
        }
        else
        {
            base.enabled = false;
        }
    }

    public static ActiveAnimation Play(Animation anim, string clipName, Direction playDirection, EnableCondition enableBeforePlay, DisableCondition disableCondition)
    {
        if (!NGUITools.GetActive(anim.gameObject))
        {
            if (enableBeforePlay != EnableCondition.EnableThenPlay)
            {
                return null;
            }
            NGUITools.SetActive(anim.gameObject, true);
            UIPanel[] componentsInChildren = anim.gameObject.GetComponentsInChildren<UIPanel>();
            int i = 0;
            int num = componentsInChildren.Length;
            while (i < num)
            {
                componentsInChildren[i].Refresh();
                i++;
            }
        }
        ActiveAnimation activeAnimation = anim.GetComponent<ActiveAnimation>();
        if (activeAnimation == null)
        {
            activeAnimation = anim.gameObject.AddComponent<ActiveAnimation>();
        }
        activeAnimation.mAnim = anim;
        activeAnimation.mDisableDirection = (Direction)disableCondition;
        activeAnimation.eventReceiver = null;
        activeAnimation.callWhenFinished = null;
        activeAnimation.onFinished = null;
        activeAnimation.Play(clipName, playDirection);
        return activeAnimation;
    }

    public static ActiveAnimation Play(Animation anim, string clipName, Direction playDirection)
    {
        return ActiveAnimation.Play(anim, clipName, playDirection, EnableCondition.DoNothing, DisableCondition.DoNotDisable);
    }

    public static ActiveAnimation Play(Animation anim, Direction playDirection)
    {
        return ActiveAnimation.Play(anim, null, playDirection, EnableCondition.DoNothing, DisableCondition.DoNotDisable);
    }

    public void Reset()
    {
        if (this.mAnim != null)
        {
            foreach (object obj in this.mAnim)
            {
                AnimationState animationState = (AnimationState)obj;
                if (this.mLastDirection == Direction.Reverse)
                {
                    animationState.time = animationState.length;
                }
                else if (this.mLastDirection == Direction.Forward)
                {
                    animationState.time = 0f;
                }
            }
        }
    }
}