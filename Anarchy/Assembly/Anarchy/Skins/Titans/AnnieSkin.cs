using UnityEngine;

namespace Anarchy.Skins.Titans
{
    internal class AnnieSkin : Skin
    {
        public override int DataLength => 1;

        public AnnieSkin(FEMALE_TITAN tit, string skin) : base(tit.gameObject, new string[] { skin })
        {
        }

        public override void Apply()
        {
            foreach (Renderer render in Owner.GetComponentsInChildren<Renderer>())
            {
                TryApplyTexture(elements[0], render);
            }
        }
    }
}