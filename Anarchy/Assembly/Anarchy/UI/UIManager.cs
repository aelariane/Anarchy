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
            CheckIfNeedDisable();
        }

        internal static void CheckIfNeedDisable()
        {
            lock (activeGUIs)
            {
                if (activeGUIs.Length > 0 || Instance.transform.childCount > 0)
                {
                    return;
                }
                if (Instance.gameObject.activeInHierarchy)
                    Instance.gameObject.SetActive(false);
            }
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
                if (list.Count == 0)
                {
                    activeGUIs = new GUIBase[0];
                    CheckIfNeedDisable();
                    return wasRemoved;
                }
                activeGUIs = list.ToArray();
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
                    if (!Instance.gameObject.activeInHierarchy)
                    {
                        Instance.gameObject.SetActive(true);
                    }
                    activeGUIs = new GUIBase[] { gui };
                    return true;
                }
                for(int i = 0; i < activeGUIs.Length; i++)
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
                activeGUIs = tmp.ToArray();
                return added;
            }
        }

        private void OnGUI()
        {
            lock (activeGUIs)
            {
                for (int i = 0; i < activeGUIs.Length; i++)
                {
                    activeGUIs[i].OnGUI();
                }
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

        private void Update()
        {
            lock (activeGUIs)
            {
                for (int i = 0; i < activeGUIs.Length; i++)
                {
                    activeGUIs[i].Update();
                }
            }
        }

        public static void UpdateGUIScaling()
        {
            Style.UpdateScaling();
            lock (activeGUIs)
            {
                foreach(GUIBase baseg in GUIBase.AllBases)
                {
                    baseg.OnUpdateScaling();
                }
            }
        }
    }
}
