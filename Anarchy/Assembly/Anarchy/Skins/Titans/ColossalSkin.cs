using UnityEngine;

namespace Anarchy.Skins.Titans
{
    internal class ColossalSkin : Skin
    {
        public override int DataLength => 1;

        public ColossalSkin(COLOSSAL_TITAN tit, string skin) : base(tit.gameObject, new string[] { skin })
        {
        }

        public override void Apply()
        {
            foreach (Renderer render in Owner.GetComponentsInChildren<Renderer>())
            {
                if (render.name.Contains("hair"))
                {
                    TryApplyTexture(elements[0], render);
                }
            }
        }
    }
}