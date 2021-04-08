using Anarchy.Configuration;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

namespace Anarchy.UI
{
    public class UIManager : MonoBehaviour
    {
        private static GUIBase[] activeGUIs = new GUIBase[0];
        internal static FloatSetting HUDScaleGUI = new FloatSetting("HUDScaleGUI", 1f);
        internal static BoolSetting HUDAutoScaleGUI = new BoolSetting("HUDAutoScaleGUI", true);
        internal static FloatSetting LabelScale = new FloatSetting("LabelScale", 1f);
        private static Action onAwakeAdds = delegate () { };
        private static Action onAwakeRms = delegate () { };

        public static UIManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError($"There should be only one instance of \"UIManager\". Please make sure yo spawn it just once.");
                DestroyImmediate(this);
                return;
            }
            DontDestroyOnLoad(this);
            Instance = this;
            onAwakeAdds();
            onAwakeRms();
            onAwakeAdds = delegate () { };
            onAwakeRms = delegate () { };
        }


        //private void OnGUI()
        //{
        //    if(Style.CustomStyles != null)
        //    {
        //        UnityEngine.GUI.skin.customStyles = Style.CustomStyles;
        //    }
        //}


        public static bool Disable(GUIBase gui)
        {
            if (!gui.IsActive)
            {
                return false;
            }
            if (Instance == null)
            {
                onAwakeRms += delegate ()
                {
                    Disable(gui);
                };
                return false;
            }
            lock (activeGUIs)
            {
                bool wasLast = activeGUIs
                    .OrderBy(g => g.Layer)
                    .LastOrDefault() == gui;

                int oldCount = activeGUIs.Length;

                activeGUIs = activeGUIs
                    .Where(x => x != gui)
                    .ToArray();

                bool wasRemoved = oldCount != activeGUIs.Length;

                if (wasRemoved)
                {
                    gui.Disable();
                }
                if (activeGUIs.Length == 0)
                {
                    activeGUIs = new GUIBase[0];
                    return wasRemoved;
                }
                if (!wasLast)
                {
                    UpdateDepths();
                }
                return wasRemoved;
            }
        }

        public static bool Enable(GUIBase gui)
        {
            if (gui.IsActive)
            {
                return false;
            }
            if (Instance == null)
            {
                onAwakeAdds += delegate ()
                {
                    Enable(gui);
                };
                return false;
            }
            lock (activeGUIs)
            {
                if (activeGUIs == null || activeGUIs.Length == 0)
                {
                    activeGUIs = new GUIBase[] { gui };
                    gui.Drawer.Enable();
                    UpdateDepths();
                    return true;
                }

                for (int i = 0; i < activeGUIs.Length; i++)
                {
                    if (activeGUIs[i] == gui)
                    {
                        return false;
                    }
                }

                var list = activeGUIs.ToList();
                list.Add(gui);
                activeGUIs = list
                    .OrderBy(x => x.Layer)
                    .ToArray();

                if(activeGUIs.Last() == gui)
                {
                    if(activeGUIs.Length >= 2)
                    {
                        gui.Drawer.Enable(activeGUIs[activeGUIs.Length - 2].Drawer.Depth - 1);
                    }
                    else
                    {
                        gui.Drawer.Enable();
                        UpdateDepths();
                    }
                }
                else
                {
                    gui.Drawer.Enable();
                    UpdateDepths();
                }

                return true;
            }
        }

        private static void UpdateDepths()
        {
            int depth = activeGUIs.Length + 10;
            for (int i = 0; i < activeGUIs.Length; i++)
            {
                activeGUIs[i].Drawer.Depth = depth--;
            }
        }

        public static void SetParent(UIBase ui)
        {
            if (Instance == null)
            {
                onAwakeAdds += delegate ()
                {
                    ui.transform.SetParent(Instance.transform);
                    ui.transform.position = new Vector3(Instance.transform.position.x, Instance.transform.position.y, Instance.transform.position.z);
                };
                return;
            }
            if (!Instance.gameObject.activeInHierarchy)
            {
                Instance.gameObject.SetActive(true);
            }
            ui.transform.SetParent(Instance.transform);
            ui.transform.position = new Vector3(Instance.transform.position.x, Instance.transform.position.y, Instance.transform.position.z);
        }

        private static System.Collections.IEnumerator WaitAndEnable(GUIBase baseG)
        {
            yield return new WaitForEndOfFrame();
            baseG.DisableImmediate();
            baseG.OnUpdateScaling();
            yield return new WaitForEndOfFrame();
            baseG.EnableImmediate();
        }

        public static void UpdateGUIScaling()
        {
            Style.UpdateScaling();
            lock (activeGUIs)
            {
                foreach (GUIBase baseg in GUIBase.AllBases)
                {
                    if (baseg.IsActive && baseg.Name != nameof(PauseWindow))
                    {
                        //baseg.DisableImmediate();
                        //baseg.OnUpdateScaling();
                        //baseg.EnableImmediate();
                        Instance.StartCoroutine(WaitAndEnable(baseg));
                        continue;
                    }
                    baseg.OnUpdateScaling();
                }
                //UpdateDepths();
            }
        }
    }
}