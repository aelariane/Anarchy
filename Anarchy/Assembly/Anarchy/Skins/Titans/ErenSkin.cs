using UnityEngine;

namespace Anarchy.Skins.Titans
{
    internal class ErenSkin : Skin
    {
        public override int DataLength => 1;

        public ErenSkin(TITAN_EREN tit, string skin) : base(tit.gameObject, new string[] { skin })
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