using Anarchy.Configuration;
using Anarchy.Configuration.Presets;
using System.Collections.Generic;
using UnityEngine;
using static Anarchy.UI.GUI;

namespace Anarchy.UI
{
    public class SkinsPanel : GUIPanel
    {
        private const int Humans = 0;
        private const int Titans = 1;
        private const int City = 2;
        private const int Forest = 3;
        private const int Skyboxes = 4;
        private const int Custom = 5;

        private StringSetting currentSet = null;
        private SmartRect left;
        private string newSetName = string.Empty;
        private Rect pageRect;
        private string[] presetLabels;
        private List<SkinPreset> presets;
        private SmartRect rect;
        private SmartRect right;
        private Vector2 scroll;
        private Rect scrollArea;
        private Rect scrollAreaView;
        private SmartRect scrollRect;
        private int skinSelection = 0;
        private string[] skinTypeSelection;

        public SkinsPanel() : base(nameof(SkinsPanel), GUILayers.SkinsPanel)
        {
        }

        [GUIPage(City)]
        private void CityPage()
        {
            left.Reset();
            SelectionGrid(left, SkinSettings.CitySkins, locale.GetArray("skinStateSelection"), 3, true);
            left.MoveY();
            DrawControlElements();
        }

        [GUIPage(City, GUIPageType.EnableMethod)]
        private void CityPageEnable()
        {
            InitPresets(SkinSettings.CitySet, CityPreset.GetAllPresets());
        }

        [GUIPage(Custom)]
        private void CustomPage()
        {
            left.Reset();
            SelectionGrid(left, SkinSettings.CustomSkins, locale.GetArray("skinStateSelection"), 3, true);
            left.MoveY();
            DrawControlElements();
        }

        [GUIPage(Custom, GUIPageType.EnableMethod)]
        private void CustomPageEnable()
        {
            InitPresets(SkinSettings.CustomMapSet, CustomMapPreset.GetAllPresets());
        }

        private void DrawControlElements()
        {
            LabelCenter(left, locale["sets"], true);
            Label(left, locale["name"], false);
            left.MoveOffsetX(Style.LabelOffset);
            newSetName = TextField(left, newSetName, string.Empty, 0f, true);
            left.ResetX();
            left.BeginHorizontal(2);
            if (Button(left, locale["btnAdd"], false))
            {
                SkinPreset add = null;
                if (pageSelection == Humans)
                {
                    add = new HumanSkinPreset(newSetName);
                }
                else if (pageSelection == Titans)
                {
                    add = new TitanSkinPreset(newSetName);
                }
                else if (pageSelection == City)
                {
                    add = new CityPreset(newSetName);
                }
                else if (pageSelection == Forest)
                {
                    add = new ForestPreset(newSetName);
                }
                else if (pageSelection == Skyboxes)
                {
                    add = new SkyboxPreset(newSetName);
                }
                else if (pageSelection == Custom)
                {
                    add = new CustomMapPreset(newSetName);
                }
                if (add != null)
                {
                    presets.Add(add);
                    skinSelection = presets.Count - 1;
                    newSetName = locale["set"] + " " + (presets.Count + 1).ToString();
                    presetLabels = new string[presets.Count];
                    for (int i = 0; i < presetLabels.Length; i++)
                    {
                        presetLabels[i] = presets[i].Name;
                    }
                }
            }
            left.MoveX();
            if (Button(left, locale["btnDelete"], true))
            {
                if (skinSelection >= 0)
                {
                    presets[skinSelection].Delete();
                    presets.RemoveAt(skinSelection);
                }
                skinSelection = presets.Count > 0 ? presets.Count - 1 : -1;
                presetLabels = new string[presets.Count];
                for (int i = 0; i < presetLabels.Length; i++)
                {
                    presetLabels[i] = presets[i].Name;
                }
            }
            left.ResetX();

            scrollArea.y = left.y;
            left.MoveToEndY(WindowPosition, Style.Height + Style.VerticalMargin);
            scrollArea.height = left.y - scrollArea.y;
            scrollRect.Reset();
            scrollAreaView.height = (Style.Height * presetLabels.Length) + (Style.VerticalMargin * (presetLabels.Length + 1));

            scroll = BeginScrollView(scrollArea, scroll, scrollAreaView);
            skinSelection = SelectionGrid(scrollRect, skinSelection, presetLabels, 1, true);
            EndScrollView();
        }

        protected override void DrawMainPart()
        {
            rect.Reset();
            Box(WindowPosition, locale["title"]);
            rect.MoveOffsetX(new AutoScaleFloat(120f));
            rect.width -= new AutoScaleFloat(120f);
            pageSelection = SelectionGrid(rect, pageSelection, skinTypeSelection, skinTypeSelection.Length);

            right.Reset();
            if (presets != null && presets.Count > 0 && skinSelection >= 0)
            {
                var set = presets[skinSelection];
                set.Draw(right, locale);
            }

            rect.ResetX();
            rect.MoveToEndY(WindowPosition, Style.Height);
            rect.MoveToEndX(WindowPosition, Style.LabelOffset);
            rect.width = Style.LabelOffset;
            if (Button(rect, locale["btnClose"]))
            {
                Disable();
            }
        }

        [GUIPage(Forest)]
        private void ForestPage()
        {
            left.Reset();
            SelectionGrid(left, SkinSettings.ForestSkins, locale.GetArray("skinStateSelection"), 3, true);
            left.MoveY();
            DrawControlElements();
        }

        [GUIPage(Forest, GUIPageType.EnableMethod)]
        private void ForestPageEnable()
        {
            InitPresets(SkinSettings.ForestSet, ForestPreset.GetAllPresets());
        }

        [GUIPage(Humans)]
        private void HumansPage()
        {
            left.Reset();
            ToggleButton(left, SkinSettings.DisableCustomGas, locale["disableCustomGas"], true);
            SelectionGrid(left, SkinSettings.HumanSkins, locale.GetArray("skinStateSelection"), 3, true);
            left.MoveY();
            DrawControlElements();
        }

        [GUIPage(Humans, GUIPageType.EnableMethod)]
        private void HumansPageEnable()
        {
            InitPresets(SkinSettings.HumanSet, HumanSkinPreset.GetAllPresets());
        }

        private void InitPresets(StringSetting set, SkinPreset[] sets)
        {
            currentSet = set;
            presets = new List<SkinPreset>();
            if (sets != null)
            {
                for (int i = 0; i < sets.Length; i++)
                {
                    presets.Add(sets[i]);
                }
            }
            skinSelection = -1;
            for (int i = 0; i < presets.Count; i++)
            {
                if (presets[i].Name.Equals(set.Value))
                {
                    skinSelection = i;
                    break;
                }
            }
            presetLabels = new string[presets.Count];
            for (int i = 0; i < presetLabels.Length; i++)
            {
                presetLabels[i] = presets[i].Name;
            }
            newSetName = locale["set"] + " " + (presets.Count + 1).ToString();
        }

        protected override void OnAnyPageEnabled()
        {
            if (left != null)
            {
                for (int i = 0; i < 5; i++)
                {
                    left.MoveY();
                }
                scrollArea = new Rect(left.x, left.y, left.width, WindowPosition.height - (4 * (Style.Height + Style.VerticalMargin)) - (Style.WindowTopOffset + Style.WindowBottomOffset) - 10f);
                scrollRect = new SmartRect(0f, 0f, left.width, left.height);
                scrollAreaView = new Rect(0f, 0f, rect.width, 1000f);
                scroll = Optimization.Caching.Vectors.v2zero;
            }
        }

        protected override void OnBeforePageChanged()
        {
            if (currentSet != null)
            {
                currentSet.Value = skinSelection < 0 ? Anarchy.Configuration.StringSetting.NotDefine : presets[skinSelection].Name;
            }
            SavePresets();
        }

        protected override void OnPanelDisable()
        {
            skinTypeSelection = null;
            left = null;
            right = null;
            rect = null;
            currentSet = null;
        }

        protected override void OnPanelEnable()
        {
            rect = Helper.GetSmartRects(WindowPosition, 1)[0];
            skinTypeSelection = locale.GetArray("skinSelection");
            pageRect = new Rect(WindowPosition.x, WindowPosition.y + ((Style.Height + Style.VerticalMargin) * 2f), WindowPosition.width, WindowPosition.height - ((Style.VerticalMargin + Style.Height) * 2f));
            SmartRect[] rects = Helper.GetSmartRects(pageRect, 2);
            left = rects[0];
            right = rects[1];
        }

        private void SavePresets()
        {
            if (presets != null && presets.Count > 0)
            {
                foreach (var set in presets)
                {
                    set.Save();
                }
                presets = null;
            }
        }

        [GUIPage(Skyboxes)]
        private void SkyboxesPage()
        {
            left.Reset();
            Label(left, locale["skyboxDesc"], true);
            ToggleButton(left, SkinSettings.SkyboxSkinsEnabled, locale["skyboxState"], true);
            DrawControlElements();
        }

        [GUIPage(Skyboxes, GUIPageType.EnableMethod)]
        private void SkyboxPageEnable()
        {
            InitPresets(SkinSettings.SkyboxSet, SkyboxPreset.GetAllPresets());
        }

        [GUIPage(Titans)]
        private void TitansPage()
        {
            left.Reset();
            SelectionGrid(left, SkinSettings.TitanSkins, locale.GetArray("skinStateSelection"), 3, true);
            left.MoveY();
            DrawControlElements();
        }

        [GUIPage(Titans, GUIPageType.EnableMethod)]
        private void TitansPageEnable()
        {
            InitPresets(SkinSettings.TitanSet, TitanSkinPreset.GetAllPresets());
        }

        public override void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Disable();
                return;
            }
        }
    }
}