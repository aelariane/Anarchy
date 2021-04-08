using Anarchy;
using Anarchy.Configuration;
using UnityEngine;
using static Optimization.Caching.Colors;

namespace Optimization
{
    internal class Labels
    {
        internal static Font Font;
        private static readonly HUDLabel bottomRight = new HUDLabel("LabelInfoBottomRight", 32, TextAnchor.LowerRight, white, TextAlignment.Right);
        private static readonly HUDLabel center = new HUDLabel("LabelInfoCenter", 32, TextAnchor.LowerCenter, white, TextAlignment.Center);
        private static readonly HUDLabel topCenter = new HUDLabel("LabelInfoTopCenter", 32, TextAnchor.UpperCenter, white, TextAlignment.Center);
        private static readonly HUDLabel topLeft = new HUDLabel("LabelInfoTopLeft", 30, TextAnchor.UpperLeft, white, TextAlignment.Left, FontStyle.Bold);
        private static readonly HUDLabel topRight = new HUDLabel("LabelInfoTopRight", 28, TextAnchor.UpperRight, white, TextAlignment.Right);
        private static readonly HUDLabel networkStatus = new HUDLabel("LabelNetworkStatus", 32, TextAnchor.UpperLeft, white, TextAlignment.Left);
        private static readonly HUDLabel version = new HUDLabel("VERSION", 30, TextAnchor.MiddleCenter, white, TextAlignment.Center);

        public static BoolSetting UseCustomLabels { get; } = new BoolSetting(nameof(UseCustomLabels), false);

        public static void UpdateShadowPosition(string name, Vector3 newLocalPos)
        {
            if (VideoSettings.ShadowsUI.Value)
            {
                var shadow = GameObject.Find(name + "(Shadow)");
                if(shadow != null)
                {
                    var mesh = shadow.GetComponent<TextMesh>();
                    var anchor = mesh.anchor;
                    float deltaX = 1.25f;
                    if (anchor == TextAnchor.MiddleRight || anchor == TextAnchor.UpperRight || anchor == TextAnchor.LowerRight)
                    {
                        deltaX = -deltaX;
                    }
                    float deltaY = 1.25f;
                    if ((int)anchor >= 6)
                    {
                        deltaY = -deltaY;
                    }

                    deltaY *= Anarchy.UI.UIManager.LabelScale.Value;
                    deltaX *= Anarchy.UI.UIManager.LabelScale.Value;
                    mesh.transform.localPosition = new Vector3(newLocalPos.x + deltaX, newLocalPos.y - deltaY, newLocalPos.z + 0.00001f);
                }
            }
        }

        #region Labels
        public static string BottomRight

        {
            get
            {
                return bottomRight.text;
            }
            set
            {
                bottomRight.text = value;
            }
        }

        public static string Center
        {
            get
            {
                return center.text;
            }
            set
            {
                center.text = value;
            }
        }

        public static string NetworkStatus
        {
            get
            {
                return networkStatus.text;
            }
            set
            {
                networkStatus.text = value;
            }
        }

        public static string TopCenter
        {
            get
            {
                return topCenter.text;
            }
            set
            {
                topCenter.text = value;
            }
        }

        public static string TopLeft
        {
            get
            {
                return topLeft.text;
            }
            set
            {
                topLeft.text = value;
            }
        }

        public static string TopRight
        {
            get
            {
                return topRight.text;
            }
            set
            {
                topRight.text = value;
            }
        }

        public static string VERSION
        {
            get
            {
                return version.text;
            }
            set
            {
                version.text = value;
            }
        }

        #endregion Labels

        internal static TextMesh CreateLabel(string name, int size, TextAnchor anchor, Color color, Font font, TextAlignment align, FontStyle style = FontStyle.Normal)
        {
            if (font == null)
            {
                return null;
            }

            GameObject res = GameObject.Find(name);
            if (res == null || res.GetComponent<UILabel>() == null)
            {
                return null;
            }

            TextMesh text = res.GetComponent<TextMesh>();
            if (text == null)
            {
                text = res.AddComponent<TextMesh>();
            }

            MeshRenderer render = res.GetComponent<MeshRenderer>();
            if (render == null)
            {
                render = res.AddComponent<MeshRenderer>();
            }

            UILabel label = res.GetComponent<UILabel>();
            render.material = font.material;
            text.font = font;
            text.fontSize = Mathf.RoundToInt(size * Anarchy.UI.UIManager.LabelScale.Value);
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

        private static TextMesh CreateShadow(string name, int size, TextAnchor anchor, Color color, Font font, TextAlignment align, FontStyle style = FontStyle.Normal)
        {
            if (font == null)
            {
                return null;
            }

            GameObject res1 = GameObject.Find(name);
            if (res1 == null || res1.GetComponent<UILabel>() == null)
            {
                return null;
            }

            GameObject res = (GameObject)GameObject.Instantiate(res1, res1.transform.position, res1.transform.rotation);

            TextMesh text = res.GetComponent<TextMesh>();
            if (text == null)
            {
                text = res.AddComponent<TextMesh>();
            }

            MeshRenderer render = res.GetComponent<MeshRenderer>();
            if (render == null)
            {
                render = res.AddComponent<MeshRenderer>();
            }

            UILabel label = res.GetComponent<UILabel>();
            render.material = font.material;
            text.font = font;
            text.fontSize = Mathf.RoundToInt(size * Anarchy.UI.UIManager.LabelScale.Value);
            text.anchor = anchor;
            text.alignment = align;
            text.color = color;
            text.fontStyle = style;
            res.transform.SetParent(res1.transform.parent);
            res.transform.localPosition = res1.transform.localPosition;
            res.transform.localRotation = res1.transform.localRotation;
            res.transform.localScale = new Vector3(4.9f, 4.9f);
            Transform tf = text.transform;
            float deltaX = 1.25f;
            if (anchor == TextAnchor.MiddleRight || anchor == TextAnchor.UpperRight || anchor == TextAnchor.LowerRight)
            {
                deltaX = -deltaX;
            }
            float deltaY = 1.25f;
            if ((int)anchor >= 6)
            {
                deltaY = -deltaY;
            }

            deltaY *= Anarchy.UI.UIManager.LabelScale.Value;
            deltaX *= Anarchy.UI.UIManager.LabelScale.Value;
            tf.localPosition = new Vector3(tf.localPosition.x + deltaX, tf.localPosition.y - deltaY, tf.localPosition.z + 0.00001f);

            if (label != null)
            {
                text.text = label.text;
                label.enabled = false;
            }
            res.layer = 5;
            text.richText = true;
            text.gameObject.name = name + "(Shadow)";
            return text;
        }

        private class HUDLabel
        {
            private TextAlignment align;
            private TextAnchor anchor;
            public string Name;
            private Color color;
            private TextMesh regularTextMesh;
            private TextMesh shadowTextMesh;
            private int size;
            private FontStyle style;

            public string text
            {
                get
                {
                    if (regularTextMesh != null)
                    {
                        return regularTextMesh.text;
                    }
                    CreateMeshes();
                    if (regularTextMesh == null)
                    {
                        return string.Empty;
                    }
                    return regularTextMesh.text;
                }
                set
                {
                    if (regularTextMesh == null)
                    {
                        CreateMeshes();
                        if (regularTextMesh == null)
                        {
                            return;
                        }
                    }
                    regularTextMesh.text = value.ToHTMLFormat();
                    if (VideoSettings.ShadowsUI.Value)
                    {
                        shadowTextMesh.text = $"<color=#111111>{value.RemoveHex().RemoveHTML()}</color>";
                    }
                }
            }

            public HUDLabel(string name, int size, TextAnchor anchor, Color color, TextAlignment align, FontStyle style = FontStyle.Normal)
            {
                Name = name;
                this.size = size;
                this.anchor = anchor;
                this.color = color;
                this.align = align;
                this.style = style;
            }

            private void CreateMeshes()
            {
                if (regularTextMesh == null)
                {
                    regularTextMesh = CreateLabel(Name, size, anchor, color, Anarchy.UI.Style.Font, align, style);
                }
                shadowTextMesh = CreateShadow(Name, size, anchor, color, Anarchy.UI.Style.Font, align, style);
            }
        }
    }
}