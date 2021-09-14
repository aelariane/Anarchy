using System.IO;
using System.Linq;
using UnityEngine;

namespace Anarchy.UI
{
    /// <summary>
    /// Set of UI Helper methods
    /// </summary>
    public static class Helper
    {
        #region Useless stuff

        //public static void AntiAliasing(Texture2D tex, int anisolevel)
        //{
        //    byte[,] matrixR = new byte[tex.height, tex.width];
        //    byte[,] matrixG = new byte[tex.height, tex.width];
        //    byte[,] matrixB = new byte[tex.height, tex.width];
        //    byte[,] matrixA = new byte[tex.height, tex.width];
        //    for (int y = 0; y < tex.height; y++)
        //    {
        //        for(int x = 0; x < tex.width; x++)
        //        {
        //            Color32 clr = tex.GetPixel(x, y);
        //            matrixR[y, x] = clr.r;
        //            matrixG[y, x] = clr.g;
        //            matrixB[y, x] = clr.b;
        //            matrixA[y, x] = clr.a;
        //        }
        //    }
        //    Replace(matrixR, tex.width, tex.height);
        //    Replace(matrixG, tex.width, tex.height);
        //    Replace(matrixB, tex.width, tex.height);
        //    Replace(matrixA, tex.width, tex.height);
        //    for (int y = 0; y < tex.height; y++)
        //    {
        //        for (int x = 0; x < tex.width; x++)
        //        {
        //            tex.SetPixel(x, y, new Color32(matrixR[y, x], matrixG[y, x], matrixB[y, x], matrixA[y, x]));
        //        }
        //    }
        //    tex.Apply();
        //}

        //private static void CheckPixel(Texture2D tex, int x, int y, int width, int height)
        //{
        //    Color prevX = tex.GetPixel(x - 1, y);
        //    Color prevY = tex.GetPixel(x, y - 1);
        //    float rX = prevX.r / prevY.r;
        //    float gX = prevX.g / prevY.g;
        //    float bX = prevX.b / prevY.b;
        //    float aX = prevX.a / prevY.a;
        //    float rY = 1f - rX;
        //    float gY = 1f - gX;
        //    float bY = 1f - bX;
        //    float aY = 1f - aX;
        //    if ((aX * prevX.a + aY * prevY.a) /2f < 0.3f)
        //        return;
        //    Color newColor = new Color((rX * prevX.r + rY * prevY.r) / 2f, (gX * prevX.g + gY * prevY.g) /2f, (bX * prevX.b + bY * prevY.b) /2f, (aX * prevX.a + aY * prevY.a) /2f);
        //    tex.SetPixel(x, y, newColor);
        //}

        //private static void Replace(byte[,] matrix, int width, int height)
        //{
        //    for (int y = 1; y < height; y++)
        //    {
        //        for (int x = 1; x < width; x++)
        //        {
        //            byte current = matrix[y, x];
        //            byte prevX = matrix[y, x - 1];
        //            byte prevY = matrix[y - 1, x];
        //            float k = (float)prevX / (float)prevY;
        //            matrix[y, x] = (byte)(((byte)(prevX * (k / 2f))) + ((byte)(prevY * ( 1-k / 2f))));
        //        }
        //    }
        //}

        //private static void Plot(Texture2D tex, int x, int y, bool isX = true)
        //{
        //    Color clr = tex.GetPixel(x, y);
        //    Color clr2;
        //    if (isX)
        //    {
        //        clr2 = tex.GetPixel(x - 1, y);
        //    }
        //    else
        //    {
        //        clr2 = tex.GetPixel(x, y - 1);
        //    }
        //    if (clr == clr2)
        //        return;
        //    float deltaR = ((1f - clr2.r) + (1f - clr.r)) / 2f;
        //    float deltaG = ((1f - clr2.g) + (1f - clr.g)) / 2f;
        //    float deltaB = ((1f - clr2.b) + (1f - clr.b)) / 2f;
        //    float deltaA = ((1f - clr2.a) + (1f - clr.a)) / 2f;
        //    //float newR = (clr2.r + clr.r) / 2f;
        //    //float newG = (clr2.g + clr.g) / 2f;
        //    //float newB = (clr2.b + clr.b) / 2f;
        //    //float newA = clr2.a -= 0.1f;
        //    if(clr2.a + deltaR >= 0.2f)
        //        tex.SetPixel(x, y, new Color(clr2.r + deltaR, clr2.g + deltaG, clr2.b + deltaB, clr2.a + deltaA));
        //}

        //public static Texture2D CombineTextures(Texture2D[] textures)
        //{
        //    if (textures.Length < 2)
        //    {
        //        throw new System.Exception("Cannot combine less then 2 textures");
        //    }
        //    Texture2D result = RectAngle(textures[0].width, textures[0].height, Optimization.Caching.Colors.empty);
        //    for(int i = 0; i < textures.Length; i++)
        //    {
        //        Texture2D src = textures[i];
        //        for(int y = 0; y < result.height; y++)
        //        {
        //            for(int x = 0; x < result.width; x++)
        //            {
        //                Color clr = src.GetPixel(x, y);
        //                if(clr.a != 0f)
        //                {
        //                    result.SetPixel(x, y, clr);
        //                }
        //            }
        //        }
        //    }
        //    return result;
        //}

        //private static bool FillCheckEnd(Texture2D tex, int x, int y, int end)
        //{
        //    for (int i = x; i < end; i++)
        //    {
        //        if (tex.GetPixel(i, y).a != 0f)
        //        {
        //            return false;
        //        }
        //    }
        //    return true;
        //}

        //public static void FillTemplate(Texture2D tex, Color color)
        //{
        //    int width = tex.width;
        //    int height = tex.height;
        //    Color empty = Optimization.Caching.Colors.empty;
        //    for (int y = 0; y < height; y++)
        //    {
        //        for (int x = 0; x < width; x++)
        //        {
        //            if (tex.GetPixel(x, y).a != 0f)
        //            {
        //                x += FillLine(tex, y, x + 1, color);
        //                continue;
        //            }
        //        }
        //    }
        //    tex.Apply();
        //}

        //private static int FillLine(Texture2D tex, int y, int x, Color color)
        //{
        //    int result = 0;
        //    if (FillCheckEnd(tex, x, y, tex.width))
        //        return tex.width - x;
        //    Color empty = Optimization.Caching.Colors.empty;
        //    for (int i = x; i < tex.width; i++)
        //    {
        //        if (tex.GetPixel(i, y).a != 0f)
        //        {
        //            break;
        //        }
        //        tex.SetPixel(i, y, color);
        //        result++;
        //    }
        //    return result;
        //}

        //internal delegate int Func(int x);

        //public static Texture2D Triangle(int width, int height, Color color, bool up, bool right, bool full = true, int boldness = 1)
        //{
        //    Texture2D result = TriangleTemplate(width, height, color, up, right, boldness);
        //    if (full)
        //    {
        //        FillTemplate(result, color);
        //    }
        //    result.Apply();
        //    return result;
        //}

        //internal static Texture2D TriangleTemplate(int width, int height, Color color, bool up, bool right, int boldness, bool edges = true)
        //{
        //    Texture2D result = RectAngle(width, height, Optimization.Caching.Colors.empty);
        //    int[] marks = new int[width];
        //    Func func = GetFunc(width, height, up, right);
        //    for (int x = 0; x < width; x++)
        //    {
        //        int y = func(x);
        //        if (y >= height)
        //            y = height - 1;
        //        marks[x] = y;
        //        result.SetPixel(x, y, color);

        //    }
        //    if (edges)
        //    {
        //        int incX = right ? 1 : -1;
        //        int incY = up ? 1 : -1;
        //        int startYGlobal = up ? 0 : height - 1;
        //        int startXGlobal = right ? 0 : width - 1;
        //        for (int i = 0; i < boldness; i++)
        //        {
        //            int endX = right ? (width - (i * 2) - 1) : (i * 2) + 1;
        //            int endY = marks[right ? i : width - i - 1] + (up ? -i : i);
        //            int startY = startYGlobal + (up ? i : -i);
        //            int startX = startXGlobal + (right ? i : -i);
        //            int posY = right ? i : width - i - 1;
        //            int posX = up ? i : height - i - 1;
        //            for (int x = startX; x != endX;)
        //            {
        //                result.SetPixel(x, startY, color);
        //                x += incX;
        //            }
        //            for (int y = startY; y != endY;)
        //            {
        //                result.SetPixel(startX, y, color);
        //                y += incY;
        //            }
        //            for (int x = 0; x < width; x++)
        //            {
        //                int y = marks[x] - (up ? i : -i);
        //                if ((up && y < 0) || (!up && y > height - 1))
        //                    continue;
        //                result.SetPixel(x, y, color);
        //            }
        //        }
        //    }
        //    result.Apply();
        //    return result;
        //}

        //private static Func GetFunc(int width, int height, bool up, bool right)
        //{
        //    bool neg = (up && right) || (!up && !right);
        //    float k = ((float)height / (float)width);
        //    float b = 0f;
        //    if (neg)
        //    {
        //        k *= -1f;
        //        b = height;
        //    }
        //    return delegate (int x)
        //    {
        //        return Mathf.RoundToInt(k * x + b);
        //    };
        //}

        #endregion Useless stuff
        /// <summary>
        /// Applies given parameters to style   
        /// </summary>
        /// <param name="style"></param>
        /// <param name="anchor"></param>
        /// <param name="fontStyle"></param>
        /// <param name="fontSize"></param>
        /// <param name="wordWrap"></param>

        public static void ApplyStyle(this GUIStyle style, TextAnchor anchor, FontStyle fontStyle, int fontSize, bool wordWrap)
        {
            if (style == null)
            {
                return;
            }

            style.alignment = anchor;
            style.fontStyle = fontStyle;
            style.fontSize = fontSize;
            style.wordWrap = wordWrap;
            style.padding = new RectOffset(1, 1, 1, 1);
            style.margin = new RectOffset(Style.HorizontalMargin, Style.HorizontalMargin, Style.VerticalMargin, Style.VerticalMargin);
            style.border = new RectOffset(0, 0, 0, 0);
        }

        public static void ApplyStyle(this GUIStyle style, TextAnchor anchor, FontStyle fstyle, int fontSize, bool wordWrap, Color color)
        {
            if (style == null)
            {
                return;
            }

            style.ApplyStyle(anchor, fstyle, fontSize, wordWrap, new Color[6].Select(x => color).ToArray());
        }

        public static void ApplyStyle(this GUIStyle res, TextAnchor anchor, FontStyle style, int fontSize, bool wordWrap, Color[] colors)
        {
            if (res == null)
            {
                return;
            }

            res.ApplyStyle(anchor, style, fontSize, wordWrap);
            res.normal.textColor = colors[0];
            res.hover.textColor = colors[1];
            res.active.textColor = colors[2];
            res.onNormal.textColor = colors[3];
            res.onHover.textColor = colors[4];
            res.onActive.textColor = colors[5];
        }

        public static Texture2D BorderedTexture(int width, int height, int borderWidthX, int borderWidthY, Color border, Color center)
        {
            Texture2D texture2D = new Texture2D(width, height, TextureFormat.RGBA32, false);
            for (int i = 0; i < width; i++)
            {
                if (i < borderWidthX || i > width - 1 - borderWidthX)
                {
                    for (int j = 0; j < height; j++)
                    {
                        texture2D.SetPixel(i, j, border);
                    }
                }
                else
                {
                    for (int k = 0; k < height; k++)
                    {
                        if (k < borderWidthY || k > height - 1 - borderWidthY)
                        {
                            texture2D.SetPixel(i, k, border);
                        }
                        else
                        {
                            texture2D.SetPixel(i, k, center);
                        }
                    }
                }
            }
            texture2D.Apply();
            return texture2D;
        }

        public static GUIStyle CreateStyle(TextAnchor anchor, FontStyle fontStyle, int fontSize, bool wordWrap)
        {
            GUIStyle style = new GUIStyle();
            style.ApplyStyle(anchor, fontStyle, fontSize, wordWrap);
            return style;
        }

        public static GUIStyle CreateStyle(TextAnchor anchor, FontStyle style, int fontSize, bool wordWrap, Color color)
        {
            return CreateStyle(anchor, style, fontSize, wordWrap, new Color[6].Select(x => color).ToArray());
        }

        public static GUIStyle CreateStyle(TextAnchor anchor, FontStyle style, int fontSize, bool wordWrap, Color[] colors)
        {
            GUIStyle res = CreateStyle(anchor, style, fontSize, wordWrap);
            res.normal.textColor = colors[0];
            res.hover.textColor = colors[1];
            res.active.textColor = colors[2];
            res.onNormal.textColor = colors[3];
            res.onHover.textColor = colors[4];
            res.onActive.textColor = colors[5];
            return res;
        }

        public static Texture2D GetTexture(string path)
        {
            if (!File.Exists(path) || (!path.EndsWith(".png") && !path.EndsWith(".jpeg") && !path.EndsWith(".jpg")))
            {
                return Texture2D.whiteTexture;
            }
            Texture2D result = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            byte[] data = File.ReadAllBytes(path);
            result.LoadImage(data);
            result.Apply();
            return result;
        }

        public static Rect GetScreenMiddle(float width, float height)
        {
            return new Rect(Style.ScreenWidth / 2f - (width / 2f), Style.ScreenHeight / 2f - (height / 2f), width, height);
        }

        public static SmartRect[] GetSmartRects(Rect baseWindow, int count)
        {
            SmartRect[] result = new SmartRect[count];
            float x = baseWindow.x + Style.WindowSideOffset;
            float allWidth = baseWindow.width - Style.WindowSideOffset * 2;
            float width = (allWidth - (Style.WindowSideOffset * (count - 1))) / count;
            float y = baseWindow.y + Style.WindowTopOffset;
            for (int i = 0; i < count; i++)
            {
                result[i] = new SmartRect(x, y, width, Style.Height, Style.HorizontalMargin, Style.VerticalMargin);
                x += width + Style.WindowSideOffset;
            }
            return result;
        }

        public static Texture2D RectAngle(int width, int height, Color color)
        {
            Texture2D result = new Texture2D(width, height, TextureFormat.ARGB32, false);
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    result.SetPixel(i, j, color);
                }
            }
            result.Apply();
            return result;
        }

        public static Color[] TextureColors(Color main, int states)
        {
            Color[] result = new Color[states];
            for (int i = 0; i < states; i++)
            {
                if (Style.UseVectors)
                {
                    Vector3 delta = Style.TextureDeltas[i];
                    result[i] = new Color(main.r + delta.x, main.g + delta.y, main.b + delta.z, 1f);
                }
                else
                {
                    result[i] = Style.TextureColors[i];
                }
            }
            return result;
        }
    }
}