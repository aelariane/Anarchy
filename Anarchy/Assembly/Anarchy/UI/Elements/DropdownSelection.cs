using Anarchy.Configuration;
using UnityEngine;

namespace Anarchy.UI.Elements
{
    public class DropdownSelection : GUIBase
    {
        private GUIStyle activeButtonStyle;
        private Rect boxPosition;
        private GUIStyle boxStyle;
        private Rect cursorLimits;
        private GUIBase guiOwner;
        private int initialSelection;
        private SmartRect rect;
        private Setting<int> refSet;
        private string[] selections;

        private DropdownSelection() : base(nameof(DropdownSelection), GUILayers.DropdownBox)
        {
            boxStyle = new GUIStyle();
            boxStyle.normal.background = Style.Button.normal.background;
            activeButtonStyle = new GUIStyle(Style.Button);
            activeButtonStyle.normal = Style.SelectionGrid.onNormal;
            activeButtonStyle.hover = Style.SelectionGrid.onHover;
            activeButtonStyle.active = Style.SelectionGrid.onActive;
        }

        protected internal override void Draw()
        {
            var pos = new Vector2(Event.current.mousePosition.x, Event.current.mousePosition.y);
            if (!cursorLimits.Contains(pos) || (guiOwner != null && guiOwner.IsActive == false))
            {
                Disable();
                return;
            }

            UnityEngine.GUI.Box(boxPosition, string.Empty, boxStyle);
            rect.Reset();
            bool wasPressed = false;
            for (int i = 0; i < selections.Length; i++)
            {
                if (UnityEngine.GUI.Button(rect, selections[i], i == refSet.Value ? activeButtonStyle : Style.Button))
                {
                    refSet.Value = i;
                    wasPressed = true;
                }
                rect.MoveY();
            }

            if (wasPressed)
            {
                Disable();
                return;
            }
        }

        public static DropdownSelection CreateNew(GUIBase baseGUI, Rect position, string[] selections, Setting<int> referenceSetting)
        {
            var element = new DropdownSelection();
            element.guiOwner = baseGUI;

            element.cursorLimits = new Rect(
                position.x - new AutoScaleFloat(10f),
                position.y - new AutoScaleFloat(10f),
                position.width + new AutoScaleFloat(20f),
                position.height * selections.Length + (Style.VerticalMargin * (selections.Length - 1)) + (2 * selections.Length) + new AutoScaleFloat(20f)
            );

            element.refSet = referenceSetting;
            element.boxPosition = new Rect(
                position.x,
                position.y,
                position.width,
                position.height * selections.Length + (Style.VerticalMargin * (selections.Length - 1)) + (2 * selections.Length)
            );

            element.rect = new SmartRect(position);
            element.animator = new Animation.DropDownAnimation(element, position, selections.Length);
            element.Enable();
            element.initialSelection = referenceSetting.Value;
            element.selections = selections;
            return element;
        }

        public static DropdownSelection CreateNew(Rect position, string[] selections, Setting<int> referenceSetting)
        {
            var element = new DropdownSelection();

            element.cursorLimits = new Rect(
                position.x - new AutoScaleFloat(10f),
                position.y - new AutoScaleFloat(10f),
                position.width + new AutoScaleFloat(20f),
                position.height * selections.Length + Style.VerticalMargin * (selections.Length + 1) + new AutoScaleFloat(20f)
            );

            element.refSet = referenceSetting;
            element.boxPosition = new Rect(
                position.x,
                position.y,
                position.width,
                position.height * selections.Length + Style.VerticalMargin * (selections.Length - 1)
            );

            element.rect = new SmartRect(position);
            element.animator = new Animation.DropDownAnimation(element, position, selections.Length);
            element.Enable();
            element.initialSelection = referenceSetting.Value;
            element.selections = selections;
            return element;
        }
    }
}