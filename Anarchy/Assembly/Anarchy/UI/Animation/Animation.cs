using System;

namespace Anarchy.UI.Animation
{
    public abstract class Animation
    {
        protected GUIBase myBase;

        public Animation(GUIBase @base)
        {
            myBase = @base;
        }

        protected abstract bool Close();

        protected virtual void OnStartClose() { }

        protected virtual void OnStartOpen() { }

        protected abstract bool Open();

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
                myBase.OnGUI = null;
            };
            return act;
        }

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
                myBase.OnGUI = myBase.Draw;
            };
            return act;
        }
    }
}