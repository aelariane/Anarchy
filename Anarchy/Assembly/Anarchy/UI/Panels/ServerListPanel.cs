using Anarchy.Configuration.Presets;
using Anarchy.Network;
using Anarchy.UI.Animation;
using Optimization;
using System.Collections.Generic;
using UnityEngine;
using static Anarchy.UI.GUI;

namespace Anarchy.UI
{
    internal class ServerListPanel : GUIPanel
    {
        private const int ServerListPage = 0;
        private const int CreationPage = 1;
        private const int SettingsPage = 2;
        private const int PasswordRequestPage = 3;

        private const int PresetsVisibleCount = 6;
        private const float UpdateTime = 2f;
        private readonly string[] CustomServers = new string[] { "01042015", "verified343" };

        private Anarchy.Configuration.IntSetting mapSelectionSetting;

        private bool connected = false;
        private bool connectNext = false;
        private int customServer = 0;
        private string customServerField = string.Empty;
        private string[] customServers;
        private int daylight;
        private string[] daylights;
        private string[] difficulities;
        private int difficulity;
        private bool disconnectByJoin = false;
        private SmartRect left;
        private string[] maps;
        //private int mapSelection;
        private string maxPlayers;
        private string nameFilter;
        private string newPresetName;
        private int oldCustomServer;
        private int oldRegion;
        private string password;
        private int playersCount;
        private List<ServerPreset> presets;
        private Rect presetArea;
        private SmartRect presetRect;
        private Vector2 presetScroll;
        private Rect presetView;
        private string[] protocols;
        private string pwdInput;
        private SmartRect pwdRect;
        private SmartRect rect;
        private string[] regions;
        private int region;
        private SmartRect right;
        private List<RoomInfo> roomList;
        private RoomInfo roomToJoin;
        private string[] serProtocols;
        private string serverName;
        private string serverTime;
        private Vector2 scroll;
        private Rect scrollArea;
        private Rect scrollAreaView;
        private SmartRect scrollRect;
        private float timeToUpdate;

        public ServerListPanel() : base(nameof(ServerListPanel), GUILayers.ServerList)
        {
        }

        private void CheckReconnect()
        {
            if (oldRegion == region && customServer == oldCustomServer)
            {
                return;
            }

            if (customServer <= 1)
            {
                UIMainReferences.ConnectField = CustomServers[customServer];
            }
            else
            {
                UIMainReferences.ConnectField = customServerField;
            }
            oldCustomServer = customServer;
            oldRegion = region;
            connectNext = true;
            if (PhotonNetwork.connected)
            {
                PhotonNetwork.Disconnect();
            }
        }

        private bool CheckRoomFilters(RoomInfo room)
        {
            if (string.IsNullOrEmpty(nameFilter))
            {
                return true;
            }

            return room.Name.ToUpper().Contains(nameFilter.ToUpper());
        }

        [GUIPage(CreationPage, GUIPageType.DisableMethod)]
        private void DisableCreation()
        {
            left = null;
            right = null;
            maps = null;
            difficulities = null;
            daylights = null;
            serverName = null;
            serverTime = null;
            password = null;
            maxPlayers = null;
            if (presets != null && presets.Count > 0)
            {
                foreach (ServerPreset set in presets)
                {
                    set.Save();
                }
            }
            presets = null;
            newPresetName = null;
            presetRect = null;
            mapSelectionSetting = null;
        }

        [GUIPage(ServerListPage, GUIPageType.DisableMethod)]
        private void DisableList()
        {
            regions = null;
            nameFilter = null;
            roomList = null;
            rect = null;
            scrollRect = null;
            customServers = null;
        }

        [GUIPage(SettingsPage, GUIPageType.DisableMethod)]
        private void DisableSettings()
        {
            rect = null;
            regions = null;
            protocols = null;
            serProtocols = null;
        }

        protected override void DrawMainPart()
        {
        }

        [GUIPage(CreationPage, GUIPageType.EnableMethod)]
        private void EnableCreation()
        {
            head = locale["roomCreationTitle"];
            SmartRect[] rects = Helper.GetSmartRects(WindowPosition, 2);
            left = rects[0];
            right = rects[1];

            serverName = "Food for titoons";
            serverTime = "83";
            password = string.Empty;
            //mapSelection = 0;
            maps = new string[] { 
                "The City", "The Forest", "The Forest II", "The Forest III", "The Forest IV  - LAVA", "Annie", "Annie II", "Colossal Titan", "Colossal Titan II", 
                "Trost", "Trost II", "Racing - Akina", "Outside The Walls", "The City III", "Cave Fight", "House Fight", "Custom", "Custom (No PT)", //End RC maps
                //Custom maps, added in Anarchy or other mods
                "The City II", "The City IV", "The City V",
                //Anarchy maps
                "Custom-Anarchy (No PT)" };
            daylight = 0;
            daylights = locale.GetArray("dayLights");
            difficulity = 0;
            difficulities = locale.GetArray("difficulities");
            maxPlayers = "5";
            ServerPreset[] sets = ServerPreset.LoadPresets();
            presets = new List<ServerPreset>();
            if (sets != null)
            {
                foreach (ServerPreset set in sets)
                {
                    presets.Add(set);
                }
            }
            newPresetName = "Set " + (presets.Count + 1);
            if (sets.Length > 0)
            {
                newPresetName = sets[sets.Length - 1].Name;
            }
            presetArea = new Rect(left.x, 0f, left.width, Style.Height * PresetsVisibleCount + Style.VerticalMargin * (PresetsVisibleCount - 1));
            presetView = new Rect(0f, 0f, left.width, Style.Height * presets.Count + (Style.VerticalMargin * (presets.Count - 1)));
            presetRect = new SmartRect(0f, 0f, left.width, Style.Height, 0f, Style.VerticalMargin);
            presetScroll = Optimization.Caching.Vectors.v2zero;
            mapSelectionSetting = new Configuration.IntSetting("tempMapSelectionSetting", 0);
            mapSelectionSetting.Value = 0;
        }

        [GUIPage(ServerListPage, GUIPageType.EnableMethod)]
        private void EnableList()
        {
            head = locale["title"];
            rect = Helper.GetSmartRects(WindowPosition, 1)[0];
            scroll = Optimization.Caching.Vectors.v2zero;
            scrollRect = new SmartRect(0f, 0f, rect.width, rect.height, 0f, Style.VerticalMargin);
            scrollArea = new Rect(rect.x, rect.y, rect.width, WindowPosition.height - (4 * (Style.Height + Style.VerticalMargin)) - (Style.WindowTopOffset + Style.WindowBottomOffset) - 10f);
            scrollAreaView = new Rect(0f, 0f, rect.width, 1000f);
            roomList = new List<RoomInfo>();
            nameFilter = string.Empty;
            connected = PhotonNetwork.connected;
            region = NetworkSettings.PreferedRegion.Value;
            oldRegion = region;
            oldCustomServer = customServer;
            regions = locale.GetArray("regions");
            playersCount = 0;
            customServers = locale.GetArray("servers");
            UpdateRoomList();
            timeToUpdate = UpdateTime;
        }

        [GUIPage(SettingsPage, GUIPageType.EnableMethod)]
        private void EnableSettings()
        {
            head = locale["roomSettings"];
            rect = Helper.GetSmartRects(WindowPosition, 1)[0];
            regions = locale.GetArray("regions");
            serProtocols = new string[] { "GPBinaryV16", "GPBinaryV18" };
            protocols = new string[] { "UDP", "TCP", "WebSocket", "WebSocketSecure" };
        }

        private void ExportPreset(ServerPreset set)
        {
            set.Daylight = daylight;
            set.Difficulity = difficulity;
            set.Map = mapSelectionSetting.Value;
            set.Password = password;
            set.Players = maxPlayers;
            set.ServerName = serverName;
            set.Time = serverTime;
        }

        private void ImportPreset(ServerPreset preset)
        {
            daylight = preset.Daylight;
            difficulity = preset.Difficulity;
            mapSelectionSetting.Value = preset.Map;
            password = preset.Password;
            serverName = preset.ServerName;
            maxPlayers = preset.Players;
            serverTime = preset.Time;
            newPresetName = preset.Name;
        }

        protected override void OnPanelDisable()
        {
            NetworkingPeer.RemoveEvent(PhotonNetworkingMessage.OnJoinedLobby, OnJoinedLobby);
            NetworkingPeer.RemoveEvent(PhotonNetworkingMessage.OnDisconnectedFromPhoton, OnDisconnectedFromPhoton);
            NetworkingPeer.RemoveEvent(PhotonNetworkingMessage.OnReceivedRoomListUpdate, OnReceivedRoomListUpdate);
            connected = false;
            roomToJoin = null;
            if (!disconnectByJoin && PhotonNetwork.connected)
            {
                PhotonNetwork.Disconnect();
            }
            if (!PhotonNetwork.inRoom)
            {
                AnarchyManager.MainMenu.EnableImmediate();
            }
        }

        protected override void OnPanelEnable()
        {
            NetworkingPeer.RegisterEvent(PhotonNetworkingMessage.OnJoinedLobby, OnJoinedLobby);
            NetworkingPeer.RegisterEvent(PhotonNetworkingMessage.OnDisconnectedFromPhoton, OnDisconnectedFromPhoton);
            NetworkingPeer.RegisterEvent(PhotonNetworkingMessage.OnReceivedRoomListUpdate, OnReceivedRoomListUpdate);
            connectNext = false;
            regions = locale.GetArray("regions");
            TryConnect(NetworkSettings.PreferedRegion.Value);
            AnarchyManager.MainMenu.DisableImmediate();
        }

        private void OnJoinedLobby(AOTEventArgs args)
        {
            connected = true;
            try
            {
                head = locale["connected"] + " " + regions[region];
            }
            catch
            {
                Debug.Log(locale == null);
                Debug.Log(regions == null);
            }
            UpdateRoomList();
            timeToUpdate = UpdateTime;
        }

        private void OnDisconnectedFromPhoton(AOTEventArgs args)
        {
            connected = false;
            if (connectNext)
            {
                TryConnect(region);
                connectNext = false;
            }
        }

        private void OnReceivedRoomListUpdate(AOTEventArgs args)
        {
            UpdateRoomList();
            timeToUpdate = UpdateTime;
        }

        [GUIPage(PasswordRequestPage)]
        private void PasswordPage()
        {
            pwdRect.Reset();
            LabelCenter(pwdRect, locale["pwdRequest"], true);
            pwdInput = TextField(pwdRect, pwdInput, string.Empty, 0f, true);
            if (Button(pwdRect, locale["joinRoom"], true))
            {
                if (roomToJoin == null || !roomToJoin.IsCorrectPassword(pwdInput))
                {
                    pageSelection = ServerListPage;
                }
                else
                {
                    PhotonNetwork.JoinRoom(roomToJoin.Name);
                    disconnectByJoin = true;
                    DisableImmediate();
                }
            }
            else if (Button(pwdRect, locale["btnBack"], false))
            {
                pageSelection = ServerListPage;
            }
        }

        [GUIPage(PasswordRequestPage, GUIPageType.DisableMethod)]
        private void PasswordPageDisable()
        {
            pwdInput = null;
            pwdRect = null;
        }

        [GUIPage(PasswordRequestPage, GUIPageType.EnableMethod)]
        private void PasswordPageEnable()
        {
            pwdInput = string.Empty;
            Rect rect = Helper.GetScreenMiddle(Style.WindowWidth / 2f, Style.Height * 3f + Style.VerticalMargin * 2f);
            pwdRect = new SmartRect(rect.x, rect.y, rect.width, Style.Height, Style.HorizontalMargin, Style.VerticalMargin);
        }

        [GUIPage(CreationPage)]
        private void RoomCreation()
        {
            left.Reset();
            right.Reset();
            LabelCenter(left, locale["roomSettings"], true);
            serverName = TextField(left, serverName, locale["roomName"], Style.LabelOffset, true);
            password = TextField(left, password, locale["pwd"], Style.LabelOffset, true);
            serverTime = TextField(left, serverTime, locale["time"], Style.LabelOffset * 2f, true);
            maxPlayers = TextField(left, maxPlayers, locale["players"], Style.LabelOffset * 2f, true);

            LabelCenter(left, locale["difficulity"], true);
            difficulity = SelectionGrid(left, difficulity, difficulities, difficulities.Length, true);

            LabelCenter(left, locale["dayLight"], true);
            daylight = SelectionGrid(left, daylight, daylights, daylights.Length, true);
            left.MoveY();

            LabelCenter(left, locale["presets"], true);
            Label(left, locale["presetNote"], true);
            newPresetName = TextField(left, newPresetName, locale["presetName"], Style.LabelOffset, true);
            left.width = (left.DefaultWidth - Style.HorizontalMargin) / 2f;
            if (Button(left, locale["presetAdd"], false))
            {
                ServerPreset set = new ServerPreset(newPresetName);
                ExportPreset(set);
                presets.Add(set);
                presetView.height = (presets.Count * Style.Height) + ((presets.Count - 1) * Style.VerticalMargin);
                set.Save();
            }
            left.MoveX();
            if (Button(left, locale["presetRemove"], true))
            {
                if (presets.Count > 0)
                {
                    ServerPreset selected = null;
                    for (int i = 0; i < presets.Count; i++)
                    {
                        if (presets[i].Name == newPresetName)
                        {
                            selected = presets[i];
                        }
                    }
                    if (selected != null)
                    {
                        presets.Remove(selected);
                        selected.Delete();
                        newPresetName = "Set " + (presets.Count + 1);
                        if (presets.Count > 0)
                        {
                            newPresetName = presets[presets.Count - 1].Name;
                        }

                        presetView.height = (presets.Count * Style.Height) + ((presets.Count - 1) * Style.VerticalMargin);
                    }
                }
            }
            left.ResetX();
            if (presets.Count > 0)
            {
                presetArea.y = left.y;
                presetRect.Reset();
                presetScroll = BeginScrollView(presetArea, presetScroll, presetView);
                {
                    for (int i = 0; i < presets.Count; i++)
                    {
                        if (Button(presetRect, presets[i].Name, true))
                        {
                            ServerPreset selected = presets[i];
                            ImportPreset(selected);
                        }
                    }
                }
                EndScrollView();
            }

            left.MoveToEndY(WindowPosition, Style.Height);
            left.width = left.DefaultWidth / 2f - Style.HorizontalMargin;
            if (Button(left, locale["btnCreation"], false))
            {
                disconnectByJoin = true;
                string[] args = new string[]
                {
                    serverName,
                    maps[mapSelectionSetting],
                    new string[] { "normal", "hard", "abnormal" }[difficulity],
                    serverTime,
                    new string[] { "day", "dawn", "night" }[daylight],
                    password.Length > 0 ? new SimpleAES().Encrypt(password) : string.Empty,
                    UnityEngine.Random.Range(1000000, 10000000).ToString()
                };
                if (!int.TryParse(maxPlayers, out int max))
                {
                    max = 5;
                }
                PhotonNetwork.CreateRoom(string.Join("`", args), new RoomOptions() { isVisible = true, isOpen = true, maxPlayers = max }, null);
                DisableImmediate();
                AnarchyManager.Background.Disable();
                return;
            }
            left.MoveX(Style.HorizontalMargin, true);
            if (Button(left, locale["btnOffline"], false))
            {
                disconnectByJoin = true;
                PhotonNetwork.Disconnect();
                PhotonNetwork.offlineMode = true;
                string[] args = new string[]
                {
                    serverName,
                    maps[mapSelectionSetting.Value],
                    new string[] { "normal", "hard", "abnormal" }[difficulity],
                    serverTime,
                    new string[] { "day", "dawn", "night" }[daylight],
                    password.Length > 0 ? new SimpleAES().Encrypt(password) : string.Empty,
                    UnityEngine.Random.Range(1000000, 10000000).ToString()
                };
                if (!int.TryParse(maxPlayers, out int max))
                {
                    max = 5;
                }
                PhotonNetwork.CreateRoom(string.Join("`", args), new RoomOptions() { isVisible = true, isOpen = true, maxPlayers = max }, null);
                DisableImmediate();
                AnarchyManager.Background.Disable();
                return;
            }

            LabelCenter(right, locale["mapSelection"], true);
            //mapSelection = SelectionGrid(right, mapSelection, maps, 1);
            DropdownMenuScrollable(this, right, mapSelectionSetting, maps, 10, true);
            right.MoveY();
            right.MoveY();
            Label(right, LevelInfo.GetInfo(maps[mapSelectionSetting.Value], false).Description, true);
            right.MoveToEndY(WindowPosition, Style.Height);
            right.MoveToEndX(WindowPosition, new AutoScaleFloat(240f) + Style.HorizontalMargin);
            right.width = new AutoScaleFloat(120f);
            if (Button(right, locale["btnSettings"], false))
            {
                connected = false;
                pageSelection = SettingsPage;
                return;
            }
            right.MoveX();
            if (Button(right, locale["btnList"], false))
            {
                connected = PhotonNetwork.connected;
                if (connected)
                {
                    timeToUpdate = 0.1f;
                }
                pageSelection = ServerListPage;
                return;
            }
        }

        [GUIPage(ServerListPage)]
        private void ServerList()
        {
            if (Event.current != null && Event.current.type == EventType.KeyDown && UnityEngine.GUI.GetNameOfFocusedControl() == "ServerListFilter")
            {
                UpdateRoomList();
            }

            rect.Reset();
            region = SelectionGrid(rect, region, regions, regions.Length, true);

            Label(rect, locale["customServer"], false);
            float offset = ((scrollArea.width - (Style.HorizontalMargin * 3)) / 4f) + Style.HorizontalMargin;
            rect.MoveOffsetX(offset);
            rect.width -= (new AutoScaleFloat(100f) - Style.HorizontalMargin);
            float txt = offset + rect.width;
            customServer = SelectionGrid(rect, customServer, customServers, customServers.Length, false);

            rect.ResetX();
            rect.MoveOffsetX(txt + Style.HorizontalMargin);
            customServerField = TextField(rect, customServerField, string.Empty, 0f, true);

            rect.ResetX();
            CheckReconnect();
            UnityEngine.GUI.SetNextControlName("ServerListFilter");
            nameFilter = TextField(rect, nameFilter, locale["filter"], offset, true);

            rect.ResetX();
            rect.MoveY(Style.VerticalMargin);
            scrollRect.Reset();
            scrollArea.y = rect.y;
            scroll = BeginScrollView(scrollArea, scroll, scrollAreaView);
            if (connected)
            {
                foreach (var room in roomList)
                {
                    if (Button(scrollRect, room.UIName.ToHTMLFormat(), true) && room.PlayerCount != room.MaxPlayers)
                    {
                        if (room.HasPassword)
                        {
                            roomToJoin = room;
                            pageSelection = PasswordRequestPage;
                        }
                        else
                        {
                            PhotonNetwork.JoinRoom(room.Name);
                            disconnectByJoin = true;
                            DisableImmediate();
                        }
                        return;
                    }
                }
            }
            EndScrollView();

            rect.MoveToEndY(WindowPosition, Style.Height);
            rect.width = new AutoScaleFloat(170f);
            if (Button(rect, locale["btnCreation"], false))
            {
                pageSelection = CreationPage;
                connected = false;
                return;
            }
            rect.MoveX(new AutoScaleFloat(5f), true);
            if (Button(rect, locale["btnSettings"], false))
            {
                pageSelection = SettingsPage;
                connected = false;
                return;
            }
            rect.width = new AutoScaleFloat(120f);
            rect.MoveToEndX(WindowPosition, new AutoScaleFloat(120f));
            if (Button(rect, locale["btnBack"]))
            {
                Disable();
                return;
            }
        }

        [GUIPage(SettingsPage)]
        private void Settings()
        {
            rect.Reset();
            LabelCenter(rect, locale["preferedRegion"], true);
            rect.MoveY();
            SelectionGrid(rect, NetworkSettings.PreferedRegion, regions, regions.Length, true);
            Label(rect, locale["preferedRegionDesc"], true);
            rect.MoveY();
            LabelCenter(rect, locale["connectionProtocol"], true);
            rect.MoveY();
            int prot = NetworkSettings.ConnectionProtocol;
            SelectionGrid(rect, NetworkSettings.ConnectionProtocol, protocols, protocols.Length, true);
            if (prot != NetworkSettings.ConnectionProtocol.Value)
            {
                PhotonNetwork.SwitchToProtocol(NetworkSettings.ConnectProtocol);
            }
            Label(rect, locale["connectionProtocolDescUDP"], true);
            Label(rect, locale["connectionProtocolDescTCP"], true);
            Label(rect, locale["connectionProtocolDescWS"], true);
            rect.ResetX();
            rect.MoveToEndY(WindowPosition, Style.Height * 2f + Style.VerticalMargin);
            Label(rect, locale["settingsDesc"], true);
            rect.MoveToEndX(WindowPosition, new AutoScaleFloat(240f) + Style.HorizontalMargin);
            rect.width = new AutoScaleFloat(120f);
            if (Button(rect, locale["btnCreation"], false))
            {
                pageSelection = CreationPage;
                return;
            }
            rect.MoveX();
            if (Button(rect, locale["btnList"], true))
            {
                connected = PhotonNetwork.connected;
                if (connected)
                {
                    timeToUpdate = 0.1f;
                }
                pageSelection = ServerListPage;
                return;
            }
        }

        private void TryConnect(int selection)
        {
            if (PhotonNetwork.connected)
            {
                PhotonNetwork.Disconnect();
            }

            head = locale["connecting"] + " " + regions[selection] + "...";
            bool result = PhotonNetwork.ConnectToMaster(string.Format(NetworkSettings.AdressString, NetworkSettings.RegionAdresses[selection]), NetworkingPeer.ProtocolToNameServerPort[PhotonNetwork.networkingPeer.UsedProtocol], FengGameManagerMKII.ApplicationId, UIMainReferences.ConnectField);
            if (!result)
            {
                FengGameManagerMKII.FGM.StartCoroutine(TryConnectI(selection));
            }
        }

        private System.Collections.IEnumerator TryConnectI(int sel)
        {
            yield return new WaitForSeconds(0.5f);
            TryConnect(sel);
        }

        public override void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Disable();
                return;
            }
            if (!connected)
            {
                return;
            }

            timeToUpdate -= Time.deltaTime;
            if (timeToUpdate <= 0f)
            {
                UpdateRoomList();
                timeToUpdate = UpdateTime;
            }
        }

        private void UpdateRoomList()
        {
            if (!connected || roomList == null)
            {
                return;
            }

            lock (roomList)
            {
                playersCount = 0;
                roomList.Clear();
                RoomInfo[] rooms = PhotonNetwork.GetRoomList();
                foreach (RoomInfo room in rooms)
                {
                    if (CheckRoomFilters(room))
                    {
                        roomList.Add(room);
                    }
                    playersCount += room.PlayerCount;
                }
                head = $"{locale["connected"]} {regions[region]} [{locale["players"]} {playersCount}, {locale["ping"]} {PhotonNetwork.GetPing()}]";
                scrollAreaView.height = (Style.Height * roomList.Count) + (Style.VerticalMargin * (roomList.Count - 1));
            }
        }
    }
}
