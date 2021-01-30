using Anarchy.Configuration;
using System;
using UnityEngine;

namespace Anarchy.Skins.Maps
{
    internal abstract class LevelSkin : Skin
    {
        protected string[] skybox;
        protected string random = "";

        public override int DataLength => throw new NotImplementedException();

        protected LevelSkin(GameObject owner, string[] data) : base(owner, data)
        {
            skybox = new string[6];
            for (int i = data.Length - 6, j = 0; i < data.Length; i++, j++)
            {
                skybox[j] = data[i];
            }
            random = data[0];
            elements[0] = null;
        }

        protected void ApplySkybox()
        {
            if (!SkinSettings.SkyboxSkinsEnabled.Value)
            {
                return;
            }
            Material mat = Camera.main.GetComponent<Skybox>().material;
            int j = 0;
            for (int i = DataLength - 6; i < DataLength; i++, j++)
            {
                SkinElement skin = elements[i];
                if (skin != null && skin.IsDone)
                {
                    skin.Texture.wrapMode = TextureWrapMode.Clamp;
                    mat.SetTexture("_" + IndexToName(j) + "Tex", skin.Texture);
                }
            }
        }

        private string IndexToName(int index)
        {
            switch (index)
            {
                case 0:
                default:
                    return "Front";

                case 1:
                    return "Back";

                case 2:
                    return "Left";

                case 3:
                    return "Right";

                case 4:
                    return "Up";

                case 5:
                    return "Down";
            }
        }

        public override bool NeedReload(string[] data)
        {
            return true;
        }
    }
}