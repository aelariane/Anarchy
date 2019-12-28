using UnityEngine;
using static Optimization.Caching.Colors;

namespace Anarchy.UI
{
    internal sealed class PanelMain : GUIBase
    {
        private Rect profileRect = new Rect(Style.ScreenWidth - 300f, 0f, 300f, 20f);
        private SmartRect rect;
        private GUIStyle style;

        public PanelMain() : base(nameof(PanelMain), 1)
        {
        }

        private bool Button(string key)
        {
            bool result = UnityEngine.GUI.Button(rect.ToRect(), locale.Get(key), style);
            rect.MoveY();
            return result;
        }

        private void CheckEnabled(GUIBase obj, GUIBase[] toCheck)
        {
            if(obj.Active)
            {
                obj.Disable();
                return;
            }
            for(int i = 0; i < toCheck.Length; i++)
            {
                if (toCheck[i].Active)
                {
                    toCheck[i].EnableNext(obj);
                    return;
                }
            }
            obj.Enable();
        }

        protected internal override void Draw()
        {
            if(GUI.Button(profileRect, locale["profile"] + " <b>" + User.ProfileName + "</b>"))
            {
                CheckEnabled(AnarchyManager.ProfilePanel, new GUIBase[] { AnarchyManager.SinglePanel, AnarchyManager.ServerList, AnarchyManager.SettingsPanel });
            }
            rect.Reset();
            if (Button("single"))
            {
                CheckEnabled(AnarchyManager.SinglePanel, new GUIBase[] { AnarchyManager.ProfilePanel, AnarchyManager.ServerList, AnarchyManager.SettingsPanel });
            }
            if (Button("multi"))
            {
                CheckEnabled(AnarchyManager.ServerList, new GUIBase[] { AnarchyManager.ProfilePanel, AnarchyManager.SinglePanel, AnarchyManager.SettingsPanel });
            }
            if (Button("settings"))
            {
                CheckEnabled(AnarchyManager.SettingsPanel, new GUIBase[] { AnarchyManager.ProfilePanel, AnarchyManager.ServerList, AnarchyManager.SinglePanel });
            }
            if (Button("custom_characters"))
            {
                Application.LoadLevel("characterCreation");
                return;
            }
            if (Button("exit"))
            {
                Application.Quit();
            }
        }

        protected override void OnDisable()
        {
            rect = null;
            style = null;
            AnarchyManager.ProfilePanel.DisableImmediate();
            AnarchyManager.SinglePanel.DisableImmediate();
            AnarchyManager.SettingsPanel.DisableImmediate();
            AnarchyManager.ServerList.DisableImmediate();
        }

        protected override void OnEnable()
        {
            profileRect = new Rect(Style.ScreenWidth - 300f, 0f, 300f, 20f);
            rect = new SmartRect(new Rect(Style.ScreenWidth - 440f, Style.ScreenHeight - 400f, 435f, 60f), 0f, 20f);
            style = Helper.CreateStyle(TextAnchor.MiddleRight, FontStyle.Normal, 43, true, new Color[] { white, orange, yellow, white, white, white });
            style.normal.background = style.hover.background = style.active.background = EmptyTexture;
            style.font = AnarchyAssets.Load<Font>(Style.FontName);

        }

        public override void OnUpdateScaling()
        {
            OnEnable();
        }
    }
}
