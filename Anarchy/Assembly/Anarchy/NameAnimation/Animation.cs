using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Anarchy.NameAnimation
{
    public abstract class Animation
    {
        protected string Name;

        protected Animation(string name)
        {
            Name = name;
        }

        public bool Active { get; set; }
        public abstract float Time { get; set; }
        public abstract List<Color> Colors { get; set; }

        public abstract IEnumerator Animate();

        protected abstract int Index { get; set; }

        protected static string ToRgbHex(Color c)
        {
            return $"{ToByte(c.r):X2}{ToByte(c.g):X2}{ToByte(c.b):X2}";
        }

        private static byte ToByte(float f)
        {
            f = Mathf.Clamp01(f);
            return (byte)(f * 255);
        }
    }
}