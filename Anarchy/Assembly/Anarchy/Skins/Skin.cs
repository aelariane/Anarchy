using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Anarchy.Skins
{
    internal abstract class Skin
    {
        public abstract int DataLength { get; }

        protected Dictionary<int, SkinElement> elements;

        public GameObject Owner { get; }

        public Skin(GameObject owner, string[] data)
        {
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            //if (data.Length != DataLength)
            //{
            //    Debug.Log($"Couldnt create the Skin because data.Length ({data.Length}) was bigger than allowed {DataLength}");
            //    throw new ArgumentException($"Invalid length of received data. Type {GetType().Name}, expected value: {DataLength}", nameof(data) + ".Length");
            //}
            elements = new Dictionary<int, SkinElement>();
            for (int i = 0; i < DataLength; i++)
            {
                elements.Add(i, new SkinElement(data[i], true));
            }
        }

        public abstract void Apply();

        public virtual void CheckReload(string[] data)
        {
            if (data.Length != DataLength)
            {
                return;
            }
            int i = 0;
            foreach (KeyValuePair<int, SkinElement> el in elements)
            {
                if (el.Value != null)
                {
                    el.Value.CheckReload(data[i]);
                }
                i++;
            }
        }

        public static void Check(Skin skin, string[] newData)
        {
            if (skin != null)
            {
                skin.CheckReload(newData);
                if (newData != null)
                {
                    FengGameManagerMKII.FGM.StartCoroutine(CheckRoutine(skin, newData));
                }
            }
        }

        private static System.Collections.IEnumerator CheckRoutine(Skin skin, string[] newData)
        {
            //Needed for Anarchy to not Crash on high TItanCount (Maybe increase?)
            //The better the net the lower it can be ig
            yield return new WaitForSeconds(5);
            if (skin.NeedReload(newData))
            {
                yield return FengGameManagerMKII.FGM.StartCoroutine(skin.Reload(newData));
            }
            skin.Apply();
        }

        public virtual bool NeedReload(string[] data)
        {
            //if (data.Length != DataLength)
            //{
            //    return false;
            //}
            int i = 0;
            foreach (SkinElement skin in elements.Values)
            {
                if (skin == null)
                {
                    continue;
                }
                if (skin.NeedReload || !skin.Path.Equals(data[i++]))
                {
                    return true;
                }
            }
            return false;
        }

        public virtual System.Collections.IEnumerator Reload(string[] newData)
        {
            for (int i = 0; i < elements.Count; i++)
            {
                SkinElement skin = elements[i];
                if (skin == null)
                {
                    continue;
                }
                if (skin.NeedReload || !skin.Path.Equals(newData[i]))
                {
                    SkinElement element = new SkinElement(newData[i], true);
                    yield return FengGameManagerMKII.FGM.StartCoroutine(element.TryLoad());
                    elements[i] = element;
                }
            }
            yield break;
        }

        protected void TryApplyTexture(SkinElement element, Renderer rend, bool canBeTransparent = false)
        {
            if (element != null && element.IsDone)
            {
                if (!canBeTransparent && element.IsTransparent)
                {
                    return;
                }
                if (canBeTransparent && element.IsTransparent)
                {
                    rend.enabled = false;
                }
                else
                {
                    if (element.Materials != null)
                    {
                        rend.material = element.Materials[0];
                        return;
                    }
                    element.Materials = new List<Material>(1);
                    rend.material.mainTexture = element.Texture;
                    element.Materials.Add(rend.material);
                }
            }
        }

        protected void TryApplyTextures(SkinElement element, Renderer[] renderers, bool canBeTransparent)
        {
            if (element != null && element.IsDone)
            {
                if (!canBeTransparent && element.IsTransparent)
                {
                    return;
                }
                if (element.Materials == null)
                {
                    element.Materials = new List<Material>(renderers.Select(x =>
                    {
                        x.material.mainTexture = element.Texture;
                        return x.material;
                    }));
                }
                else if(element.Materials.Count != renderers.Length)
                {
                    element.Materials = new List<Material>(renderers.Select(x =>
                    {
                        x.material.mainTexture = element.Texture;
                        return x.material;
                    }));
                }
                for (int i = 0; i < renderers.Length; i++)
                {
                    var render = renderers[i];
                    if (canBeTransparent && element.IsTransparent)
                    {
                        render.enabled = false;
                        continue;
                    }
                    render.material = element.Materials[i];
                }
            }
        }
    }
}