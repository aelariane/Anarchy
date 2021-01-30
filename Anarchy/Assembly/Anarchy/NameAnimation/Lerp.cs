using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Anarchy.NameAnimation
{
    public class Lerp : Animation
    {
        private float duration;

        public Lerp(string name) : base(name)
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
                    float t = 0.0f;

                    Color sampleColor;
                    while (t < (1.0f + Mathf.Epsilon))
                    {
                        sampleColor = Color.Lerp(Colors[i], Colors[i + 1], t);
                        t += UnityEngine.Time.deltaTime / duration;
                        Name = "[" + ToRgbHex(sampleColor) + "]" + Name.RemoveHex();
                        PhotonNetwork.player.UIName = Name;
                        yield return null;
                    }

                    sampleColor = Color.Lerp(Colors[i], Colors[i + 1], 1.0f);
                    Name = "[" + ToRgbHex(sampleColor) + "]" + Name.RemoveHex();
                    PhotonNetwork.player.UIName = Name;
                }
                yield return new WaitForFixedUpdate();
            }
        }
    }
}