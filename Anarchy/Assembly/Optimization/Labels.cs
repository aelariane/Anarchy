using UnityEngine;
using static Optimization.Caching.Colors;

namespace Optimization
{
    internal class Labels
    {
        internal static Font Font;
        private static TextMesh bottomRight;
        private static TextMesh center;
        private static TextMesh topCenter;
        private static TextMesh topLeft;
        private static TextMesh topRight;
        private static TextMesh networkStatus;
        private static TextMesh version;

        #region Labels
        public static string BottomRight
        {
            get
            {
                if(bottomRight != null)
                {
                    return bottomRight.text;
                }
                bottomRight = CreateLabel("LabelInfoBottomRight", 32, TextAnchor.LowerRight, white, Font, TextAlignment.Right);
                if (bottomRight == null)
                    return "";
                return bottomRight.text;
            }
            set
            {
                if(bottomRight == null)
                {
                    bottomRight = CreateLabel("LabelInfoBottomRight", 32, TextAnchor.LowerRight, white, Font, TextAlignment.Right);
                    if (bottomRight == null)
                    {
                        return;
                    }
                }
                bottomRight.text = value;
            }
        }

        public static string Center
        {
            get
            {
                if (center != null)
                {
                    return center.text;
                }
                center = CreateLabel("LabelInfoCenter", 32, TextAnchor.LowerCenter, white, Font, TextAlignment.Center);
                if (center == null)
                    return "";
                return center.text;
            }
            set
            {
                if(center == null)
                {
                    center = CreateLabel("LabelInfoCenter", 32, TextAnchor.LowerCenter, white, Font, TextAlignment.Center);
                    if (center == null)
                        return;
                }
                center.text = value;
            }
        }

        public static string NetworkStatus
        {
            get
            {
                if (networkStatus != null)
                {
                    return networkStatus.text;
                }
                networkStatus = CreateLabel("LabelNetworkStatus", 32, TextAnchor.UpperLeft, white, Font, TextAlignment.Left);
                if (networkStatus == null)
                    return "";
                return networkStatus.text;
            }
            set
            {
                if(networkStatus == null)
                {
                    networkStatus = CreateLabel("LabelNetworkStatus", 32, TextAnchor.UpperLeft, white, Font, TextAlignment.Left);
                    if (networkStatus == null)
                        return;
                }
                networkStatus.text = value;
            }
        }

        public static string TopCenter
        {
            get
            {
                if (topCenter != null)
                {
                    return topCenter.text;
                }
                topCenter = CreateLabel("LabelInfoTopCenter", 32, TextAnchor.UpperCenter, white, Font, TextAlignment.Center);
                if (topCenter == null)
                    return "";
                return topCenter.text;
            }
            set
            {
                if(topCenter == null)
                {
                    topCenter = CreateLabel("LabelInfoTopCenter", 32, TextAnchor.UpperCenter, white, Font, TextAlignment.Center);
                    if (topCenter == null)
                        return;
                }
                topCenter.text = value;
            }
        }

        public static string TopLeft
        {
            get
            {
                if (topLeft != null)
                {
                    return topLeft.text;
                }
                topLeft = CreateLabel("LabelInfoTopLeft", 30, TextAnchor.UpperLeft, white, Font, TextAlignment.Left, FontStyle.Bold);
                if (topLeft == null)
                    return "";
                return topLeft.text;
            }
            set
            {
                if(topLeft == null)
                {
                    topLeft = CreateLabel("LabelInfoTopLeft", 30, TextAnchor.UpperLeft, white, Font, TextAlignment.Left, FontStyle.Bold);
                    if (topLeft == null)
                        return;
                }
                topLeft.text = value;
            }
        }

        public static string TopRight
        {
            get
            {
                if (topRight != null)
                {
                    return topRight.text;
                }
                topRight = CreateLabel("LabelInfoTopRight", 28, TextAnchor.UpperRight, white, Font, TextAlignment.Right);
                if (topRight == null)
                    return "";
                return topRight.text;
            }
            set
            {
                if(topRight == null)
                {
                    topRight = CreateLabel("LabelInfoTopRight", 28, TextAnchor.UpperRight, white, Font, TextAlignment.Right);
                    if (topRight == null)
                        return;
                }
                topRight.text = value;
            }
        }

        public static string VERSION
        {
            get
            {
                if (version != null)
                {
                    return version.text;
                }
                version = CreateLabel("VERSION", 30, TextAnchor.MiddleCenter, white, Font, TextAlignment.Center);
                if (version == null)
                    return "";
                return version.text;
            }
            set
            {
                if(version == null)
                {
                    version = CreateLabel("VERSION", 30, TextAnchor.MiddleCenter, white, Font, TextAlignment.Center);
                    if (version == null)
                        return;
                }
                version.text = value;
            }
        }
        #endregion

        internal static TextMesh CreateLabel(string name, int size, TextAnchor anchor, Color color, Font font, TextAlignment align, FontStyle style = FontStyle.Normal)
        {
            if (font == null)
                return null;
            GameObject res = GameObject.Find(name);
            if (res == null || res.GetComponent<UILabel>() == null)
                return null;

            TextMesh text = res.GetComponent<TextMesh>();
            if (text == null)
                text = res.AddComponent<TextMesh>();
            MeshRenderer render = res.GetComponent<MeshRenderer>();
            if (render == null)
                render = res.AddComponent<MeshRenderer>();

            UILabel label = res.GetComponent<UILabel>();
            render.material = font.material;
            text.font = font;
            text.fontSize = size;
            text.anchor = anchor;
            text.alignment = align;
            text.color = color;
            text.fontStyle = style;
            res.transform.SetParent(res.transform.parent);
            res.transform.localPosition = res.transform.localPosition;
            res.transform.localRotation = res.transform.localRotation;
            res.transform.localScale = new Vector3(4.9f, 4.9f);

            if (label != null)
            {
                text.text = label.text;
                label.enabled = false;
            }
            res.layer = 5;
            text.richText = true;
            return text;
        }

        #region Old Version
        //internal static TextMesh CreateLabel(string name, int size, TextAnchor anchor, Color color, Font font, TextAlignment align)
        //{
        //    if (font == null)
        //        return null;
        //    GameObject res = GameObject.Find(name);
        //    if (res == null)
        //        return null;
        //    if (res.GetComponent<UILabel>() == null)
        //        return null;
        //    TextMesh text = res.GetComponent<TextMesh>();
        //    if (text == null)
        //        text = res.AddComponent<TextMesh>();
        //    MeshRenderer render = res.GetComponent<MeshRenderer>();
        //    if (render == null)
        //        render = res.AddComponent<MeshRenderer>();
        //    res.transform.localScale = new Vector3(4.9f, 4.9f);
        //    UILabel label = res.GetComponent<UILabel>();
        //    render.material = font.material;
        //    text.font = font;
        //    text.fontSize = size;
        //    text.anchor = anchor;
        //    text.alignment = align;
        //    text.color = color;
        //    if (label != null)
        //    {
        //        text.text = label.text;
        //        label.enabled = false;
        //    }
        //    text.richText = true;
        //    return text;
        //}
        #endregion
    }
}
