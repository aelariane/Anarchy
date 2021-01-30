using UnityEngine;

namespace Anarchy.UI
{
    public class GUIDrawer
    {
        private GUIBase owner;
        private GUIDrawerObject drawer;

        public int Depth
        {
            set
            {
                if (drawer == null)
                {
                    return;
                }
                if (drawer.layer != value)
                {
                    drawer.layer = value;
                    drawer.Cancel();
                }
            }
            get
            {
                if (drawer == null)
                {
                    return -1;
                }
                return drawer.layer;
            }
        }

        public GUIDrawer(GUIBase myBase)
        {
            owner = myBase;
        }

        public void Enable()
        {
            if (drawer != null)
            {
                return;
            }
            drawer = new GameObject(owner.Name + "_DrawerObject").AddComponent<GUIDrawerObject>();
            drawer.layer = -1;
            drawer.owner = this;
            drawer.Cancel();
            Object.DontDestroyOnLoad(drawer);
        }

        public void Enable(int currentDepth)
        {
            if (drawer != null)
            {
                return;
            }
            drawer = new GameObject(owner.Name + "_DrawerObject").AddComponent<GUIDrawerObject>();
            drawer.layer = currentDepth;
            drawer.owner = this;
            drawer.Cancel();
            Object.DontDestroyOnLoad(drawer);
        }

        public void Disable()
        {
            if (drawer == null)
            {
                return;
            }
            Object.Destroy(drawer);
            drawer = null;
        }

        private class GUIDrawerObject : MonoBehaviour
        {
            internal GUIDrawer owner;
            internal int layer;
            private bool needCancel = true;

            internal void Cancel()
            {
                needCancel = true;
            }

            private void OnGUI()
            {
                if (needCancel)
                {
                    needCancel = false;
                    return;
                }
                if (owner.owner.OnGUI != null)
                {
                    UnityEngine.GUI.depth = layer;
                    owner.owner.OnGUI();
                }
            }

            private void Update()
            {
                owner.owner.Update();
            }
        }
    }
}