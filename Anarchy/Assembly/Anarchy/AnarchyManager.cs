using Anarchy.Configuration;
using Anarchy.Inputs;
using Anarchy.UI;
using ExitGames.Client.Photon;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Anarchy
{
    /// <summary>
    /// Main entrace point of Anarchy mod
    /// </summary>
    internal class AnarchyManager : MonoBehaviour
    {
        //In case you want to make your mod synchronizeable with public anarchy version
        //Note: Anarchy sync with current public version will work if
        //1. AnarchyVersion equals to public's mod AnarchyVersion
        //2. CustomVersion turned to true AND FullAnarchySync turned to true AND CustomName not equals string.Empty or ""
        //All of 3 of them should math this rule to have sync with current public version

        //In case if you want to make sync only between YOUR version. Just set CustomName to something that not equals string.Empty or ""

        //And AnarchyVersion should match as well in ANY case if you want any kind of sync
        public static readonly Version AnarchyVersion = new Version("0.9.6.2");

        /// <summary>
        /// Your version Custom name
        /// </summary>
        public static readonly string CustomName = string.Empty;
        /// <summary>
        /// If you want to use full Anarchy synchronization feautures
        /// </summary>
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
        public static ChatHistoryPanel ChatHistory;

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
            ChatHistory = new ChatHistoryPanel();
            StatsPanel = new SingleStatsPanel();
            DontDestroyOnLoad(new GameObject("DiscordManager").AddComponent<Network.Discord.DiscordSDK>());
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
            //Antis.AntisManager.ResponseAction += (id, ban, reason) =>
            //{
            //    var player = PhotonPlayer.Find(id);
            //    if(player == null)
            //    {
            //        return;
            //    }
            //    Network.Antis.Kick(player, ban, reason);
            //};
            Network.BanList.Load();
            Antis.AntisManager.ResponseAction += (a, b, c) => { Network.Antis.Kick(PhotonPlayer.Find(a), b, c); };
            Antis.AntisManager.OnResponseCallback += (id, banned, reason) =>
            {
                Log.AddLineRaw($"Player [{id}] has been {(banned ? "banned" : "kicked")}. " +
                                  $"{(reason == "" ? "" : $"reason: {reason}")}");
            };
        }

        /// <summary>
        /// Imports settings from RC mod
        /// </summary>
        public static void ImportRCSettings()
        {
            //Importing titan skins
            SkinSettings.TitanSkins.Value = PlayerPrefs.GetInt("titan", 0) > 2 ? 0 : PlayerPrefs.GetInt("titan", 0);
            var titanSet = new Configuration.Presets.TitanSkinPreset("Set 1 (Imported)");
            for(int i = 1; i <= 5; i++)
            {
                titanSet.HairTypes[i - 1] = (Configuration.Presets.TitanSkinPreset.HairType) PlayerPrefs.GetInt("titiantype" + i, 0); //Hair type
                titanSet.Hairs[i - 1] = PlayerPrefs.GetString("titanhair" + i, string.Empty); //Hair links
                titanSet.Eyes[i - 1] = PlayerPrefs.GetString("titaneye" + i, string.Empty); //Eye links
                titanSet.Bodies[i - 1] = PlayerPrefs.GetString("titanbody" + i, string.Empty); //Body links
            }
            titanSet.Colossal = PlayerPrefs.GetString("colossal", string.Empty); //Colossal
            titanSet.Annie = PlayerPrefs.GetString("annie", string.Empty); //Annie
            titanSet.Eren = PlayerPrefs.GetString("eren", string.Empty); //Eren

            titanSet.RandomizePairs = PlayerPrefs.GetInt("titanR", 0) == 1;
            titanSet.Save();
            SkinSettings.TitanSet.Value = titanSet.Name;

            //Importing
            string[] citySkinKeys = new string[]
            {

            };

            string[] forestSkinKeys = new string[]
            {

            };

            //Importing human skins
            string[] humanSkinKeys = new string[]
            {
                "horse",
                "hair",
                "eye",
                "glass",
                "face",
                "skin",
                "costume",
                "logo",
                "bladel",
                "blader",
                "gas",
                "hoodie",
                "trail"
            };
            SkinSettings.HumanSkins.Value = PlayerPrefs.GetInt("human", 0) > 2 ? 0 : PlayerPrefs.GetInt("human", 0);
            for (int i = 0; i < 3; i++)
            {
                var set = new Configuration.Presets.HumanSkinPreset("Set " + (i + 1).ToString() + " (Imported)");
                for(int j = 0; j < humanSkinKeys.Length; j++)
                {
                    string key = humanSkinKeys[j];
                    if(i > 0)
                    {
                        key += (i + 1).ToString();
                    }
                    set.SkinData[j] = PlayerPrefs.GetString(key, string.Empty);
                }
                set.Save();
            }
            SkinSettings.HumanSet.Value = "Set " + (PlayerPrefs.GetInt("humangui", 0) + 1).ToString() + " (Imported)"; //Set selection
            SkinSettings.DisableCustomGas.Value = PlayerPrefs.GetInt("gasenable", 0) == 0; //If custom gas textures are enabled

            SkinSettings.CitySkins.Value = PlayerPrefs.GetInt("level", 0) > 2 ? 0 : PlayerPrefs.GetInt("level", 0);
            SkinSettings.ForestSkins.Value = PlayerPrefs.GetInt("level", 0) > 2 ? 0 : PlayerPrefs.GetInt("level", 0);
            SkinSettings.CustomSkins.Value = PlayerPrefs.GetInt("level", 0) > 2 ? 0 : PlayerPrefs.GetInt("level", 0);
                

            //PlayerPrefs.GetString("tree1", (string)FengGameManagerMKII.settings[33]);
            //PlayerPrefs.GetString("tree2", (string)FengGameManagerMKII.settings[34]);
            //PlayerPrefs.GetString("tree3", (string)FengGameManagerMKII.settings[35]);
            //PlayerPrefs.GetString("tree4", (string)FengGameManagerMKII.settings[36]);
            //PlayerPrefs.GetString("tree5", (string)FengGameManagerMKII.settings[37]);
            //PlayerPrefs.GetString("tree6", (string)FengGameManagerMKII.settings[38]);
            //PlayerPrefs.GetString("tree7", (string)FengGameManagerMKII.settings[39]);
            //PlayerPrefs.GetString("tree8", (string)FengGameManagerMKII.settings[40]);
            //PlayerPrefs.GetString("leaf1", (string)FengGameManagerMKII.settings[41]);
            //PlayerPrefs.GetString("leaf2", (string)FengGameManagerMKII.settings[42]);
            //PlayerPrefs.GetString("leaf3", (string)FengGameManagerMKII.settings[43]);
            //PlayerPrefs.GetString("leaf4", (string)FengGameManagerMKII.settings[44]);
            //PlayerPrefs.GetString("leaf5", (string)FengGameManagerMKII.settings[45]);
            //PlayerPrefs.GetString("leaf6", (string)FengGameManagerMKII.settings[46]);
            //PlayerPrefs.GetString("leaf7", (string)FengGameManagerMKII.settings[47]);
            //PlayerPrefs.GetString("leaf8", (string)FengGameManagerMKII.settings[48]);
            //PlayerPrefs.GetString("forestG", (string)FengGameManagerMKII.settings[49]);
            //PlayerPrefs.GetInt("forestR", (int)FengGameManagerMKII.settings[50]);
            //PlayerPrefs.GetString("house1", (string)FengGameManagerMKII.settings[51]);
            //PlayerPrefs.GetString("house2", (string)FengGameManagerMKII.settings[52]);
            //PlayerPrefs.GetString("house3", (string)FengGameManagerMKII.settings[53]);
            //PlayerPrefs.GetString("house4", (string)FengGameManagerMKII.settings[54]);
            //PlayerPrefs.GetString("house5", (string)FengGameManagerMKII.settings[55]);
            //PlayerPrefs.GetString("house6", (string)FengGameManagerMKII.settings[56]);
            //PlayerPrefs.GetString("house7", (string)FengGameManagerMKII.settings[57]);
            //PlayerPrefs.GetString("house8", (string)FengGameManagerMKII.settings[58]);
            //PlayerPrefs.GetString("cityG", (string)FengGameManagerMKII.settings[59]);
            //PlayerPrefs.GetString("cityW", (string)FengGameManagerMKII.settings[60]);
            //PlayerPrefs.GetString("cityH", (string)FengGameManagerMKII.settings[61]);
            //PlayerPrefs.GetInt("skinQ", QualitySettings.masterTextureLimit);
            //PlayerPrefs.GetInt("skinQL", (int)FengGameManagerMKII.settings[63]);

            //PlayerPrefs.GetString("cnumber", (string)FengGameManagerMKII.settings[82]);
            //PlayerPrefs.GetString("cmax", (string)FengGameManagerMKII.settings[85]);
            //PlayerPrefs.GetInt("customlevel", (int)FengGameManagerMKII.settings[91]);
            //PlayerPrefs.GetInt("traildisable", (int)FengGameManagerMKII.settings[92]);
            //PlayerPrefs.GetInt("wind", (int)FengGameManagerMKII.settings[93]);
            //PlayerPrefs.GetString("trailskin", (string)FengGameManagerMKII.settings[94]);
            //PlayerPrefs.GetString("snapshot", (string)FengGameManagerMKII.settings[95]);
            //PlayerPrefs.GetString("trailskin2", (string)FengGameManagerMKII.settings[96]);
            //PlayerPrefs.GetInt("reel", (int)FengGameManagerMKII.settings[97]);
            //PlayerPrefs.GetString("reelin", (string)FengGameManagerMKII.settings[98]);
            //PlayerPrefs.GetString("reelout", (string)FengGameManagerMKII.settings[99]);
            //PlayerPrefs.GetFloat("vol", AudioListener.volume);
            //PlayerPrefs.GetString("tforward", (string)FengGameManagerMKII.settings[101]);
            //PlayerPrefs.GetString("tback", (string)FengGameManagerMKII.settings[102]);
            //PlayerPrefs.GetString("tleft", (string)FengGameManagerMKII.settings[103]);
            //PlayerPrefs.GetString("tright", (string)FengGameManagerMKII.settings[104]);
            //PlayerPrefs.GetString("twalk", (string)FengGameManagerMKII.settings[105]);
            //PlayerPrefs.GetString("tjump", (string)FengGameManagerMKII.settings[106]);
            //PlayerPrefs.GetString("tpunch", (string)FengGameManagerMKII.settings[107]);
            //PlayerPrefs.GetString("tslam", (string)FengGameManagerMKII.settings[108]);
            //PlayerPrefs.GetString("tgrabfront", (string)FengGameManagerMKII.settings[109]);
            //PlayerPrefs.GetString("tgrabback", (string)FengGameManagerMKII.settings[110]);
            //PlayerPrefs.GetString("tgrabnape", (string)FengGameManagerMKII.settings[111]);
            //PlayerPrefs.GetString("tantiae", (string)FengGameManagerMKII.settings[112]);
            //PlayerPrefs.GetString("tbite", (string)FengGameManagerMKII.settings[113]);
            //PlayerPrefs.GetString("tcover", (string)FengGameManagerMKII.settings[114]);
            //PlayerPrefs.GetString("tsit", (string)FengGameManagerMKII.settings[115]);
            //PlayerPrefs.GetInt("reel2", (int)FengGameManagerMKII.settings[116]);

            //PlayerPrefs.GetString("customGround", (string)FengGameManagerMKII.settings[162]);
            //PlayerPrefs.GetString("forestskyfront", (string)FengGameManagerMKII.settings[163]);
            //PlayerPrefs.GetString("forestskyback", (string)FengGameManagerMKII.settings[164]);
            //PlayerPrefs.GetString("forestskyleft", (string)FengGameManagerMKII.settings[165]);
            //PlayerPrefs.GetString("forestskyright", (string)FengGameManagerMKII.settings[166]);
            //PlayerPrefs.GetString("forestskyup", (string)FengGameManagerMKII.settings[167]);
            //PlayerPrefs.GetString("forestskydown", (string)FengGameManagerMKII.settings[168]);
            //PlayerPrefs.GetString("cityskyfront", (string)FengGameManagerMKII.settings[169]);
            //PlayerPrefs.GetString("cityskyback", (string)FengGameManagerMKII.settings[170]);
            //PlayerPrefs.GetString("cityskyleft", (string)FengGameManagerMKII.settings[171]);
            //PlayerPrefs.GetString("cityskyright", (string)FengGameManagerMKII.settings[172]);
            //PlayerPrefs.GetString("cityskyup", (string)FengGameManagerMKII.settings[173]);
            //PlayerPrefs.GetString("cityskydown", (string)FengGameManagerMKII.settings[174]);
            //PlayerPrefs.GetString("customskyfront", (string)FengGameManagerMKII.settings[175]);
            //PlayerPrefs.GetString("customskyback", (string)FengGameManagerMKII.settings[176]);
            //PlayerPrefs.GetString("customskyleft", (string)FengGameManagerMKII.settings[177]);
            //PlayerPrefs.GetString("customskyright", (string)FengGameManagerMKII.settings[178]);
            //PlayerPrefs.GetString("customskyup", (string)FengGameManagerMKII.settings[179]);
            //PlayerPrefs.GetString("customskydown", (string)FengGameManagerMKII.settings[180]);
            //PlayerPrefs.GetInt("dashenable", (int)FengGameManagerMKII.settings[181]);
            //PlayerPrefs.GetString("dashkey", (string)FengGameManagerMKII.settings[182]);
            //PlayerPrefs.GetInt("vsync", (int)FengGameManagerMKII.settings[183]);
            //PlayerPrefs.GetString("fpscap", (string)FengGameManagerMKII.settings[184]);
            //PlayerPrefs.GetInt("speedometer", (int)FengGameManagerMKII.settings[189]);
            //PlayerPrefs.GetInt("bombMode", (int)FengGameManagerMKII.settings[192]);
            //PlayerPrefs.GetInt("teamMode", (int)FengGameManagerMKII.settings[193]);
            //PlayerPrefs.GetInt("rockThrow", (int)FengGameManagerMKII.settings[194]);
            //PlayerPrefs.GetInt("explodeModeOn", (int)FengGameManagerMKII.settings[195]);
            //PlayerPrefs.GetString("explodeModeNum", (string)FengGameManagerMKII.settings[196]);
            //PlayerPrefs.GetInt("healthMode", (int)FengGameManagerMKII.settings[197]);
            //PlayerPrefs.GetString("healthLower", (string)FengGameManagerMKII.settings[198]);
            //PlayerPrefs.GetString("healthUpper", (string)FengGameManagerMKII.settings[199]);
            //PlayerPrefs.GetInt("infectionModeOn", (int)FengGameManagerMKII.settings[200]);
            //PlayerPrefs.GetString("infectionModeNum", (string)FengGameManagerMKII.settings[201]);
            //PlayerPrefs.GetInt("banEren", (int)FengGameManagerMKII.settings[202]);
            //PlayerPrefs.GetInt("moreTitanOn", (int)FengGameManagerMKII.settings[203]);
            //PlayerPrefs.GetString("moreTitanNum", (string)FengGameManagerMKII.settings[204]);
            //PlayerPrefs.GetInt("damageModeOn", (int)FengGameManagerMKII.settings[205]);
            //PlayerPrefs.GetString("damageModeNum", (string)FengGameManagerMKII.settings[206]);
            //PlayerPrefs.GetInt("sizeMode", (int)FengGameManagerMKII.settings[207]);
            //PlayerPrefs.GetString("sizeLower", (string)FengGameManagerMKII.settings[208]);
            //PlayerPrefs.GetString("sizeUpper", (string)FengGameManagerMKII.settings[209]);
            //PlayerPrefs.GetInt("spawnModeOn", (int)FengGameManagerMKII.settings[210]);
            //PlayerPrefs.GetString("nRate", (string)FengGameManagerMKII.settings[211]);
            //PlayerPrefs.GetString("aRate", (string)FengGameManagerMKII.settings[212]);
            //PlayerPrefs.GetString("jRate", (string)FengGameManagerMKII.settings[213]);
            //PlayerPrefs.GetString("cRate", (string)FengGameManagerMKII.settings[214]);
            //PlayerPrefs.GetString("pRate", (string)FengGameManagerMKII.settings[215]);
            //PlayerPrefs.GetInt("horseMode", (int)FengGameManagerMKII.settings[216]);
            //PlayerPrefs.GetInt("waveModeOn", (int)FengGameManagerMKII.settings[217]);
            //PlayerPrefs.GetString("waveModeNum", (string)FengGameManagerMKII.settings[218]);
            //PlayerPrefs.GetInt("friendlyMode", (int)FengGameManagerMKII.settings[219]);
            //PlayerPrefs.GetInt("pvpMode", (int)FengGameManagerMKII.settings[220]);
            //PlayerPrefs.GetInt("maxWaveOn", (int)FengGameManagerMKII.settings[221]);
            //PlayerPrefs.GetString("maxWaveNum", (string)FengGameManagerMKII.settings[222]);
            //PlayerPrefs.GetInt("endlessModeOn", (int)FengGameManagerMKII.settings[223]);
            //PlayerPrefs.GetString("endlessModeNum", (string)FengGameManagerMKII.settings[224]);
            //PlayerPrefs.GetString("motd", (string)FengGameManagerMKII.settings[225]);
            //PlayerPrefs.GetInt("pointModeOn", (int)FengGameManagerMKII.settings[226]);
            //PlayerPrefs.GetString("pointModeNum", (string)FengGameManagerMKII.settings[227]);
            //PlayerPrefs.GetInt("ahssReload", (int)FengGameManagerMKII.settings[228]);
            //PlayerPrefs.GetInt("punkWaves", (int)FengGameManagerMKII.settings[229]);
            //PlayerPrefs.GetInt("mapOn", (int)FengGameManagerMKII.settings[231]);
            //PlayerPrefs.GetString("mapMaximize", (string)FengGameManagerMKII.settings[232]);
            //PlayerPrefs.GetString("mapToggle", (string)FengGameManagerMKII.settings[233]);
            //PlayerPrefs.GetString("mapReset", (string)FengGameManagerMKII.settings[234]);
            //PlayerPrefs.GetInt("globalDisableMinimap", (int)FengGameManagerMKII.settings[235]);
            //PlayerPrefs.GetString("chatRebind", (string)FengGameManagerMKII.settings[236]);
            //PlayerPrefs.GetString("hforward", (string)FengGameManagerMKII.settings[237]);
            //PlayerPrefs.GetString("hback", (string)FengGameManagerMKII.settings[238]);
            //PlayerPrefs.GetString("hleft", (string)FengGameManagerMKII.settings[239]);
            //PlayerPrefs.GetString("hright", (string)FengGameManagerMKII.settings[240]);
            //PlayerPrefs.GetString("hwalk", (string)FengGameManagerMKII.settings[241]);
            //PlayerPrefs.GetString("hjump", (string)FengGameManagerMKII.settings[242]);
            //PlayerPrefs.GetString("hmount", (string)FengGameManagerMKII.settings[243]);
            //PlayerPrefs.GetInt("chatfeed", (int)FengGameManagerMKII.settings[244]);
            //PlayerPrefs.GetFloat("bombR", (float)FengGameManagerMKII.settings[246]);
            //PlayerPrefs.GetFloat("bombG", (float)FengGameManagerMKII.settings[247]);
            //PlayerPrefs.GetFloat("bombB", (float)FengGameManagerMKII.settings[248]);
            //PlayerPrefs.GetFloat("bombA", (float)FengGameManagerMKII.settings[249]);
            //PlayerPrefs.GetInt("bombRadius", (int)FengGameManagerMKII.settings[250]);
            //PlayerPrefs.GetInt("bombRange", (int)FengGameManagerMKII.settings[251]);
            //PlayerPrefs.GetInt("bombSpeed", (int)FengGameManagerMKII.settings[252]);
            //PlayerPrefs.GetInt("bombCD", (int)FengGameManagerMKII.settings[253]);
            //PlayerPrefs.GetString("cannonUp", (string)FengGameManagerMKII.settings[254]);
            //PlayerPrefs.GetString("cannonDown", (string)FengGameManagerMKII.settings[255]);
            //PlayerPrefs.GetString("cannonLeft", (string)FengGameManagerMKII.settings[256]);
            //PlayerPrefs.GetString("cannonRight", (string)FengGameManagerMKII.settings[257]);
            //PlayerPrefs.GetString("cannonFire", (string)FengGameManagerMKII.settings[258]);
            //PlayerPrefs.GetString("cannonMount", (string)FengGameManagerMKII.settings[259]);
            //PlayerPrefs.GetString("cannonSlow", (string)FengGameManagerMKII.settings[260]);
            //PlayerPrefs.GetInt("deadlyCannon", (int)FengGameManagerMKII.settings[261]);
            //PlayerPrefs.GetString("liveCam", (string)FengGameManagerMKII.settings[262]);
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
            var gos = (GameObject[])FindObjectsOfType(typeof(GameObject));
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
            Antis.AntisThreadManager.OnApplicationQuit();
            try
            {
                User.Save();
                Network.BanList.Save();
                GameModes.Load();
                GameModes.Save();
                Settings.Save();
                Style.Save();
            }
            catch(Exception ex)
            {
                UnityEngine.Debug.Log("Error occured on ApplicationQuit\n" + ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void OnLevelWasLoaded(int id)
        {
            if (Application.loadedLevelName == "menu")
            {
                if (!Background.IsActive)
                {
                    Background.Enable();
                }

                if (Chat != null && Chat.IsActive)
                {
                    Chat.Disable();
                    Chat.Clear();
                }

                if (Log != null && Log.IsActive)
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
                if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single)
                {
                    SingleRunStats.Reset();

                }
                if (Background.IsActive)
                {
                    Background.Disable();
                }

                if (Application.loadedLevelName != "characterCreation" && Application.loadedLevelName != "SnapShot" &&
                    PhotonNetwork.inRoom)
                {
                    if (Chat != null && !Chat.IsActive)
                    {
                        Chat.Enable();
                    }

                    if (Log != null && !Log.IsActive)
                    {
                        Log.Enable();
                    }
                }
            }

            PhotonNetwork.player.SetCustomProperties(new ExitGames.Client.Photon.Hashtable { { PhotonPlayerProperty.anarchyFlags, 0 }, { PhotonPlayerProperty.anarchyAbuseFlags, 0 } });
            PhotonNetwork.SetModProperties();

            Pause?.Continue();
            Settings.Apply();
            VideoSettings.Apply();
            if (PauseWindow.IsActive)
            {
                PauseWindow.DisableImmediate();
            }
            if (StatsPanel.IsActive)
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

        private void LateUpdate()
        {
            if (InputManager.IsInputAnarchy((int)InputAnarchy.DebugPanel))
            {
                if (DebugPanel.IsActive)
                {
                    DebugPanel.DisableImmediate();
                }
                else
                {
                    DebugPanel.EnableImmediate();
                }
            }
            if (InputManager.IsInputAnarchy((int)InputAnarchy.ChatHistoryPanel))
            {
                if (ChatHistory.IsActive)
                {
                    ChatHistory.DisableImmediate();
                }
                else
                {
                    ChatHistory.EnableImmediate();
                }
            }
            if (IN_GAME_MAIN_CAMERA.GameType == GameType.Single && InputManager.IsInputAnarchyHolding((int)InputAnarchy.StatsPanel))
            {
                InputManager.MenuOn = true;
                Screen.showCursor = true;
                Screen.lockCursor = false;
                StatsPanel.EnableImmediate();
            }

            if (InputManager.IsInputAnarchy((int)InputAnarchy.Rejoin))
            {
                Network.NetworkManager.NeedRejoin = true;
                PhotonNetwork.Disconnect();
            }
        }
    }
}
