using UnityEngine;

namespace Anarchy.UI
{
    public class SmartRect
    {
        private const float DefaultOffsetX = 20;
        private const float DefaultOffsetY = 5f;

        public readonly float DefaultHeight;
        public readonly float DefaultWidth;
        public readonly float DefaultX;
        public readonly float DefaultY;

        private float moveX;
        private float moveY;
        private float offsetX;
        private float offsetY;
        private Rect source;

        public float height
        {
            get
            {
                return source.height;
            }
            set
            {
                source.height = value;
                moveY = value + offsetY;
            }
        }

        public float width
        {
            get
            {
                return source.width;
            }
            set
            {
                source.width = value;
                moveX = value + offsetX;
            }
        }

        public float x
        {
            get
            {
                return source.x;
            }
            set
            {
                source.x = value;
            }
        }

        public float y
        {
            get
            {
                return source.y;
            }
            set
            {
                source.y = value;
            }
        }

        public SmartRect(Rect src) : this(src, DefaultOffsetX, DefaultOffsetY)
        {
        }

        public SmartRect(Rect src, float offX, float offY)
        {
            source = new Rect(src.x, src.y, src.width, src.height);
            offsetX = offX;
            offsetY = offY;
            moveX = source.width + offsetX;
            moveY = source.height + offsetY;
            DefaultHeight = src.height;
            DefaultWidth = src.width;
            DefaultX = src.x;
            DefaultY = src.y;
        }

        public SmartRect(float x, float y, float width, float height) : this(new Rect(x, y, width, height))
        {
        }

        public SmartRect(float x, float y, float width, float height, float offX, float offY) : this(new Rect(x, y, width, height), offX, offY)
        {
        }

        public void BeginHorizontal(int elementCount)
        {
            width = (width - (offsetX * (elementCount - 1))) / elementCount;
        }

        public void Move(Vector2 vec)
        {
            source.x += vec.x;
            source.y += vec.y;
        }

        public void MoveOffsetX(float off)
        {
            source.x += off;
            source.width -= off;
        }

        public void MoveOffsetY(float off)
        {
            source.y += off;
            source.height -= off;
        }

        public void MoveToEndX(Rect box, float width)
        {
            source.x += ((((box.x + Style.WindowSideOffset) + box.width - (Style.WindowSideOffset * 2)) - source.x) - width);
        }

        public void MoveToEndY(Rect box, float height)
        {
            source.y += ((((box.y + Style.WindowTopOffset) + box.height - (Style.WindowTopOffset + Style.WindowBottomOffset)) - source.y) - height);
        }

        public void MoveX()
        {
            source.x += moveX;
        }

        public void MoveX(float off, bool wid = false)
        {
            source.x += off;
            if (wid)
            {
                source.x += source.width;
            }
        }

        public void MoveY()
        {
            source.y += moveY;
        }

        public void MoveY(float off, bool hei = false)
        {
            source.y += off;
            if (hei)
            {
                source.y += source.height;
            }
        }

        public void Reset()
        {
            source.x = DefaultX;
            source.y = DefaultY;
            height = DefaultHeight;
            width = DefaultWidth;
        }

        public void ResetX(bool includeWidth = true)
        {
            source.x = DefaultX;
            if (includeWidth)
            {
                source.width = DefaultWidth;
            }
        }

        public void ResetY(bool includeHeight = false)
        {
            source.y = DefaultY;
            if (includeHeight)
            {
                source.height = DefaultHeight;
            }
        }

        public Rect ToRect()
        {
            return source;
        }

        public static implicit operator Rect(SmartRect r)
        {
            return r.source;
        }
    }
}