using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Anarchy.UI
{
    /// <summary>
    /// Base class for GUI panels
    /// </summary>
    /// <remarks>
    /// Basically, this base helps to unify all panels in game, make them have same style etc.
    /// Animation set to <seealso cref="Animation.CenterAnimation"/> be default
    /// Must have title key in <seealso cref="GUIBase.locale"/>
    /// </remarks>
    public abstract class GUIPanel : GUIBase
    {
        private static readonly object[] parameters = new object[0];

        private Dictionary<int, MethodInfo> allDisableMethods = new Dictionary<int, MethodInfo>();
        private Dictionary<int, MethodInfo> allEnableMethods = new Dictionary<int, MethodInfo>();
        private Dictionary<int, MethodInfo> allPages = new Dictionary<int, MethodInfo>();
        private MethodInfo currentPage;
        protected string head = string.Empty;
        private int oldPageSelection = -1;

        /// <summary>
        /// Current page. Change this to switch between pages
        /// </summary>
        protected int pageSelection = 0;

        /// <summary>
        /// Rect of panel's window
        /// </summary>
        protected Rect WindowPosition { get; private set; }

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
            animator = new Animation.CenterAnimation(this, Helper.GetScreenMiddle(Style.WindowWidth, Style.WindowHeight));
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
            GUI.Box(WindowPosition, head);
            DrawMainPart();
            if (currentPage != null)
            {
                currentPage.Invoke(this, parameters);
            }
            CheckPageChange();
        }

        /// <summary>
        /// Draws main part of panel
        /// </summary>
        protected abstract void DrawMainPart();

        private void EmptyPage()
        {
        }

        /// <summary>
        /// Calls when any of pages was disabled
        /// </summary>
        protected virtual void OnAnyPageDisabled()
        {
        }

        /// <summary>
        /// Calls when any of pages was enabled
        /// </summary>
        protected virtual void OnAnyPageEnabled()
        {
        }

        /// <summary>
        /// Calls just before any page will be changed
        /// </summary>
        /// <remarks>Calls before <seealso cref="OnAnyPageDisabled"/> and <seealso cref="OnAnyPageEnabled"/></remarks>
        protected virtual void OnBeforePageChanged()
        {
        }

        /// <summary>Avoid overriding this on <see cref="GUIPanel"/> unless you're not sure what you do</summary>
        /// <remarks>
        /// Override <seealso cref="OnPanelDisable"/> for panels
        /// </remarks>
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

        /// <summary>Avoid overriding this on <see cref="GUIPanel"/> unless you're not sure what you do</summary>
        /// <remarks>
        /// Override <seealso cref="OnPanelEnable"/> for panels
        /// </remarks>
        protected override void OnEnable()
        {
            head = locale["title"];
            WindowPosition = Helper.GetScreenMiddle(Style.WindowWidth, Style.WindowHeight);
            pageSelection = 0;
            oldPageSelection = -1;
            OnPanelEnable();
            CheckPageChange();
        }

        public override void OnUpdateScaling()
        {
            animator = new Animation.CenterAnimation(this, Helper.GetScreenMiddle(Style.WindowWidth, Style.WindowHeight));
        }

        /// <summary>
        /// Calls when panel is disabled
        /// </summary>
        /// <remarks>Override this instead of <see cref="OnDisable"/></remarks>
        protected abstract void OnPanelDisable();

        /// <summary>
        /// Calls when panel was enabled
        /// </summary>
        /// <remarks>Override this instead of <see cref="OnEnable"/></remarks>
        protected abstract void OnPanelEnable();
    }
}