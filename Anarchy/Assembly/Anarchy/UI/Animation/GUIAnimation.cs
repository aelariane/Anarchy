using System;

namespace Anarchy.UI.Animation
{
    /// <summary>
    /// Base of GUI Animations
    /// </summary>
    public abstract class GUIAnimation
    {
        /// <summary>
        /// Owner <seealso cref="GUIBase"/>
        /// </summary>
        protected GUIBase owner;

        /// <summary>
        /// Initializes animation
        /// </summary>
        /// <param name="base">Owner of the animation</param>
        public GUIAnimation(GUIBase @base)
        {
            owner = @base;
        }

        /// <summary>
        /// Draws Close animation
        /// </summary>
        /// <returns><seealso cref="false"/> when animation is complete. <seealso cref="true"/> otherwise</returns>
        protected abstract bool Close();

        /// <summary>
        /// Calls before closing animation starts excuting
        /// </summary>
        protected virtual void OnStartClose()
        {
        }

        /// <summary>
        /// Calls before open animation starts executing
        /// </summary>
        protected virtual void OnStartOpen()
        {
        }

        /// <summary>
        /// Draws Open animation
        /// </summary>
        /// <returns><seealso cref="false"/> if animation is complete. <seealso cref="true"/> otherwise</returns>
        protected abstract bool Open();

        /// <summary>
        /// Starts closing animation
        /// </summary>
        /// <param name="onEnd"></param>
        /// <returns></returns>
        public Action StartClose(Action onEnd)
        {
            OnStartClose();
            Action act = () =>
            {
                if (Close())
                {
                    return;
                }
                onEnd();
                owner.OnGUI = null;
            };
            return act;
        }

        /// <summary>
        /// Starts opening animation
        /// </summary>
        /// <param name="onEnd"></param>
        /// <returns></returns>
        public Action StartOpen(Action onEnd)
        {
            OnStartOpen();
            Action act = () =>
            {
                if (Open())
                {
                    return;
                }
                onEnd();
                owner.OnGUI = owner.Draw;
            };
            return act;
        }
    }
}