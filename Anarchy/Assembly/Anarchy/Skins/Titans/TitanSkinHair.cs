using UnityEngine;

namespace Anarchy.Skins.Titans
{
    internal class TitanSkinHair : Skin
    {
        public override int DataLength => 1;

        public TitanSkinHair(GameObject owner, string hair) : base(owner, new string[] { hair })
        {
        }

        public override void Apply()
        {
            TryApplyTexture(elements[0], Owner.renderer, true);
        }
    }
}