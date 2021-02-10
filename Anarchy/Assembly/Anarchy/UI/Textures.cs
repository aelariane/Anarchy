using System.Collections.Generic;
using UnityEngine;
using static Anarchy.UI.ElementType;

namespace Anarchy.UI
{
    internal class Textures
    {
        private static readonly ElementType[] order = new ElementType[7] { Box, Button, SelectionGrid, Slider, SliderBody, TextField, Toggle };
        private static readonly int[] stateCounts = new int[7] { 1, 3, 6, 6, 6, 3, 6 };
        internal static Dictionary<ElementType, Texture2D[]> TextureCache;

        public static Texture2D[] Test()
        {
            Color main = Style.BackgroundColor;
            Color[] colors = Helper.TextureColors(main, 6);
            List<Texture2D> liest = new List<Texture2D>();
            Texture2D it = new Texture2D(20, 20, TextureFormat.ARGB32, false);
            for (int x = 0; x < 20; x++)
            {
                for (int y = 0; y < 20; y++)
                {
                    it.SetPixel(x, y, main);
                }
            }
            it.Apply();
            liest.Add(it);
            for (int j = 0; j < 6; j++)
            {
                //int offset = Mathf.RoundToInt(4 * UIManager.HUDScaleGUI);
                //int baseVal = Mathf.RoundToInt(15 * UIManager.HUDScaleGUI);
                Texture2D tex = new Texture2D(20, 20, TextureFormat.ARGB32, false);
                for(int x = 0; x < 20; x++)
                {
                    for (int y = 0; y < 20; y++)
                    {
                        tex.SetPixel(x, y, colors[j]);
                    }
                }
                //int xPos = 0 + offset;
                //int xPos1 = baseVal - offset;
                //for (int x = 0; x < baseVal; x++)
                //{
                //    for (int y = 0; y < baseVal; y++)
                //    {
                //        if (x >= xPos && x <= xPos1)
                //        {
                //            tex.SetPixel(x, y, colors[j]);
                //            continue;
                //        }
                //        tex.SetPixel(x, y, Optimization.Caching.Colors.empty);
                //    }
                //}
                tex.Apply();
                liest.Add(tex);
            }

            return liest.ToArray();
        }

        public static void Initialize()
        {
            if (TextureCache == null)
            {
                TextureCache = new Dictionary<ElementType, Texture2D[]>();
            }
            TextureCache.Clear();
            Color main = Style.BackgroundColor;
            for (int i = 0; i < order.Length; i++)
            {
                Texture2D[] local = new Texture2D[stateCounts[i]];
                switch (order[i])
                {
                    case ElementType.Box:
                        Texture2D box = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                        box.SetPixel(0, 0, Style.BackgroundColor);
                        box.Apply();
                        local[i] = box;
                        break;

                    case ElementType.Slider:
                        {
                            for (int j = 0; j < stateCounts[i]; j++)
                            {
                                int offset = Mathf.RoundToInt(6 * UIManager.HUDScaleGUI);
                                int baseVal = Mathf.RoundToInt(15 * UIManager.HUDScaleGUI);
                                Texture2D tex = new Texture2D(1, baseVal, TextureFormat.ARGB32, false);
                                int yPos = 0 + offset;
                                int yPos1 = baseVal - 1 - offset;
                                for (int y = 0; y < baseVal; y++)
                                {
                                    if (y >= yPos && y <= yPos1)
                                    {
                                        tex.SetPixel(0, y, Optimization.Caching.Colors.black);
                                        continue;
                                    }
                                    tex.SetPixel(0, y, Optimization.Caching.Colors.empty);
                                }
                                tex.Apply();
                                local[j] = tex;
                            }
                        }
                        break;

                    case ElementType.SliderBody:
                        {
                            Color[] colors = Helper.TextureColors(main, stateCounts[i]);
                            for (int j = 0; j < stateCounts[i]; j++)
                            {
                                int offset = Mathf.RoundToInt(4 * UIManager.HUDScaleGUI);
                                int baseVal = Mathf.RoundToInt(15 * UIManager.HUDScaleGUI);
                                Texture2D tex = new Texture2D(baseVal, baseVal, TextureFormat.ARGB32, false);
                                int yPos = 0 + offset;
                                int yPos1 = baseVal - offset;
                                for (int x = 0; x < baseVal; x++)
                                {
                                    for (int y = 0; y < baseVal; y++)
                                    {
                                        if (y >= yPos && y <= yPos1)
                                        {
                                            tex.SetPixel(x, y, colors[j]);
                                            continue;
                                        }
                                        tex.SetPixel(x, y, Optimization.Caching.Colors.empty);
                                    }
                                }
                                tex.Apply();
                                local[j] = tex;
                            }
                        }
                        break;

                    default:
                        {
                            Color[] colors = Helper.TextureColors(main, stateCounts[i]);
                            for (int j = 0; j < stateCounts[i]; j++)
                            {
                                Texture2D tex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                                tex.SetPixel(0, 0, colors[j]);
                                tex.Apply();
                                local[j] = tex;
                            }
                        }
                        break;
                }
                TextureCache.Add(order[i], local);
            }
        }
    }
}