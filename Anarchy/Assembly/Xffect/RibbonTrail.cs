using Optimization.Caching;
using UnityEngine;

public class RibbonTrail
{
    protected Color Color = Color.white;
    protected float ElapsedTime;
    protected float ElemLength;
    protected float Fps;
    protected Vector3 HeadPosition;
    protected bool IndexDirty;
    protected Vector2 LowerLeftUV;
    protected int StretchType;
    protected float TrailLength;
    protected float UnitWidth;
    protected Vector2 UVDimensions;
    protected VertexPool.VertexSegment Vertexsegment;
    public const int CHAIN_EMPTY = 99999;

    public int ElemCount;
    public RibbonTrail.Element[] ElementArray;
    public int Head;
    public int MaxElements;
    public float SquaredElemLength;
    public int Tail;

    public RibbonTrail(VertexPool.VertexSegment segment, float width, int maxelemnt, float len, Vector3 pos, int stretchType, float maxFps)
    {
        if (maxelemnt <= 2)
        {
            Debug.LogError("ribbon trail's maxelement should > 2!");
        }
        this.MaxElements = maxelemnt;
        this.Vertexsegment = segment;
        this.ElementArray = new RibbonTrail.Element[this.MaxElements];
        this.Head = (this.Tail = 99999);
        this.SetTrailLen(len);
        this.UnitWidth = width;
        this.HeadPosition = pos;
        this.StretchType = stretchType;
        RibbonTrail.Element dtls = new RibbonTrail.Element(this.HeadPosition, this.UnitWidth);
        this.IndexDirty = false;
        this.Fps = 1f / maxFps;
        this.AddElememt(dtls);
        RibbonTrail.Element dtls2 = new RibbonTrail.Element(this.HeadPosition, this.UnitWidth);
        this.AddElememt(dtls2);
    }

    public void AddElememt(RibbonTrail.Element dtls)
    {
        if (this.Head == 99999)
        {
            this.Tail = this.MaxElements - 1;
            this.Head = this.Tail;
            this.IndexDirty = true;
            this.ElemCount++;
        }
        else
        {
            if (this.Head == 0)
            {
                this.Head = this.MaxElements - 1;
            }
            else
            {
                this.Head--;
            }
            if (this.Head == this.Tail)
            {
                if (this.Tail == 0)
                {
                    this.Tail = this.MaxElements - 1;
                }
                else
                {
                    this.Tail--;
                }
            }
            else
            {
                this.ElemCount++;
            }
        }
        this.ElementArray[this.Head] = dtls;
        this.IndexDirty = true;
    }

    public void Reset()
    {
        this.ResetElementsPos();
    }

    public void ResetElementsPos()
    {
        if (this.Head != 99999 && this.Head != this.Tail)
        {
            int num = this.Head;
            for (; ; )
            {
                int num2 = num;
                if (num2 == this.MaxElements)
                {
                    num2 = 0;
                }
                this.ElementArray[num2].Position = this.HeadPosition;
                if (num2 == this.Tail)
                {
                    break;
                }
                num = num2 + 1;
            }
        }
    }

    public void SetColor(Color color)
    {
        this.Color = color;
    }

    public void SetHeadPosition(Vector3 pos)
    {
        this.HeadPosition = pos;
    }

    public void SetTrailLen(float len)
    {
        this.TrailLength = len;
        this.ElemLength = this.TrailLength / (float)(this.MaxElements - 1);
        this.SquaredElemLength = this.ElemLength * this.ElemLength;
    }

    public void SetUVCoord(Vector2 lowerleft, Vector2 dimensions)
    {
        this.LowerLeftUV = lowerleft;
        this.UVDimensions = dimensions;
    }

    public void Smooth()
    {
        if (this.ElemCount <= 3)
        {
            return;
        }
        RibbonTrail.Element element = this.ElementArray[this.Head];
        int num = this.Head + 1;
        if (num == this.MaxElements)
        {
            num = 0;
        }
        int num2 = num + 1;
        if (num2 == this.MaxElements)
        {
            num2 = 0;
        }
        RibbonTrail.Element element2 = this.ElementArray[num];
        RibbonTrail.Element element3 = this.ElementArray[num2];
        Vector3 from = element.Position - element2.Position;
        Vector3 to = element2.Position - element3.Position;
        float num3 = Vector3.Angle(from, to);
        if (num3 > 60f)
        {
            Vector3 a = (element.Position + element3.Position) / 2f;
            Vector3 vector = a - element2.Position;
            Vector3 zero = Vectors.zero;
            float smoothTime = 0.1f / (num3 / 60f);
            element2.Position = Vector3.SmoothDamp(element2.Position, element2.Position + vector.normalized * element2.Width, ref zero, smoothTime);
        }
    }

    public void Update()
    {
        this.ElapsedTime += Time.deltaTime;
        if (this.ElapsedTime < this.Fps)
        {
            return;
        }
        this.ElapsedTime -= this.Fps;
        bool flag = false;
        while (!flag)
        {
            RibbonTrail.Element element = this.ElementArray[this.Head];
            int num = this.Head + 1;
            if (num == this.MaxElements)
            {
                num = 0;
            }
            RibbonTrail.Element element2 = this.ElementArray[num];
            Vector3 headPosition = this.HeadPosition;
            Vector3 a = headPosition - element2.Position;
            float sqrMagnitude = a.sqrMagnitude;
            if (sqrMagnitude >= this.SquaredElemLength)
            {
                Vector3 b = a * (this.ElemLength / a.magnitude);
                element.Position = element2.Position + b;
                RibbonTrail.Element dtls = new RibbonTrail.Element(headPosition, this.UnitWidth);
                this.AddElememt(dtls);
                a = headPosition - element.Position;
                if (a.sqrMagnitude <= this.SquaredElemLength)
                {
                    flag = true;
                }
            }
            else
            {
                element.Position = headPosition;
                flag = true;
            }
            if ((this.Tail + 1) % this.MaxElements == this.Head)
            {
                RibbonTrail.Element element3 = this.ElementArray[this.Tail];
                int num2;
                if (this.Tail == 0)
                {
                    num2 = this.MaxElements - 1;
                }
                else
                {
                    num2 = this.Tail - 1;
                }
                RibbonTrail.Element element4 = this.ElementArray[num2];
                Vector3 vector = element3.Position - element4.Position;
                float magnitude = vector.magnitude;
                if ((double)magnitude > 1E-06)
                {
                    float num3 = this.ElemLength - a.magnitude;
                    vector *= num3 / magnitude;
                    element3.Position = element4.Position + vector;
                }
            }
        }
        Vector3 position = IN_GAME_MAIN_CAMERA.BaseCamera.transform.position;
        this.UpdateVertices(position);
        this.UpdateIndices();
    }

    public void UpdateIndices()
    {
        if (!this.IndexDirty)
        {
            return;
        }
        VertexPool pool = this.Vertexsegment.Pool;
        if (this.Head != 99999 && this.Head != this.Tail)
        {
            int num = this.Head;
            int num2 = 0;
            for (; ; )
            {
                int num3 = num + 1;
                if (num3 == this.MaxElements)
                {
                    num3 = 0;
                }
                if (num3 * 2 >= 65536)
                {
                    Debug.LogError("Too many elements!");
                }
                int num4 = this.Vertexsegment.VertStart + num3 * 2;
                int num5 = this.Vertexsegment.VertStart + num * 2;
                int num6 = this.Vertexsegment.IndexStart + num2 * 6;
                pool.Indices[num6] = num5;
                pool.Indices[num6 + 1] = num5 + 1;
                pool.Indices[num6 + 2] = num4;
                pool.Indices[num6 + 3] = num5 + 1;
                pool.Indices[num6 + 4] = num4 + 1;
                pool.Indices[num6 + 5] = num4;
                if (num3 == this.Tail)
                {
                    break;
                }
                num = num3;
                num2++;
            }
            pool.IndiceChanged = true;
        }
        this.IndexDirty = false;
    }

    public void UpdateVertices(Vector3 eyePos)
    {
        float num = 0f;
        float num2 = this.ElemLength * (float)(this.MaxElements - 2);
        if (this.Head != 99999 && this.Head != this.Tail)
        {
            int num3 = this.Head;
            int num4 = this.Head;
            for (; ; )
            {
                if (num4 == this.MaxElements)
                {
                    num4 = 0;
                }
                RibbonTrail.Element element = this.ElementArray[num4];
                if (num4 * 2 >= 65536)
                {
                    Debug.LogError("Too many elements!");
                }
                int num5 = this.Vertexsegment.VertStart + num4 * 2;
                int num6 = num4 + 1;
                if (num6 == this.MaxElements)
                {
                    num6 = 0;
                }
                Vector3 lhs;
                if (num4 == this.Head)
                {
                    lhs = this.ElementArray[num6].Position - element.Position;
                }
                else if (num4 == this.Tail)
                {
                    lhs = element.Position - this.ElementArray[num3].Position;
                }
                else
                {
                    lhs = this.ElementArray[num6].Position - this.ElementArray[num3].Position;
                }
                Vector3 rhs = eyePos - element.Position;
                Vector3 vector = Vector3.Cross(lhs, rhs);
                vector.Normalize();
                vector *= element.Width * 0.5f;
                Vector3 vector2 = element.Position - vector;
                Vector3 vector3 = element.Position + vector;
                VertexPool pool = this.Vertexsegment.Pool;
                float num7;
                if (this.StretchType == 0)
                {
                    num7 = num / num2 * Mathf.Abs(this.UVDimensions.y);
                }
                else
                {
                    num7 = num / num2 * Mathf.Abs(this.UVDimensions.x);
                }
                Vector2 zero = Vectors.v2zero;
                pool.Vertices[num5] = vector2;
                pool.Colors[num5] = this.Color;
                if (this.StretchType == 0)
                {
                    zero.x = this.LowerLeftUV.x + this.UVDimensions.x;
                    zero.y = this.LowerLeftUV.y - num7;
                }
                else
                {
                    zero.x = this.LowerLeftUV.x + num7;
                    zero.y = this.LowerLeftUV.y;
                }
                pool.UVs[num5] = zero;
                pool.Vertices[num5 + 1] = vector3;
                pool.Colors[num5 + 1] = this.Color;
                if (this.StretchType == 0)
                {
                    zero.x = this.LowerLeftUV.x;
                    zero.y = this.LowerLeftUV.y - num7;
                }
                else
                {
                    zero.x = this.LowerLeftUV.x + num7;
                    zero.y = this.LowerLeftUV.y - Mathf.Abs(this.UVDimensions.y);
                }
                pool.UVs[num5 + 1] = zero;
                if (num4 == this.Tail)
                {
                    break;
                }
                num3 = num4;
                num += (this.ElementArray[num6].Position - element.Position).magnitude;
                num4++;
            }
            this.Vertexsegment.Pool.UVChanged = true;
            this.Vertexsegment.Pool.VertChanged = true;
            this.Vertexsegment.Pool.ColorChanged = true;
        }
    }

    public class Element
    {
        public Vector3 Position;

        public float Width;

        public Element(Vector3 position, float width)
        {
            this.Position = position;
            this.Width = width;
        }
    }
}