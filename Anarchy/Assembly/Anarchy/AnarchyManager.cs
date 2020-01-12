using System;
using Anarchy.Configuration;
using Anarchy.UI;
using System.Collections;
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
        public static Version AnarchyVersion = new Version("0.7.6.1");
        public static readonly string CustomName = string.Empty;
        public static readonly bool CustomVersion = true;
        public static readonly bool FullAnarchySync = false;

        public static Background Background;
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

        private void Awake()
        {
            StartCoroutine(OnGameWasOpened());
            DontDestroyOnLoad(this);
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
        }

        public static void DestroyMainScene()
        {
            string[] objectsToDestroy = new string[] { "PanelLogin", "LOGIN", "BG_TITLE", "Colossal", "Icosphere", "cube", "colossal", "CITY", "city", "rock", "AOTTG_HERO", "Checkbox", };
            GameObject[] gos = (GameObject[])FindObjectsOfType(typeof(GameObject));
            for(int i = 0; i < objectsToDestroy.Length; i++)
            {
                string name = objectsToDestroy[i];
                for(int j = 0; j < gos.Length; j++)
                {
                    if (gos[j].name.Contains(name) || (gos[j].GetComponent<UILabel>() != null && gos[j].GetComponent<UILabel>().text.Contains("Snap")) || (gos[j].GetComponent<UILabel>() != null && gos[j].GetComponent<UILabel>().text.Contains("Custom")))
                    {
                        Destroy(gos[j]);
                    }
                }
            }
        }

        private void OnApplicationQuit()
        {
            User.Save();
            GameModes.Load();
            GameModes.Save();
            Settings.Save();
            Style.Save();
        }

        private void OnLevelWasLoaded(int id)
        {
            if(Application.loadedLevelName == "menu")
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
                Anarchy.Skins.Humans.HumanSkin.Storage.Clear();
            }
            else
            {
                if (Background.Active)
                {
                    Background.Disable();
                }
                if (Application.loadedLevelName != "characterCreation" && Application.loadedLevelName != "SnapShot" && PhotonNetwork.inRoom)
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
            if(Pause != null)
                Pause.Continue();
            Settings.Apply();
            VideoSettings.Apply();
            if (!PauseWindow.Active)
            {
                Time.timeScale = 1f;
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
    }
}
