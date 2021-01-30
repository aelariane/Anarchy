using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Anarchy.NameAnimation
{
    public class Fade : Animation
    {
        private float duration;

        public Fade(string name) : base(name)
        {
        }

        public override float Time { get; set; }
        protected override int Index { get; set; }
        public override List<Color> Colors { get; set; }

        public override IEnumerator Animate()
        {
            duration = Time / Colors.Count;

            while (Active)
            {
                for (int i = 0; i < Colors.Count - 1; i++)
                {
                    float t = Random.Range(0f, 1f);

                    Color sampleColor;
                    while (t < (1.0f + Mathf.Epsilon))
                    {
                        sampleColor = Color.Lerp(Colors[i], Colors[i + 1], t);
                        t += UnityEngine.Time.deltaTime / duration;
                        yield return null;
                    }

                    sampleColor = Color.Lerp(Colors[i], Colors[i + 1], 1.0f);
                }
                yield return new WaitForFixedUpdate();
            }
        }
    }
}