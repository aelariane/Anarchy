using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Anarchy.UI;
using UnityEngine;

namespace Anarchy.UI.Animation
{
    public class DropDownAnimation : GUIAnimation
    {
        private float resultHeight;
        private Rect position;
        private Rect basePosition;
        private GUIStyle boxStyle;

        public DropDownAnimation(GUIBase gUIBase, Rect startPosition, int rowsCount) : this(gUIBase, startPosition, rowsCount, null)
        {
            basePosition = startPosition;
            position = new Rect(startPosition.x, startPosition.y, startPosition.width, startPosition.height);
            resultHeight = (rowsCount * Style.Height) + ((rowsCount + 1) * Style.VerticalMargin);
            boxStyle = new GUIStyle();
            boxStyle.normal.background = Style.Button.normal.background;
        }

        public DropDownAnimation(GUIBase gUIBase, Rect startPosition, int rowsCount, GUIStyle boxStyle) : base(gUIBase)
        {
            basePosition = startPosition;
            position = new Rect(startPosition.x, startPosition.y, startPosition.width, startPosition.height);
            resultHeight = (rowsCount * Style.Height) + ((rowsCount + 1) * Style.VerticalMargin);
            this.boxStyle = boxStyle;
        }

        protected override void OnStartOpen()
        {
            position = new Rect(basePosition.x, basePosition.y, basePosition.width, basePosition.height);
        }

        protected override void OnStartClose()
        {
            position = new Rect(basePosition.x, basePosition.y, basePosition.width, resultHeight);
        }

        protected override bool Close()
        {
            UnityEngine.GUI.Box(position, string.Empty, boxStyle ?? Style.Box);
            position.height -= Time.unscaledDeltaTime * 1350f;
            return basePosition.height < position.height;
        }

        protected override bool Open()
        {
            UnityEngine.GUI.Box(position, string.Empty, boxStyle ?? Style.Box);
            position.height += Time.unscaledDeltaTime * 1000f;
            return resultHeight >= position.height;
        }
    }
}
