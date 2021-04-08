using UnityEngine;

namespace Anarchy.Custom.Scripts
{
    public class FadeScript : AnarchyCustomScript
    {
        public float DisabledTime { get; set; }
        public float ActiveTime { get; set; }
        public bool InitialState { get; set; }

        private float timer;
        private bool nextState;
        private float fadeTimer;

        public override void Launch()
        {
            base.Launch();
            nextState = !InitialState;
            fadeTimer = InitialState ? ActiveTime : DisabledTime;
            timer = 0f;
            gameObject.SetActive(InitialState);
        }

        public override void OnUpdate()
        {
            timer += Time.deltaTime;
            if(timer > fadeTimer)
            {
                gameObject.SetActive(nextState);
                fadeTimer = nextState ? ActiveTime : DisabledTime;
                nextState = !nextState;
                timer = 0f;
            }
        }
    }
}
