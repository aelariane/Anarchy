using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Anarchy.UI
{
    public abstract class GUIPanel : GUIBase
    {
        private static readonly object[] parameters = new object[0];

        private Dictionary<int, MethodInfo> allDisableMethods = new Dictionary<int, MethodInfo>();
        private Dictionary<int, MethodInfo> allEnableMethods = new Dictionary<int, MethodInfo>();
        private Dictionary<int, MethodInfo> allPages = new Dictionary<int, MethodInfo>();
        private MethodInfo currentPage;
        protected string head = string.Empty;
        private int oldPageSelection = -1;
        protected int pageSelection = 0;

        protected Rect BoxPosition { get; private set; }

        public GUIPanel(string name) : this(name, -1)
        {
        }

        public GUIPanel(string name, int layer) : base(name, layer)
        {
            System.Type type = GetType();
            MethodInfo[] allMethods = type.GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (MethodInfo method in allMethods)
            {
                object[] attributes = method.GetCustomAttributes(false);
                if (attributes.Length == 0)
                {
                    continue;
                }

                foreach (object obj in attributes)
                {
                    if (obj is GUIPageAttribute page)
                    {
                        Dictionary<int, MethodInfo> dict = null;
                        if (page.MethodType == GUIPageType.DrawMethod)
                        {
                            dict = allPages;
                        }
                        else if (page.MethodType == GUIPageType.DisableMethod)
                        {
                            dict = allDisableMethods;
                        }
                        else if (page.MethodType == GUIPageType.EnableMethod)
                        {
                            dict = allEnableMethods;
                        }
                        dict.Add(page.Page, method);
                        break;
                    }
                }
            }

        }

        private void CheckPageChange()
        {
            if (oldPageSelection == pageSelection)
            {
                return;
            }
            if (!allPages.ContainsKey(pageSelection))
            {
                currentPage = typeof(GUIPanel).GetMethod(nameof(EmptyPage), BindingFlags.NonPublic | BindingFlags.Instance);
                if (allDisableMethods.ContainsKey(oldPageSelection))
                {
                    allDisableMethods[oldPageSelection].Invoke(this, parameters);
                }

                oldPageSelection = pageSelection;
                return;
            }

            OnBeforePageChanged();
            if (allDisableMethods.ContainsKey(oldPageSelection))
            {
                allDisableMethods[oldPageSelection].Invoke(this, parameters);
            }

            OnAnyPageDisabled();

            if (allEnableMethods.ContainsKey(pageSelection))
            {
                allEnableMethods[pageSelection].Invoke(this, parameters);
            }

            OnAnyPageEnabled();

            currentPage = allPages[pageSelection];
            oldPageSelection = pageSelection;
        }

        protected internal override void Draw()
        {
            GUI.Box(BoxPosition, head);
            DrawMainPart();
            if (currentPage != null)
            {
                currentPage.Invoke(this, parameters);
            }
            CheckPageChange();
        }

        protected abstract void DrawMainPart();

        private void EmptyPage()
        {
        }

        protected virtual void OnAnyPageDisabled()
        {
        }

        protected virtual void OnAnyPageEnabled()
        {
        }

        protected virtual void OnBeforePageChanged()
        {
        }

        protected override void OnDisable()
        {
            currentPage = null;
            head = null;
            OnBeforePageChanged();

            foreach (MethodInfo info in allDisableMethods.Values)
            {
                info.Invoke(this, parameters);
            }
            OnPanelDisable();
        }

        protected override void OnEnable()
        {
            head = locale["title"];
            BoxPosition = Helper.GetScreenMiddle(Style.WindowWidth, Style.WindowHeight);
            pageSelection = 0;
            oldPageSelection = -1;
            OnPanelEnable();
            CheckPageChange();
        }

        protected abstract void OnPanelDisable();

        protected abstract void OnPanelEnable();
    }
}