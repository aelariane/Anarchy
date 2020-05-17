using System;
using Anarchy.Configuration;
using Anarchy.UI;
using System.Collections;
using ExitGames.Client.Photon;
using UnityEngine;

namespace Anarchy
{
    internal class AnarchyManager : MonoBehaviour
    {
        //In case you want to make your mod synchronizeable with public anarchy version
        //Note: Anarchy sync with current public version will work if
        //1. AnarchyVersion equals to public's mod AnarchyVersion
        //2. CustomVersion turned to true AND FullAnarchySync turned to true AND CustomName not equals string.Empty or ""
        //All of 3 of them should math this rule to have sync with current public version

        //In case if you want to make sync only between YOUR version. Just set CustomName to something that not equals string.Empty or ""

        //And AnarchyVersion should match as well in ANY case if you want any kind of sync
        public static readonly Version AnarchyVersion = new Version("0.7.8.0");

        public static readonly string CustomName = string.Empty;
        public static readonly bool FullAnarchySync = true;

        public static Background Background;
        public static GameFeed Feed;
        public static UI.PanelMain MainMenu;
        public static PausePanel Pause;
        public static PauseWindow PauseWindow;
        public static ProfilePanel ProfilePanel;
        public static ServerListPanel ServerList;
        public static SettingsPanel SettingsPanel;
        public static SinglePanel SinglePanel;
        public static DebugPanel DebugPanel;
        public static CharacterSelectionPanel CharacterSelectionPanel;
        public static Chat Chat;
        public static Log Log;
        public static SingleStatsPanel StatsPanel;

        private void Awake()
        {
            StartCoroutine(OnGameWasOpened());
            DontDestroyOnLoad(this);
            Feed = new GameFeed();
            Background = new Background();
            MainMenu = new UI.PanelMain();
            Pause = new PausePanel();
            PauseWindow = new PauseWindow();
            ProfilePanel = new ProfilePanel();
            SinglePanel = new SinglePanel();
            ServerList = new ServerListPanel();
            SettingsPanel = new SettingsPanel();
            DebugPanel = new DebugPanel();
            CharacterSelectionPanel = new CharacterSelectionPanel();
            Chat = new Chat();
            Log = new Log();
            DontDestroyOnLoad(new GameObject("DiscordManager").AddComponent<Network.Discord.DiscordManager>());
            DestroyMainScene();
            GameModes.ResetOnLoad();
            //Antis.Spam.EventsCounter.OnEventsSpamDetected += (sender, args) =>
            //{
            //    if (args.SpammedObject == 200 || args.SpammedObject == 253 && args.Count < 130)
            //    {
            //        return;
            //    }
            //    PhotonPlayer player = PhotonPlayer.Find(args.Sender);
            //    if (player.RCIgnored)
            //    {
            //        return;
            //    }
            //    Log.AddLine("eventSpam", args.SpammedObject.ToString(), args.Sender.ToString(), args.Count.ToString());
            //};
            //Antis.Spam.RPCCounter.OnRPCSpamDetected += (sender, args) =>
            //{
            //    if (args.SpammedObject == "netPauseAnimation" || args.SpammedObject == "netCrossFade" && args.Count < 75)
            //    {
            //        return;
            //    }
            //    PhotonPlayer player = PhotonPlayer.Find(args.Sender);
            //    if (player.RCIgnored)
            //    {
            //        return;
            //    }
            //    Log.AddLine("rpcSpam", args.SpammedObject.ToString(), args.Sender.ToString(), args.Count.ToString());
            //};
            //Antis.Spam.InstantiateCounter.OnInstantiateSpamDetected += (sender, args) =>
            //{
            //    if (args.SpammedObject.Contains("TITAN") && args.Count <= 50)
            //    {
            //        return;
            //    }
            //    PhotonPlayer player = PhotonPlayer.Find(args.Sender);
            //    if (player.RCIgnored)
            //    {
            //        return;
            //    }
            //    Log.AddLine("instantiateSpam", args.SpammedObject.ToString(), args.Sender.ToString(), args.Count.ToString());
            //};
            Antis.AntisManager.OnResponseCallback += (id, banned, reason) =>
            {
                Log.AddLineRaw($"Player [{id}] has been {(banned ? "banned" : "kicked")}. " +
                                  $"{(reason == "" ? "" : $"reason: {reason}")}");
            };
            StatsPanel = new SingleStatsPanel();
            Network.BanList.Load();
        }

        public static void DestroyMainScene()
        {
            var objectsToDestroy = new[]
            {
                "PanelLogin",
                "LOGIN",
                "BG_TITLE",
                "Colossal",
                "Icosphere",
                "cube",
                "colossal",
                "CITY",
                "city",
                "rock",
                "AOTTG_HERO",
                "Checkbox",
            };
            var gos = (GameObject[]) FindObjectsOfType(typeof(GameObject));
            foreach (var name in objectsToDestroy)
            {
                foreach (var go in gos)
                {
                    if (go.name.Contains(name) ||
                        (go.GetComponent<UILabel>() != null && go.GetComponent<UILabel>().text.Contains("Snap")) ||
                        (go.GetComponent<UILabel>() != null && go.GetComponent<UILabel>().text.Contains("Custom")))
                    {
                        Destroy(go);
                    }
                }
            }
        }

        private void OnApplicationQuit()
        {
            User.Save();
            Network.BanList.Save();
            GameModes.Load();
            GameModes.Save();
            Settings.Save();
            Style.Save();
        }

        private void OnLevelWasLoaded(int id)
        {
            if (Application.loadedLevelName == "menu")
            {
                if (!Background.Active)
                {
                    Background.Enable();
                }

                if (Chat != null && Chat.Active)
                {
                    Chat.Disable();
                    Chat.Clear();
                }

                if (Log != null && Log.Active)
                {
                    Log.Disable();
                    Log.Clear();
                }
                DestroyMainScene();
                GameModes.ResetOnLoad();
                Network.BanList.Save();
                Skins.Humans.HumanSkin.Storage.Clear();
            }
            else
            {
                if(IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
                {
                    SingleRunStats.Reset();
                }
                if (Background.Active)
                {
                    Background.Disable();
                }

                if (Application.loadedLevelName != "characterCreation" && Application.loadedLevelName != "SnapShot" &&
                    PhotonNetwork.inRoom)
                {
                    if (Chat != null && !Chat.Active)
                    {
                        Chat.Enable();
                    }

                    if (Log != null && !Log.Active)
                    {
                        Log.Enable();
                    }
                }
            }

            Pause?.Continue();
            Settings.Apply();
            VideoSettings.Apply();
            if (PauseWindow.Active)
            {
                PauseWindow.DisableImmediate();
            }
            if (StatsPanel.Active)
            {
                StatsPanel.DisableImmediate();
            }
        }


        private IEnumerator OnGameWasOpened()
        {
            var back = new GameObject("TempBackground").AddComponent<BackgroundOnStart>();
            yield return StartCoroutine(AnarchyAssets.LoadAssetBundle());
            Instantiate(AnarchyAssets.Load("UIManager"));
            Instantiate(AnarchyAssets.Load("LoadScreen"));
            Destroy(back);
        }

        private void Update()
        {
        }
    }
}
