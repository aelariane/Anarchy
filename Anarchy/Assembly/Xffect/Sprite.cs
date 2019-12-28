using Optimization.Caching;
using UnityEngine;

public class Sprite
{
    private Matrix4x4 LocalMat;
    private ORIPOINT OriPoint;
    private Quaternion Rotation;
    private Vector3 ScaleVector;
    private STYPE Type;
    private int UVStretch;
    private Matrix4x4 WorldMat;
    protected bool ColorChanged;
    protected float ElapsedTime;
    protected float Fps;
    protected Matrix4x4 LastMat;
    protected Vector2 LowerLeftUV;
    protected Vector3 RotateAxis;
    protected bool UVChanged;
    protected Vector2 UVDimensions;
    protected VertexPool.VertexSegment Vertexsegment;
    public Color Color;
    public Camera MainCamera;
    public STransform MyTransform;
    public Vector3 v1 = Vectors.zero;
    public Vector3 v2 = Vectors.zero;
    public Vector3 v3 = Vectors.zero;
    public Vector3 v4 = Vectors.zero;

    public Sprite(VertexPool.VertexSegment segment, float width, float height, STYPE type, ORIPOINT oripoint, Camera cam, int uvStretch, float maxFps)
    {
        this.UVChanged = (this.ColorChanged = false);
        this.MyTransform.position = Vectors.zero;
        this.MyTransform.rotation = Quaternion.identity;
        this.LocalMat = (this.WorldMat = Matrix4x4.identity);
        this.Vertexsegment = segment;
        this.UVStretch = uvStretch;
        this.LastMat = Matrix4x4.identity;
        this.ElapsedTime = 0f;
        this.Fps = 1f / maxFps;
        this.OriPoint = oripoint;
        this.RotateAxis = Vectors.zero;
        this.SetSizeXZ(width, height);
        this.RotateAxis.y = 1f;
        this.Type = type;
        this.MainCamera = cam;
        this.ResetSegment();
    }

    public void Init(Color color, Vector2 lowerLeftUV, Vector2 uvDimensions)
    {
        this.SetUVCoord(lowerLeftUV, uvDimensions);
        this.SetColor(color);
        this.SetRotation(Quaternion.identity);
        this.SetScale(1f, 1f);
        this.SetRotation(0f);
    }

    public void Reset()
    {
        this.MyTransform.Reset();
        this.SetColor(Color.white);
        this.SetUVCoord(Vectors.v2zero, Vectors.v2zero);
        this.ScaleVector = Vectors.one;
        this.Rotation = Quaternion.identity;
        VertexPool pool = this.Vertexsegment.Pool;
        int vertStart = this.Vertexsegment.VertStart;
        pool.Vertices[vertStart] = Vectors.zero;
        pool.Vertices[vertStart + 1] = Vectors.zero;
        pool.Vertices[vertStart + 2] = Vectors.zero;
        pool.Vertices[vertStart + 3] = Vectors.zero;
    }

    public void ResetSegment()
    {
        VertexPool pool = this.Vertexsegment.Pool;
        int indexStart = this.Vertexsegment.IndexStart;
        int vertStart = this.Vertexsegment.VertStart;
        pool.Indices[indexStart] = vertStart;
        pool.Indices[indexStart + 1] = vertStart + 3;
        pool.Indices[indexStart + 2] = vertStart + 1;
        pool.Indices[indexStart + 3] = vertStart + 3;
        pool.Indices[indexStart + 4] = vertStart + 2;
        pool.Indices[indexStart + 5] = vertStart + 1;
        pool.Vertices[vertStart] = Vectors.zero;
        pool.Vertices[vertStart + 1] = Vectors.zero;
        pool.Vertices[vertStart + 2] = Vectors.zero;
        pool.Vertices[vertStart + 3] = Vectors.zero;
        pool.Colors[vertStart] = Color.white;
        pool.Colors[vertStart + 1] = Color.white;
        pool.Colors[vertStart + 2] = Color.white;
        pool.Colors[vertStart + 3] = Color.white;
        pool.UVs[vertStart] = Vectors.v2zero;
        pool.UVs[vertStart + 1] = Vectors.v2zero;
        pool.UVs[vertStart + 2] = Vectors.v2zero;
        pool.UVs[vertStart + 3] = Vectors.v2zero;
        pool.UVChanged = (pool.IndiceChanged = (pool.ColorChanged = (pool.VertChanged = true)));
    }

    public void SetColor(Color c)
    {
        this.Color = c;
        this.ColorChanged = true;
    }

    public void SetPosition(Vector3 pos)
    {
        this.MyTransform.position = pos;
    }

    public void SetRotation(Quaternion q)
    {
        this.MyTransform.rotation = q;
    }

    public void SetRotation(float angle)
    {
        this.Rotation = Quaternion.AngleAxis(angle, this.RotateAxis);
    }

    public void SetRotationFaceTo(Vector3 dir)
    {
        this.MyTransform.rotation = Quaternion.FromToRotation(Vectors.up, dir);
    }

    public void SetRotationTo(Vector3 dir)
    {
        if (dir == Vectors.zero)
        {
            return;
        }
        Quaternion rotation = Quaternion.identity;
        Vector3 vector = dir;
        vector.y = 0f;
        if (vector == Vectors.zero)
        {
            vector = Vectors.up;
        }
        if (this.OriPoint == ORIPOINT.CENTER)
        {
            Quaternion rhs = Quaternion.FromToRotation(new Vector3(0f, 0f, 1f), vector);
            Quaternion lhs = Quaternion.FromToRotation(vector, dir);
            rotation = lhs * rhs;
        }
        else if (this.OriPoint == ORIPOINT.LEFT_UP)
        {
            Quaternion rhs2 = Quaternion.FromToRotation(this.LocalMat.MultiplyPoint3x4(this.v3), vector);
            Quaternion lhs2 = Quaternion.FromToRotation(vector, dir);
            rotation = lhs2 * rhs2;
        }
        else if (this.OriPoint == ORIPOINT.LEFT_BOTTOM)
        {
            Quaternion rhs3 = Quaternion.FromToRotation(this.LocalMat.MultiplyPoint3x4(this.v4), vector);
            Quaternion lhs3 = Quaternion.FromToRotation(vector, dir);
            rotation = lhs3 * rhs3;
        }
        else if (this.OriPoint == ORIPOINT.RIGHT_BOTTOM)
        {
            Quaternion rhs4 = Quaternion.FromToRotation(this.LocalMat.MultiplyPoint3x4(this.v1), vector);
            Quaternion lhs4 = Quaternion.FromToRotation(vector, dir);
            rotation = lhs4 * rhs4;
        }
        else if (this.OriPoint == ORIPOINT.RIGHT_UP)
        {
            Quaternion rhs5 = Quaternion.FromToRotation(this.LocalMat.MultiplyPoint3x4(this.v2), vector);
            Quaternion lhs5 = Quaternion.FromToRotation(vector, dir);
            rotation = lhs5 * rhs5;
        }
        else if (this.OriPoint == ORIPOINT.BOTTOM_CENTER)
        {
            Quaternion rhs6 = Quaternion.FromToRotation(new Vector3(0f, 0f, 1f), vector);
            Quaternion lhs6 = Quaternion.FromToRotation(vector, dir);
            rotation = lhs6 * rhs6;
        }
        else if (this.OriPoint == ORIPOINT.TOP_CENTER)
        {
            Quaternion rhs7 = Quaternion.FromToRotation(new Vector3(0f, 0f, -1f), vector);
            Quaternion lhs7 = Quaternion.FromToRotation(vector, dir);
            rotation = lhs7 * rhs7;
        }
        else if (this.OriPoint == ORIPOINT.RIGHT_CENTER)
        {
            Quaternion rhs8 = Quaternion.FromToRotation(new Vector3(-1f, 0f, 0f), vector);
            Quaternion lhs8 = Quaternion.FromToRotation(vector, dir);
            rotation = lhs8 * rhs8;
        }
        else if (this.OriPoint == ORIPOINT.LEFT_CENTER)
        {
            Quaternion rhs9 = Quaternion.FromToRotation(new Vector3(1f, 0f, 0f), vector);
            Quaternion lhs9 = Quaternion.FromToRotation(vector, dir);
            rotation = lhs9 * rhs9;
        }
        this.MyTransform.rotation = rotation;
    }

    public void SetScale(float width, float height)
    {
        this.ScaleVector.x = width;
        this.ScaleVector.z = height;
    }

    public void SetSizeXZ(float width, float height)
    {
        this.v1 = new Vector3(-width / 2f, 0f, height / 2f);
        this.v2 = new Vector3(-width / 2f, 0f, -height / 2f);
        this.v3 = new Vector3(width / 2f, 0f, -height / 2f);
        this.v4 = new Vector3(width / 2f, 0f, height / 2f);
        Vector3 zero = Vectors.zero;
        if (this.OriPoint == ORIPOINT.LEFT_UP)
        {
            zero = this.v3;
        }
        else if (this.OriPoint == ORIPOINT.LEFT_BOTTOM)
        {
            zero = this.v4;
        }
        else if (this.OriPoint == ORIPOINT.RIGHT_BOTTOM)
        {
            zero = this.v1;
        }
        else if (this.OriPoint == ORIPOINT.RIGHT_UP)
        {
            zero = this.v2;
        }
        else if (this.OriPoint == ORIPOINT.BOTTOM_CENTER)
        {
            zero = new Vector3(0f, 0f, height / 2f);
        }
        else if (this.OriPoint == ORIPOINT.TOP_CENTER)
        {
            zero = new Vector3(0f, 0f, -height / 2f);
        }
        else if (this.OriPoint == ORIPOINT.LEFT_CENTER)
        {
            zero = new Vector3(width / 2f, 0f, 0f);
        }
        else if (this.OriPoint == ORIPOINT.RIGHT_CENTER)
        {
            zero = new Vector3(-width / 2f, 0f, 0f);
        }
        this.v1 += zero;
        this.v2 += zero;
        this.v3 += zero;
        this.v4 += zero;
    }

    public void SetUVCoord(Vector2 lowerleft, Vector2 dimensions)
    {
        this.LowerLeftUV = lowerleft;
        this.UVDimensions = dimensions;
        this.UVChanged = true;
    }

    public void Transform()
    {
        this.LocalMat.SetTRS(Vectors.zero, this.Rotation, this.ScaleVector);
        if (this.Type == STYPE.BILLBOARD)
        {
            Transform transform = this.MainCamera.transform;
            this.MyTransform.LookAt(this.MyTransform.position + transform.rotation * Vectors.up, transform.rotation * Vectors.back);
        }
        this.WorldMat.SetTRS(this.MyTransform.position, this.MyTransform.rotation, Vectors.one);
        Matrix4x4 matrix4x = this.WorldMat * this.LocalMat;
        VertexPool pool = this.Vertexsegment.Pool;
        int vertStart = this.Vertexsegment.VertStart;
        Vector3 vector = matrix4x.MultiplyPoint3x4(this.v1);
        Vector3 vector2 = matrix4x.MultiplyPoint3x4(this.v2);
        Vector3 vector3 = matrix4x.MultiplyPoint3x4(this.v3);
        Vector3 vector4 = matrix4x.MultiplyPoint3x4(this.v4);
        if (this.Type == STYPE.BILLBOARD_SELF)
        {
            Vector3 vector5 = Vectors.zero;
            Vector3 vector6 = Vectors.zero;
            float magnitude;
            if (this.UVStretch == 0)
            {
                vector5 = (vector + vector4) / 2f;
                vector6 = (vector2 + vector3) / 2f;
                magnitude = (vector4 - vector).magnitude;
            }
            else
            {
                vector5 = (vector + vector2) / 2f;
                vector6 = (vector4 + vector3) / 2f;
                magnitude = (vector2 - vector).magnitude;
            }
            Vector3 lhs = vector5 - vector6;
            Vector3 rhs = this.MainCamera.transform.position - vector5;
            Vector3 vector7 = Vector3.Cross(lhs, rhs);
            vector7.Normalize();
            vector7 *= magnitude * 0.5f;
            Vector3 rhs2 = this.MainCamera.transform.position - vector6;
            Vector3 vector8 = Vector3.Cross(lhs, rhs2);
            vector8.Normalize();
            vector8 *= magnitude * 0.5f;
            if (this.UVStretch == 0)
            {
                vector = vector5 - vector7;
                vector4 = vector5 + vector7;
                vector2 = vector6 - vector8;
                vector3 = vector6 + vector8;
            }
            else
            {
                vector = vector5 - vector7;
                vector2 = vector5 + vector7;
                vector4 = vector6 - vector8;
                vector3 = vector6 + vector8;
            }
        }
        pool.Vertices[vertStart] = vector;
        pool.Vertices[vertStart + 1] = vector2;
        pool.Vertices[vertStart + 2] = vector3;
        pool.Vertices[vertStart + 3] = vector4;
    }

    public void Update(bool force)
    {
        this.ElapsedTime += Time.deltaTime;
        if (this.ElapsedTime > this.Fps || force)
        {
            this.Transform();
            if (this.UVChanged)
            {
                this.UpdateUV();
            }
            if (this.ColorChanged)
            {
                this.UpdateColor();
            }
            this.UVChanged = (this.ColorChanged = false);
            if (!force)
            {
                this.ElapsedTime -= this.Fps;
            }
        }
    }

    public void UpdateColor()
    {
        VertexPool pool = this.Vertexsegment.Pool;
        int vertStart = this.Vertexsegment.VertStart;
        pool.Colors[vertStart] = this.Color;
        pool.Colors[vertStart + 1] = this.Color;
        pool.Colors[vertStart + 2] = this.Color;
        pool.Colors[vertStart + 3] = this.Color;
        this.Vertexsegment.Pool.ColorChanged = true;
    }

    public void UpdateUV()
    {
        VertexPool pool = this.Vertexsegment.Pool;
        int vertStart = this.Vertexsegment.VertStart;
        if (this.UVDimensions.y > 0f)
        {
            pool.UVs[vertStart] = this.LowerLeftUV + Vector2.up * this.UVDimensions.y;
            pool.UVs[vertStart + 1] = this.LowerLeftUV;
            pool.UVs[vertStart + 2] = this.LowerLeftUV + Vector2.right * this.UVDimensions.x;
            pool.UVs[vertStart + 3] = this.LowerLeftUV + this.UVDimensions;
        }
        else
        {
            pool.UVs[vertStart] = this.LowerLeftUV;
            pool.UVs[vertStart + 1] = this.LowerLeftUV + Vector2.up * this.UVDimensions.y;
            pool.UVs[vertStart + 2] = this.LowerLeftUV + this.UVDimensions;
            pool.UVs[vertStart + 3] = this.LowerLeftUV + Vector2.right * this.UVDimensions.x;
        }
        this.Vertexsegment.Pool.UVChanged = true;
    }
}