using UnityEngine;

namespace Anarchy.UI
{
    internal class BackgroundOnStart : MonoBehaviour
    {
        private Texture2D background;
        private readonly Color backgroundColor = new Color(0f, 36f / 255f, 36f / 255f, 1f);
        private Rect position;

        private void Awake()
        {
            background = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            background.SetPixel(0, 0, backgroundColor);
            background.Apply();
            position = new Rect(0f, 0f, Style.ScreenWidth, Style.ScreenHeight);
        }

        private void OnGUI()
        {
            UnityEngine.GUI.DrawTexture(position, background);
        }
    }
}
