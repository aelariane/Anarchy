using UnityEngine;

namespace Anarchy.Custom.Scripts
{
    public class FadingScript : AnarchyCustomScript
    {
        public float FadeTimer { get; set; }
        public bool InitialState { get; set; }

        private float timer;
        private bool nextState;
        private float fadeTimer;

        public override void Launch()
        {
            base.Launch();
            nextState = !InitialState;
            fadeTimer = FadeTimer;
            timer = 0f;
            gameObject.SetActive(InitialState);
        }

        public override void OnUpdate()
        {
            timer += Time.deltaTime;
            if(timer > fadeTimer)
            {
                gameObject.SetActive(nextState);
                nextState = !nextState;
                timer = 0f;
            }
        }
    }
}
