using UnityEngine;

namespace Anarchy.Skins.Maps
{
    internal class CustomMapSkin : Skin
    {
        public override int DataLength => 7;

        public CustomMapSkin(string[] data) : base(new GameObject(), data)
        {
        }

        public override void Apply()
        {
            var mat = Camera.main.GetComponent<Skybox>().material;
            for (int i = 0; i < DataLength - 1; i++)
            {
                SkinElement skin = elements[i];
                if (skin != null && skin.IsDone)
                {
                    mat.SetTexture("_" + IndexToName(i) + "Tex", skin.Texture);
                }
            }
            SkinElement groundSkin = elements[6];
            System.Collections.Generic.List<Renderer> tmp = new System.Collections.Generic.List<Renderer>();
            foreach (GameObject go in RC.CustomLevel.groundList)
            {
                if (go != null && go.renderer != null)
                {
                    Renderer[] renders = go.GetComponentsInChildren<Renderer>();
                    foreach (Renderer render in renders)
                    {
                        if (render != null)
                        {
                            tmp.Add(render);
                        }
                    }
                }
            }
            if (groundSkin == null || !groundSkin.IsDone || tmp.Count < 1)
            {
                return;
            }

            foreach (Renderer render in tmp)
            {
                if (render != null)
                {
                    if (groundSkin.Path.ToLower() == "transparent")
                    {
                        render.enabled = false;
                        continue;
                    }
                    TryApplyTexture(groundSkin, render, false);
                }
            }

            Minimap.TryRecaptureInstance();
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
    }
}