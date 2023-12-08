﻿using UnityEngine;

namespace Anarchy.Skins.Titans
{
    internal class TitanSkin : Skin
    {
        public override int DataLength => 2;

        public TitanSkin(TITAN go, string body, string eyes) : base(go.baseG, new string[2] { body, eyes })
        {
        }

        public override void Apply()
        {
            Renderer[] renders = Owner.GetComponentsInChildren<Renderer>();
            foreach (Renderer render in renders)
            {
                if (render.name.Contains("eye"))
                {
                    ApplyEye(render);
                }
                else if (render.name.Equals("hair"))
                {
                    ApplyBody(render);
                }
            }
        }

        private void ApplyEye(Renderer rend)
        {
            if (elements[1] != null && elements[1].IsDone)
            {
                TryApplyTexture(elements[1], rend, true);
                //idk why it works with this but it does...
                if (PhotonNetwork.IsMasterClient)
                    rend.material.mainTextureScale = new Vector2(rend.material.mainTextureScale.x * 2f, rend.material.mainTextureScale.y * 3f);
                else
                    rend.material.mainTextureScale = new Vector2(rend.material.mainTextureScale.x * 4f, rend.material.mainTextureScale.y * 8f); //RC
                rend.material.mainTextureOffset = new Vector2(0f, 0f);
            }
        }

        private void ApplyBody(Renderer rend)
        {
            // rend.material = Owner.GetComponent<TITAN>().mainMaterial.GetComponent<SkinnedMeshRenderer>().material;
            if (elements[0] != null && elements[0].IsDone)
            {
                TryApplyTexture(elements[0], rend);
            }
        }
    }
}