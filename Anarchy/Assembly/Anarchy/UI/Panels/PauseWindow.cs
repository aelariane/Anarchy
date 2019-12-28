using UnityEngine;

namespace Anarchy.UI
{
    internal class PauseWindow : GUIBase
    {
        const int Height = 90;
        const int Width= 200;
        private Rect windowRect;
        public float PauseWaitTime;

        public PauseWindow() : base("PauseWindow", 7809)
        {
            windowRect = Helper.GetScreenMiddle(Width, Height);
        }

        protected internal override void Draw()
        {
            string label = PauseWaitTime <= 3f ? locale.Format("unpausing", PauseWaitTime.ToString("F1")) : locale["pauseEnabled"];
            GUI.Box(windowRect, string.Empty);
            GUI.LabelCenter(windowRect, label);
        }

        public override void Update()
        {
            if (Time.timeScale <= 0.1f)
            {
                if (PauseWaitTime <= 3f)
                {
                    PauseWaitTime -= Time.deltaTime * 1000000f;
                    if (PauseWaitTime <= 0f)
                    {
                        PauseWaitTime = 0f;
                        Time.timeScale = 1f;
                        DisableImmediate();
                    }
                }
            }
        }
    }
}
