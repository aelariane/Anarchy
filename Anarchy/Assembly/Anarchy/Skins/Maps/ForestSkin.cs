using UnityEngine;

namespace Anarchy.Skins.Maps
{
    internal class ForestSkin : LevelSkin
    {
        public override int DataLength => 24;

        public ForestSkin(string[] data) : base(new GameObject(), data)
        {
        }

        public override void Apply()
        {
            ApplySkybox();
            int index = 0;
            GameObject[] objects = (GameObject[])GameObject.FindObjectsOfType(typeof(GameObject));
            foreach (GameObject go in objects)
            {
                if (go != null)
                {
                    if (go.name.Contains("TREE"))
                    {
                        int first, second;
                        int.TryParse(random[index++].ToString(), out first);
                        int.TryParse(random[index++].ToString(), out second);
                        if (first >= 8 || first < 0)
                        {
                            first = Random.Range(0, 8);
                        }
                        if (second >= 8 || second < 0)
                        {
                            second = Random.Range(0, 8);
                        }
                        Renderer[] rends = go.GetComponentsInChildren<Renderer>();
                        foreach (Renderer render in rends)
                        {
                            if (render.name.Contains("Cube"))
                            {
                                TryApplyTexture(elements[1 + first], render);
                            }
                            else if (render.name.Contains("Plane_031"))
                            {
                                TryApplyTexture(elements[9 + second], render, !GameModes.BombMode.Enabled);
                            }
                        }
                    }
                    else
                    {
                        if (go.name.Contains("Cube_001") && go.transform.parent.gameObject.tag != "Player")
                        {
                            Renderer[] rends = go.GetComponentsInChildren<Renderer>();
                            foreach (Renderer rend in rends)
                            {
                                if (!elements[17].IsTransparent)
                                {
                                    TryApplyTexture(elements[17], rend);
                                }
                            }
                        }
                    }
                }
            }

            Minimap.TryRecaptureInstance();
        }
    }
}