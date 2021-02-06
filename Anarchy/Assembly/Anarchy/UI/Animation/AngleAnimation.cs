using UnityEngine;

namespace Anarchy.UI.Animation
{
    /// <summary>
    /// Animation thant makes window appear from one of the 4 angles of screen
    /// </summary>
    public class AngleAnimation : GUIAnimation
    {
        private float deltaHeight;
        private float deltaWidth;
        private float deltaX;
        private float deltaY;
        private Rect endPoint;
        private int frames;
        private int nextFramesCount;
        private Rect position;
        private readonly Rect resultPositiion;
        private readonly Rect startPosition;

        public float Speed { get; set; } = 1f;

        public AngleAnimation(GUIBase @base, StartPoint startPoint, Rect finishRect) : this(@base, StartPointToRect(startPoint), finishRect)
        {
        }

        private AngleAnimation(GUIBase @base, Rect startRect, Rect finishRect) : base(@base)
        {
            startPosition = startRect;
            resultPositiion = finishRect;
        }

        private void CalctulateDeltas()
        {
            nextFramesCount = FengGameManagerMKII.FPS.FPS;
            deltaX = (endPoint.x - position.x) / nextFramesCount;
            deltaY = (endPoint.y - position.y) / nextFramesCount;
            deltaWidth = (endPoint.width - position.width) / nextFramesCount;
            deltaHeight = (endPoint.height - position.height) / nextFramesCount;
            nextFramesCount = Mathf.RoundToInt(nextFramesCount * (1f / Speed));
            deltaX *= Speed;
            deltaY *= Speed;
            deltaWidth *= Speed;
            deltaHeight *= Speed;
        }

        protected override bool Close()
        {
            return Draw();
        }

        private bool Draw()
        {
            GUI.Box(position, string.Empty);
            position.x += deltaX;
            position.y += deltaY;
            position.width += deltaWidth;
            position.height += deltaHeight;
            return frames++ < nextFramesCount;
        }

        protected override void OnStartClose()
        {
            position = resultPositiion;
            endPoint = startPosition;
            CalctulateDeltas();
            frames = 0;
        }

        protected override void OnStartOpen()
        {
            position = startPosition;
            endPoint = resultPositiion;
            CalctulateDeltas();
            frames = 0;
        }

        protected override bool Open()
        {
            return Draw();
        }

        public static Rect StartPointToRect(StartPoint point)
        {
            switch (point)
            {
                default:
                case StartPoint.TopRight:
                    return new Rect(0f, 0f, 0f, 0f);

                case StartPoint.TopLeft:
                    return new Rect(Style.ScreenWidth, 0f, 0f, 0f);

                case StartPoint.BottomRight:
                    return new Rect(0f, Style.ScreenHeight, 0f, 0f);

                case StartPoint.BottomLeft:
                    return new Rect(Style.ScreenWidth, Style.ScreenHeight, 0f, 0f);
            }
        }

        public enum StartPoint
        {
            TopRight,
            TopLeft,
            BottomRight,
            BottomLeft
        }
    }
}