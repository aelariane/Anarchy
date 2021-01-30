﻿using UnityEngine;
using static Optimization.Caching.Colors;

namespace Anarchy.UI
{
    internal sealed class PanelMain : GUIBase
    {
        private Rect profileRect;
        private SmartRect rect;
        private GUIStyle style;
        private int enabledPanel = -1;
        private GUIBase[] allUsedPanels;

        public PanelMain() : base(nameof(PanelMain), GUILayers.PanelMain)
        {
        }

        private bool Button(string key)
        {
            if (rect == null)
            {
                return false;
            }
            bool result = UnityEngine.GUI.Button(rect.ToRect(), locale.Get(key), style);
            rect.MoveY();
            return result;
        }

        private void CheckEnabled(GUIBase obj, GUIBase[] toCheck)
        {
            if (obj.Active)
            {
                obj.Disable();
                return;
            }
            for (int i = 0; i < toCheck.Length; i++)
            {
                if (toCheck[i].Active)
                {
                    toCheck[i].EnableNext(obj);
                    return;
                }
            }
            obj.Enable();
        }

        private int CheckActivePanel()
        {
            for (int i = 0; i < allUsedPanels.Length; i++)
            {
                if (allUsedPanels[i].Active)
                {
                    return i;
                }
            }
            return -1;
        }

        protected internal override void Draw()
        {
            var urlRect = new Rect(0f, 0f, new AutoScaleFloat(400), new AutoScaleFloat(100f));
            if(GUI.Button(urlRect, locale["aottg2Message1"] + "\n" + locale["aottg2Message2"]))
            {
                Application.OpenURL("https://www.patreon.com/aottg2");
            }
            enabledPanel = CheckActivePanel();
            if (GUI.Button(profileRect, locale["profile"] + " <b>" + User.ProfileName + "</b>"))
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
                if (enabledPanel < 0)
                {
                    Application.LoadLevel("characterCreation");
                    return;
                }
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
            allUsedPanels = null;
            AnarchyManager.ProfilePanel.DisableImmediate();
            AnarchyManager.SinglePanel.DisableImmediate();
            AnarchyManager.SettingsPanel.DisableImmediate();
            AnarchyManager.ServerList.DisableImmediate();
        }

        protected override void OnEnable()
        {
            profileRect = new Rect(Style.ScreenWidth - new AutoScaleFloat(300f), 0f, new AutoScaleFloat(300f), new AutoScaleFloat(20f));
            rect = new SmartRect(new Rect(Style.ScreenWidth - new AutoScaleFloat(400f), Style.ScreenHeight - new AutoScaleFloat(360f), new AutoScaleFloat(396f), new AutoScaleFloat(54f)), 0f, new AutoScaleFloat(18f));
            style = Helper.CreateStyle(TextAnchor.MiddleRight, FontStyle.Normal, Mathf.RoundToInt(new AutoScaleFloat(35)), true, new Color[] { white, orange, yellow, white, white, white });
            style.normal.background = style.hover.background = style.active.background = EmptyTexture;
            style.font = AnarchyAssets.Load<Font>(Style.FontName);
            allUsedPanels = new GUIBase[] { AnarchyManager.ProfilePanel, AnarchyManager.SinglePanel, AnarchyManager.ServerList, AnarchyManager.SettingsPanel };
        }

        public override void OnUpdateScaling()
        {
            OnEnable();
        }
    }
}
