using Anarchy.Configuration;
using System;
using System.Collections.Generic;
using UnityEngine;
using UGUI = UnityEngine.GUILayout;

namespace Anarchy.UI
{
    /// <summary>
    /// Styled wrapper class for <see cref="UnityEngine.GUILayout"/>
    /// </summary>
    /// <remarks>Use this class to draw Anarchy-styled unified GUI</remarks>
    public static class GUILayout
    {
        private static Dictionary<Setting<float>, string> setFloats = new Dictionary<Setting<float>, string>();
        private static Dictionary<Setting<int>, string> setIntegers = new Dictionary<Setting<int>, string>();

        private static readonly GUILayoutOption[] DefaultOption = new GUILayoutOption[1] { UGUI.Height(15f) };

        #region Areas

        public static void BeginArea(Rect rect)
        {
            UGUI.BeginArea(rect);
        }

        public static void BeginArea(Rect rect, string text)
        {
            UGUI.BeginArea(rect, text);
        }

        public static void BeginArea(SmartRect rect)
        {
            UGUI.BeginArea(rect.ToRect());
        }

        public static void BeginArea(SmartRect rect, string text)
        {
            UGUI.BeginArea(rect.ToRect(), text);
        }

        public static void BeginHorizontal()
        {
            UGUI.BeginHorizontal();
        }

        public static void BeginHorizontal(GUILayoutOption[] opts)
        {
            UGUI.BeginHorizontal(opts);
        }

        public static Vector2 BeginScrollView(Vector2 curr)
        {
            return UGUI.BeginScrollView(curr, Style.ScrollView, Style.ScrollView);
        }

        public static void BeginVertical()
        {
            UGUI.BeginHorizontal();
        }

        public static void BeginVertical(GUILayoutOption[] opts)
        {
            UGUI.BeginHorizontal(opts);
        }

        #endregion Areas

        #region Box

        public static void Box(string text)
        {
            UGUI.Box(text, Style.Box);
        }

        public static void Box(Texture img)
        {
            UGUI.Box(img);
        }

        public static void Box(string text, GUILayoutOption[] opts)
        {
            UGUI.Box(text, Style.Box, opts);
        }

        public static void Box(Texture img, GUILayoutOption[] opts)
        {
            UGUI.Box(img, opts);
        }

        #endregion Box

        #region Button

        public static bool Button(string text)
        {
            return UGUI.Button(text, Style.Button, DefaultOption);
        }

        public static bool Button(string text, GUILayoutOption[] opts)
        {
            return UGUI.Button(text, Style.Button, opts);
        }

        #endregion Button

        #region EndAreas

        public static void EndArea()
        {
            UGUI.EndArea();
        }

        public static void EndHorizontal()
        {
            UGUI.EndHorizontal();
        }

        public static void EndScrollView()
        {
            UGUI.EndScrollView();
        }

        public static void EndVertical()
        {
            UGUI.EndVertical();
        }

        #endregion EndAreas

        #region HorizontalSlider

        public static float HorizontalSlider(float val)
        {
            return UGUI.HorizontalSlider(val, 0f, 1f, Style.Slider, Style.SliderBody, DefaultOption);
        }

        public static float HorizontalSlider(float val, GUILayoutOption[] opts)
        {
            return UGUI.HorizontalSlider(val, 0f, 1f, Style.Slider, Style.SliderBody, opts);
        }

        public static float HorizontalSlider(float val, float min, float max)
        {
            return UGUI.HorizontalSlider(val, min, max, Style.Slider, Style.SliderBody, DefaultOption);
        }

        internal static GUILayoutOption Width(float width)
        {
            throw new NotImplementedException();
        }

        public static float HorizontalSlider(float val, float min, float max, GUILayoutOption[] opts)
        {
            return UGUI.HorizontalSlider(val, min, max, Style.Slider, Style.SliderBody, opts);
        }

        public static float HorizontalSlider(float val, string label)
        {
            UGUI.BeginHorizontal();
            Label(label);
            UGUI.Space(5f);
            val = UGUI.HorizontalSlider(val, 0f, 1f, Style.Slider, Style.SliderBody, DefaultOption);
            UGUI.EndHorizontal();
            return val;
        }

        public static float HorizontalSlider(float val, string label, GUILayoutOption[] lblOpts, GUILayoutOption[] sliderOpts)
        {
            UGUI.BeginHorizontal();
            UGUI.Label(label, Style.Label, lblOpts);
            UGUI.Space(5f);
            val = UGUI.HorizontalSlider(val, 0f, 1f, Style.Slider, Style.SliderBody, sliderOpts);
            UGUI.EndHorizontal();
            return val;
        }

        public static float HorizontalSlider(float val, string label, float min, float max)
        {
            UGUI.BeginHorizontal();
            Label(label);
            UGUI.Space(5f);
            val = UGUI.HorizontalSlider(val, min, max, Style.Slider, Style.SliderBody, DefaultOption);
            UGUI.EndHorizontal();
            return val;
        }

        public static float HorizontalSlider(float val, string label, float min, float max, GUILayoutOption[] lblOpts, GUILayoutOption[] sliderOpts)
        {
            UGUI.BeginHorizontal();
            UGUI.Label(label, Style.Label, lblOpts);
            UGUI.Space(5f);
            val = UGUI.HorizontalSlider(val, min, max, Style.Slider, Style.SliderBody, sliderOpts);
            UGUI.EndHorizontal();
            return val;
        }

        public static void HorizontalSlider(Setting<float> val)
        {
            val.Value = UGUI.HorizontalSlider(val.Value, 0f, 1f, Style.Slider, Style.SliderBody, DefaultOption);
        }

        public static void HorizontalSlider(Setting<float> val, GUILayoutOption[] opts)
        {
            val.Value = UGUI.HorizontalSlider(val.Value, 0f, 1f, Style.Slider, Style.SliderBody, opts);
        }

        public static void HorizontalSlider(Setting<float> val, float min, float max)
        {
            val.Value = UGUI.HorizontalSlider(val.Value, min, max, Style.Slider, Style.SliderBody, DefaultOption);
        }

        public static void HorizontalSlider(Setting<float> val, float min, float max, GUILayoutOption[] opts)
        {
            val.Value = UGUI.HorizontalSlider(val.Value, min, max, Style.Slider, Style.SliderBody, opts);
        }

        public static void HorizontalSlider(Setting<float> val, string label)
        {
            UGUI.BeginHorizontal();
            UGUI.Label(label, Style.Label, DefaultOption);
            UGUI.Space(5f);
            val.Value = UGUI.HorizontalSlider(val.Value, 0f, 1f, Style.Slider, Style.SliderBody, DefaultOption);
            UGUI.EndHorizontal();
        }

        public static void HorizontalSlider(Setting<float> val, string label, GUILayoutOption[] lblOpts, GUILayoutOption[] sliderOpts)
        {
            UGUI.BeginHorizontal();
            UGUI.Label(label, Style.Label, lblOpts);
            UGUI.Space(5f);
            val.Value = UGUI.HorizontalSlider(val.Value, 0f, 1f, Style.Slider, Style.SliderBody, sliderOpts);
            UGUI.EndHorizontal();
        }

        public static void HorizontalSlider(Setting<float> val, string label, float min, float max)
        {
            UGUI.BeginHorizontal();
            UGUI.Label(label, Style.Label, DefaultOption);
            UGUI.Space(5f);
            val.Value = UGUI.HorizontalSlider(val.Value, min, max, Style.Slider, Style.SliderBody, DefaultOption);
            UGUI.EndHorizontal();
        }

        public static void HorizontalSlider(Setting<float> val, string label, float min, float max, GUILayoutOption[] lblOpts, GUILayoutOption[] sliderOpts)
        {
            UGUI.BeginHorizontal();
            UGUI.Label(label, Style.Label, lblOpts);
            UGUI.Space(5f);
            val.Value = UGUI.HorizontalSlider(val.Value, min, max, Style.Slider, Style.SliderBody, sliderOpts);
            UGUI.EndHorizontal();
        }

        #endregion HorizontalSlider

        #region Label

        public static void Label(string content)
        {
            UGUI.Label(content, Style.Label, DefaultOption);
        }

        public static void Label(string content, GUILayoutOption[] opts)
        {
            UGUI.Label(content, Style.Label, opts);
        }

        public static void LabelCenter(string content)
        {
            UGUI.Label(content, Style.LabelCenter, DefaultOption);
        }

        public static void LabelCenter(string content, GUILayoutOption[] opts)
        {
            UGUI.Label(content, Style.LabelCenter, opts);
        }

        #endregion Label

        #region SelectionGrid

        public static int SelectionGrid(int value, string[] labels, int xCount)
        {
            return UGUI.SelectionGrid(value, labels, xCount, Style.SelectionGrid, DefaultOption);
        }

        public static int SelectionGrid(int value, string[] labels, int xCount, GUILayoutOption[] opts)
        {
            return UGUI.SelectionGrid(value, labels, xCount, Style.SelectionGrid, opts);
        }

        public static void SelectionGrid(Setting<int> set, string[] labels, int xCount)
        {
            set.Value = UGUI.SelectionGrid(set.Value, labels, xCount, Style.SelectionGrid, DefaultOption);
        }

        public static void SelectionGrid(Setting<int> set, string[] labels, int xCount, GUILayoutOption[] opts)
        {
            set.Value = UGUI.SelectionGrid(set.Value, labels, xCount, Style.SelectionGrid, opts);
        }

        #endregion SelectionGrid

        #region Space

        public static void Space()
        {
            UGUI.Space(5f);
        }

        public static void Space(float offset)
        {
            UGUI.Space(offset);
        }

        #endregion Space

        #region TextField

        public static string TextField(string val)
        {
            return UGUI.TextField(val, Style.TextField, DefaultOption);
        }

        public static string TextField(string val, GUILayoutOption[] opts)
        {
            return UGUI.TextField(val, Style.TextField, opts);
        }

        public static string TextField(string val, string label)
        {
            UGUI.BeginHorizontal();
            UGUI.Label(label, Style.Label, DefaultOption);
            UGUI.Space(5f);
            val = UGUI.TextField(val, Style.TextButton, DefaultOption);
            UGUI.EndHorizontal();
            return val;
        }

        public static string TextField(string val, string label, GUILayoutOption[] lblOpts, GUILayoutOption[] txtopts)
        {
            UGUI.BeginHorizontal();
            UGUI.Label(label, Style.Label, lblOpts);
            UGUI.Space(5f);
            val = UGUI.TextField(val, Style.TextButton, txtopts);
            UGUI.EndHorizontal();
            return val;
        }

        public static void TextField(Setting<string> val)
        {
            val.Value = UGUI.TextField(val.Value, Style.TextField, DefaultOption);
        }

        public static void TextField(Setting<string> val, GUILayoutOption[] opts)
        {
            val.Value = UGUI.TextField(val.Value, Style.TextField, opts);
        }

        public static void TextField(Setting<string> val, string label)
        {
            UGUI.BeginHorizontal();
            UGUI.Label(label, Style.Label, DefaultOption);
            UGUI.Space(5f);
            val.Value = UGUI.TextField(val.Value, Style.TextButton, DefaultOption);
            UGUI.EndHorizontal();
        }

        public static void TextField(Setting<string> val, string label, GUILayoutOption[] lblOpts, GUILayoutOption[] txtopts)
        {
            UGUI.BeginHorizontal();
            UGUI.Label(label, Style.Label, lblOpts);
            UGUI.Space(5f);
            val.Value = UGUI.TextField(val.Value, Style.TextButton, txtopts);
            UGUI.EndHorizontal();
        }

        public static void TextField(Setting<float> val)
        {
            if (!setFloats.ContainsKey(val))
            {
                setFloats.Add(val, val.Value.ToString());
            }

            string text = setFloats[val];
            text = UGUI.TextField(text, Style.TextField, DefaultOption);
            float.TryParse(text, out val.Value);
            setFloats[val] = text;
        }

        public static void TextField(Setting<float> val, GUILayoutOption[] opts)
        {
            if (!setFloats.ContainsKey(val))
            {
                setFloats.Add(val, val.Value.ToString());
            }

            string text = setFloats[val];
            text = UGUI.TextField(text, Style.TextField, opts);
            float.TryParse(text, out val.Value);
            setFloats[val] = text;
        }

        public static void TextField(Setting<float> val, string label)
        {
            UGUI.BeginHorizontal();
            UGUI.Label(label, Style.Label, DefaultOption);
            UGUI.Space(5f);
            TextField(val);
            UGUI.EndHorizontal();
        }

        public static void TextField(Setting<float> val, string label, GUILayoutOption[] lblOpts, GUILayoutOption[] txtopts)
        {
            UGUI.BeginHorizontal();
            UGUI.Label(label, Style.Label, lblOpts);
            UGUI.Space(5f);
            TextField(val, txtopts);
            UGUI.EndHorizontal();
        }

        public static void TextField(Setting<int> val)
        {
            if (!setIntegers.ContainsKey(val))
            {
                setIntegers.Add(val, val.Value.ToString());
            }

            string text = setIntegers[val];
            text = UGUI.TextField(text, Style.TextField, DefaultOption);
            int.TryParse(text, out val.Value);
            setIntegers[val] = text;
        }

        public static void TextField(Setting<int> val, GUILayoutOption[] opts)
        {
            if (!setIntegers.ContainsKey(val))
            {
                setIntegers.Add(val, val.Value.ToString());
            }

            string text = setIntegers[val];
            text = UGUI.TextField(text, Style.TextField, opts);
            int.TryParse(text, out val.Value);
            setIntegers[val] = text;
        }

        public static void TextField(Setting<int> val, string label)
        {
            UGUI.BeginHorizontal();
            UGUI.Label(label, Style.Label, DefaultOption);
            UGUI.Space(5f);
            TextField(val);
            UGUI.EndHorizontal();
        }

        public static void TextField(Setting<int> val, string label, GUILayoutOption[] lblOpts, GUILayoutOption[] txtopts)
        {
            UGUI.BeginHorizontal();
            UGUI.Label(label, Style.Label, lblOpts);
            UGUI.Space(5f);
            TextField(val, txtopts);
            UGUI.EndHorizontal();
        }

        #endregion TextField

        #region Toggle

        public static bool Toggle(bool val)
        {
            return UGUI.Toggle(val, string.Empty, Style.Toggle, DefaultOption);
        }

        public static bool Toggle(bool val, GUILayoutOption[] opts)
        {
            return UGUI.Toggle(val, string.Empty, Style.Toggle, opts);
        }

        public static bool Toggle(bool val, string label)
        {
            UGUI.BeginHorizontal();
            Label(label);
            UGUI.FlexibleSpace();
            val = UGUI.Toggle(val, string.Empty, Style.Toggle);
            UGUI.EndHorizontal();
            return val;
        }

        public static bool Toggle(bool val, string label, GUILayoutOption[] labelOpts, GUILayoutOption[] tglOpts)
        {
            UGUI.BeginHorizontal();
            UGUI.Label(label, Style.Label, labelOpts);
            UGUI.FlexibleSpace();
            val = UGUI.Toggle(val, string.Empty, Style.Toggle, tglOpts);
            UGUI.EndHorizontal();
            return val;
        }

        public static void Toggle(Setting<bool> val)
        {
            val.Value = UGUI.Toggle(val.Value, string.Empty, Style.Toggle, DefaultOption);
        }

        public static void Toggle(Setting<bool> val, GUILayoutOption[] opts)
        {
            val.Value = UGUI.Toggle(val.Value, string.Empty, Style.Toggle, opts);
        }

        public static void Toggle(Setting<bool> val, string label)
        {
            UGUI.BeginHorizontal();
            Label(label);
            UGUI.FlexibleSpace();
            val.Value = UGUI.Toggle(val.Value, string.Empty, Style.Toggle);
            UGUI.EndHorizontal();
        }

        public static void Toggle(Setting<bool> val, string label, GUILayoutOption[] labelOpts, GUILayoutOption[] tglOpts)
        {
            UGUI.BeginHorizontal();
            UGUI.Label(label, Style.Label, labelOpts);
            UGUI.FlexibleSpace();
            val.Value = UGUI.Toggle(val.Value, string.Empty, Style.Toggle, tglOpts);
            UGUI.EndHorizontal();
        }

        #endregion Toggle

        #region ToggleButton

        public static bool ToggleButton(bool val, string label)
        {
            UGUI.BeginHorizontal();
            UGUI.Label(label, Style.Label, DefaultOption);
            UGUI.Space(5f);
            val = UGUI.Button(val ? GUI.LabelEnabled : GUI.LabelDisabled, Style.TextButton, DefaultOption);
            UGUI.EndHorizontal();
            return val;
        }

        public static bool ToggleButton(bool val, string label, GUILayoutOption[] lblOpts, GUILayoutOption[] btnOpts)
        {
            UGUI.BeginHorizontal();
            UGUI.Label(label, Style.Label, lblOpts);
            UGUI.Space(5f);
            val = UGUI.Button(val ? GUI.LabelEnabled : GUI.LabelDisabled, Style.TextButton, btnOpts);
            UGUI.EndHorizontal();
            return val;
        }

        public static void ToggleButton(Setting<bool> val, string label)
        {
            UGUI.BeginHorizontal();
            UGUI.Label(label, Style.Label, DefaultOption);
            UGUI.Space(5f);
            val.Value = UGUI.Button(val.Value ? GUI.LabelEnabled : GUI.LabelDisabled, Style.TextButton, DefaultOption);
            UGUI.EndHorizontal();
        }

        public static void ToggleButton(Setting<bool> val, string label, GUILayoutOption[] lblOpts, GUILayoutOption[] btnOpts)
        {
            UGUI.BeginHorizontal();
            UGUI.Label(label, Style.Label, lblOpts);
            UGUI.Space(5f);
            val.Value = UGUI.Button(val.Value ? GUI.LabelEnabled : GUI.LabelDisabled, Style.TextButton, btnOpts);
            UGUI.EndHorizontal();
        }

        #endregion ToggleButton
    }
}