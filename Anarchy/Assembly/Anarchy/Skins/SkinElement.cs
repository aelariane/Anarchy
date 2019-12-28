using Anarchy.Configuration;
using Anarchy.UI;
using UnityEngine;

namespace Anarchy.Skins
{
    public class SkinElement
    {
        private const int AllowedSize = 1000000;

        public bool IsDone { get; private set; } = false;
        public bool IsTransparent => Path.ToLower().Equals("transparent");
        public bool NeedReload { get; private set; }
        public string Path { get; private set; }
        public Texture2D Texture { get; private set; }

        public SkinElement(string path) : this(path, true)
        {
        }

        public SkinElement(string path, bool reload)
        {
            NeedReload = reload;
            Path = path;
        }

        public void CheckReload(string path)
        {
            if (NeedReload)
            {
                return;
            }
            NeedReload = !path.Equals(Path);
        }

        public System.Collections.IEnumerator TryLoad()
        {
            NeedReload = false;
            IsDone = false;
            if (IsTransparent)
            {
                Texture2D tex = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                tex.SetPixel(0, 0, Optimization.Caching.Colors.empty);
                tex.Apply();
                Texture = tex;
                IsDone = true;
                yield break;
            }
            if (!Path.EndsWith(".jpg") && !Path.EndsWith(".jpeg") && !Path.EndsWith(".png"))
            {
                Texture = Helper.RectAngle(4, 4, Optimization.Caching.Colors.white);
                yield break;
            }
            WWW www = new WWW(Path);
            yield return www;
            if(www.texture == null || www.size > AllowedSize)
            {
                Texture = Helper.RectAngle(4, 4, Optimization.Caching.Colors.white);
                yield break;
            }
            Texture = new Texture2D(4, 4, TextureFormat.DXT1, VideoSettings.Mipmap.Value);
            www.LoadImageIntoTexture(Texture);
            www.Dispose();
            Texture.Apply();
            IsDone = true;
            yield break;
        }
    }
}