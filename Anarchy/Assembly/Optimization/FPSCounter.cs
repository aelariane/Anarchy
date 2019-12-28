using UnityEngine;

namespace Optimization
{
    public class FPSCounter
    {
        private int fps = 0;
        private float time = 1f;

        public void FPSUpdate()
        {
            time -= Time.deltaTime;
            fps++;
            if (time <= 0f)
            {
                time = 1f;
                FPS = fps;
                fps = 0;
            }
        }

        public void Reset()
        {
            time = 1f;
            fps = 0;
        }

        internal int FPS { get; private set; }
    }
}
