using UnityEngine;

namespace Anarchy.UI
{
    internal class PauseWindow : GUIBase
    {
        private const int Height = 90;
        private const int Width = 200;
        private Rect windowRect;
        public float PauseWaitTime;

        public PauseWindow() : base(nameof(PauseWindow), GUILayers.PauseWindow)
        {
            windowRect = Helper.GetScreenMiddle(Width, Height);
            base.animator = new Animation.NoneAnimation(this);
        }

        protected internal override void Draw()
        {
            string label = PauseWaitTime <= 3f ? locale.Format("unpausing", PauseWaitTime.ToString("F1")) : locale["pauseEnabled"];
            GUI.Box(windowRect, string.Empty);
            GUI.LabelCenter(windowRect, label);
        }

        protected override void OnDisable()
        {
            PauseWaitTime = 0f;
            Time.timeScale = 1f;
        }

        public override void Update()
        {
            if (Time.timeScale <= 0.1f)
            {
                if (PauseWaitTime <= 3f)
                {
                    PauseWaitTime -= Time.deltaTime * 100000f;
                    if (PauseWaitTime <= 0f)
                    {
                        PauseWaitTime = 0f;
                        Time.timeScale = 1f;
                        DisableImmediate();
                    }
                }
            }
        }

        public override void OnUpdateScaling()
        {
            windowRect = Helper.GetScreenMiddle(Width, Height);
        }
    }
}