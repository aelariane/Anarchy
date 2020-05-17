using Anarchy.Configuration;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Anarchy.UI
{
    internal class UIManager : MonoBehaviour
    {
        private static GUIBase[] activeGUIs = new GUIBase[0];
        internal static FloatSetting HUDScaleGUI = new FloatSetting("HUDScaleGUI", 1f);
        internal static BoolSetting HUDAutoScaleGUI = new BoolSetting("HUDAutoScaleGUI", true);
        private static Action onAwakeAdds = delegate () { };
        private static Action onAwakeRms = delegate () { };

        public static UIManager Instance { get; private set; }

        private void Awake()
        {
            if(Instance != null)
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
         public static bool Disable(GUIBase gui)
        {
            if (!gui.Active)
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
                bool wasRemoved = false;
                var list = new List<GUIBase>();
                for (int i = 0; i < activeGUIs.Length; i++)
                {
                    if (activeGUIs[i] == gui)
                    {
                        wasRemoved = true;
                        continue;
                    }
                    list.Add(activeGUIs[i]);
                }
                if (wasRemoved)
                {
                    gui.Disable();
                }
                if (list.Count == 0)
                {
                    activeGUIs = new GUIBase[0];
                    return wasRemoved;
                }
                activeGUIs = list.ToArray();
                UpdateDepths();
                return wasRemoved;
            }
        }

        public static bool Enable(GUIBase gui)
        {
            if (gui.Active)
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
                        return false;
                }
                var copy = new Queue<GUIBase>(activeGUIs);
                var tmp = new Queue<GUIBase>();
                bool added = false;
                for (int i = 0; i < activeGUIs.Length + 1; i++)
                {
                    if (!added && (copy.Count == 0 || gui.Layer < copy.Peek().Layer))
                    {
                        added = true;
                        tmp.Enqueue(gui);
                        continue;
                    }
                    tmp.Enqueue(copy.Dequeue());
                }
                if (added)
                {
                    gui.Drawer.Enable();
                }
                activeGUIs = tmp.ToArray();
                UpdateDepths();
                
                return added;
            }
        }


        private static void UpdateDepths()
        {
            int depth = activeGUIs.Length - 1;
            for(int i = 0; i <activeGUIs.Length; i++)
            {
                activeGUIs[i].Drawer.Depth = depth--;
            }
        }

        public static void SetParent(UIBase ui)
        {
            if(Instance == null)
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

        public static void UpdateGUIScaling()
        {
            Style.UpdateScaling();
            lock (activeGUIs)
            {
                foreach(GUIBase baseg in GUIBase.AllBases)
                {
                    if (baseg.Active)
                    {
                        baseg.DisableImmediate();
                        baseg.EnableImmediate();
                    }
                    baseg.OnUpdateScaling();
                }
            }
        }
    }
}
