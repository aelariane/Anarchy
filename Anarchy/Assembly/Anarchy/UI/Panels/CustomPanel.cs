using Optimization;
using RC;
using UnityEngine;
using static Anarchy.UI.GUI;

namespace Anarchy.UI
{
    public class CustomPanel : GUIPanel
    {
        private const int CustomLogicPage = 1;
        private const int CustomMapsPage = 0;

        public static readonly string LogicsPath = Application.dataPath + "/../Logics/";
        public static readonly string MapsPath = Application.dataPath + "/../Maps/";

        private string[] allNames;
        private GUIStyle areaStyle;
        private string filter;
        private float filterUpdate = 0f;
        private string[] gameTypes;
        private SmartRect left;
        private string[] selections;
        private SmartRect rect;
        private SmartRect right;
        private Vector2 scroll;
        private Rect scrollArea;
        private Rect scrollAreaView;
        private SmartRect scrollRect;

        public CustomPanel() : base(nameof(CustomPanel), -1)
        {
            animator = new Animation.CenterAnimation(this, Helper.GetScreenMiddle(Style.WindowWidth, Style.WindowHeight));
        }

        protected override void DrawMainPart()
        {
            left.Reset();
            right.Reset();
            rect.Reset();
            float offset = new AutoScaleFloat(120f);
            rect.MoveOffsetX(offset);
            rect.width -= offset;
            pageSelection = SelectionGrid(rect, pageSelection, selections, selections.Length, true);
            rect.ResetX();

            LabelCenter(right, locale["picker" + pageSelection.ToString()], true);
            right.BeginHorizontal(2);
            if(Button(right, locale["update"], false))
            {
                if(pageSelection == CustomLogicPage)
                {
                    allNames = LoadFiles(LogicsPath);
                }
                else
                {
                    allNames = LoadFiles(MapsPath);
                }
            }
            right.MoveX();
            if(Button(right, locale["random"], true))
            {
                int pickId = Random.Range(0, allNames.Length);
                if (filter.Length > 0)
                {
                    PickByName(allNames[pickId], true);
                }
                else
                {
                    Pick(pickId, allNames[pickId], true);
                }
            }
            right.ResetX();
            filter = TextField(right, filter, locale["filter"], Style.LabelOffset, true);

            scrollArea.y = right.y;
            right.MoveToEndY(BoxPosition, Style.Height + Style.VerticalMargin);
            scrollArea.height = right.y - scrollArea.y;
            scrollRect.Reset();
            scrollAreaView.height = (Style.Height * allNames.Length) + ((Style.VerticalMargin * 2) * allNames.Length);

            scroll = BeginScrollView(scrollArea, scroll, scrollAreaView);
            for (int i = 0; i < allNames.Length; i++)
            {
                if (Button(scrollRect, allNames[i], (i != allNames.Length - 1)))
                {
                    if (filter.Length == 0)
                    {
                        Pick(i, allNames[i], false);
                    }
                    else
                    {
                        PickByName(allNames[i], false);
                    }
                }
            }
            EndScrollView();
        }

        private string Load(int id, string path)
        {
            string[] files = System.IO.Directory.GetFiles(path);
            if (id == -1)
            {
                id = Random.Range(0, files.Length);
            }
            return System.IO.File.ReadAllText(files[id]);
        }

        private string LoadByName(string name, string path)
        {
            if(System.IO.File.Exists(path + name + ".txt"))
            {
                return System.IO.File.ReadAllText(path + name + ".txt");
            }
            return string.Empty;
        }

        private string[] LoadFiles(string path)
        {
            string[] files = System.IO.Directory.GetFiles(path);
            if(files.Length == 0)
            {
                return new string[0];
            }
            for(int i = 0; i < files.Length; i++)
            {
                var file = new System.IO.FileInfo(files[i]);
                string name = file.Name;
                if (file.Extension.Length > 0)
                {
                    name = name.Replace(file.Extension, "");
                }
                files[i] = name;
            }
            return files;
        }

        [GUIPage(CustomLogicPage)]
        private void LogicPage()
        {
            LabelCenter(left, locale["logicScript"], true);
            left.height = (BoxPosition.y + BoxPosition.height - Style.WindowBottomOffset - Style.Height - Style.VerticalMargin) - left.y;
            CustomLevel.currentScriptLogic = UnityEngine.GUI.TextArea(left.ToRect(), CustomLevel.currentScriptLogic, areaStyle);

            rect.MoveToEndY(BoxPosition, Style.Height);
            rect.width = 144f;
            rect.height = Style.Height;
            if (Button(rect, locale["btnClear"], false))
            {
                CustomLevel.currentScriptLogic = "";
            }

            rect.MoveToEndX(BoxPosition, 144f);
            if (Button(rect, locale["btnClose"]))
            {
                Disable();
                return;
            }
        }

        [GUIPage(CustomLogicPage, GUIPageType.EnableMethod)]
        private void LogicPageEnable()
        {
            Rect pos = BoxPosition;
            pos.y += (Style.Height + Style.VerticalMargin);
            SmartRect[] rects = Helper.GetSmartRects(pos, 2);
            left = rects[0];
            right = rects[1];
            allNames = LoadFiles(LogicsPath);
        }

        [GUIPage(CustomMapsPage)]
        private void MapPage()
        {
            Label(rect, locale["gameMode"], false);
            rect.MoveOffsetX(Style.LabelOffset);
            SelectionGrid(rect, RCManager.GameType, gameTypes, gameTypes.Length, true);
            rect.ResetX();
            rect.width = right.width;
            TextField(rect, RCManager.SpawnCapCustom, locale["spawnCap"], Style.BigLabelOffset, true);

            LabelCenter(left, locale["mapScript"], true);
            left.height = (BoxPosition.y + BoxPosition.height - Style.WindowBottomOffset - Style.Height - Style.VerticalMargin) - left.y;
            CustomLevel.currentScript = UnityEngine.GUI.TextArea(left.ToRect(), CustomLevel.currentScript, areaStyle);


            rect.MoveToEndY(BoxPosition, Style.Height);
            rect.width = 144f;
            rect.height = Style.Height;
            if (Button(rect, locale["btnClear"], false))
            {
                CustomLevel.currentScript = "";
            }

            rect.MoveToEndX(BoxPosition, 144f);
            if (Button(rect, locale["btnClose"]))
            {
                Disable();
                return;
            }
        }

        [GUIPage(CustomMapsPage, GUIPageType.EnableMethod)]
        private void MapPageEnable()
        {
            Rect pos = BoxPosition;
            float k = ((Style.Height + Style.VerticalMargin) * 3f);
            pos.y += k;
            SmartRect[] rects = Helper.GetSmartRects(pos, 2);
            left = rects[0];
            right = rects[1];
            scrollArea = new Rect(right.x, right.y, right.width, BoxPosition.height - (4 * (Style.Height + Style.VerticalMargin)) - (Style.WindowTopOffset + Style.WindowBottomOffset) - 10f);
            scrollRect = new SmartRect(0f, 0f, right.width, right.height);
            scrollAreaView = new Rect(0f, 0f, rect.width, 1000f);
            allNames = LoadFiles(MapsPath);
        }

        protected override void OnPanelDisable()
        {
            gameTypes = null;
            selections = null;
            rect = null;
            areaStyle = null;
            allNames = null;
            filter = null;
        }

        protected override void OnPanelEnable()
        {
            gameTypes = locale.GetArray("gameTypes");
            rect = Helper.GetSmartRects(BoxPosition, 1)[0];
            selections = locale.GetArray("selection");
            areaStyle = new GUIStyle(Style.TextField);
            areaStyle.alignment = TextAnchor.UpperLeft;
            scroll = Optimization.Caching.Vectors.v2zero;
            filter = "";
        }

        private void Pick(int id, string name, bool rnd)
        {
            if(pageSelection == CustomLogicPage)
            {
                CustomLevel.currentScriptLogic = Load(id, LogicsPath);
            }
            else
            {
                CustomLevel.currentScript = Load(id, MapsPath);
            }
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer)
            {
                FengGameManagerMKII.FGM.BasePV.RPC("Chat", PhotonTargets.All, new object[] { $"Next {(pageSelection == CustomLogicPage ? "logic" : "map")}{(rnd ? " (Random)" : "")}: <b>{name}</b>", "" });
            }
        }

        private void PickByName(string name, bool rnd)
        {
            if (pageSelection == CustomLogicPage)
            {
                CustomLevel.currentScriptLogic = LoadByName(name, LogicsPath);
                if (CustomLevel.currentScriptLogic == string.Empty)
                {
                    return;
                }
            }
            else
            {
                CustomLevel.currentScript = LoadByName(name, MapsPath);
                if(CustomLevel.currentScript == string.Empty)
                {
                    return;
                }
            }
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.MultiPlayer)
            {
                FengGameManagerMKII.FGM.BasePV.RPC("Chat", PhotonTargets.All, new object[] { $"Next {(pageSelection == CustomLogicPage ? "logic" : "map")}{(rnd ? " (Random)" : "")}: <b>{name}</b>", "" });
            }
        }

        public override void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Disable();
                return;
            }
            filterUpdate += Time.unscaledDeltaTime;
            if(filterUpdate >= 1f)
            {
                string[] loaded = LoadFiles(pageSelection == CustomMapsPage ? MapsPath : LogicsPath);
                if(filter != string.Empty)
                {
                    var list = new System.Collections.Generic.List<string>();
                    string flt = filter.ToLower();
                    foreach(string str in loaded)
                    {
                        if (str.ToLower().Contains(flt))
                        {
                            list.Add(str);
                        }
                    }
                    allNames = list.ToArray();
                }
                else
                {
                    allNames = loaded;
                }
                filterUpdate = 0f;
            }
        }

        protected override void OnAnyPageEnabled()
        {
            filter = string.Empty;
        }

        public override void OnUpdateScaling()
        {
            animator = new Animation.CenterAnimation(this, Helper.GetScreenMiddle(Style.WindowWidth, Style.WindowHeight));
        }
    }
}