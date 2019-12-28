using UnityEngine;

public class VertexPool
{
    protected bool FirstUpdate = true;
    protected int IndexTotal;
    protected int IndexUsed;
    protected bool VertCountChanged;
    protected int VertexTotal;
    protected int VertexUsed;
    public const int BlockSize = 36;

    public float BoundsScheduleTime = 1f;
    public bool ColorChanged;
    public Color[] Colors;
    public float ElapsedTime;
    public bool IndiceChanged;
    public int[] Indices;
    public Material Material;
    public Mesh Mesh;
    public bool UVChanged;
    public Vector2[] UVs;
    public bool VertChanged;
    public Vector3[] Vertices;

    public VertexPool(Mesh mesh, Material material)
    {
        this.VertexTotal = (this.VertexUsed = 0);
        this.VertCountChanged = false;
        this.Mesh = mesh;
        this.Material = material;
        this.InitArrays();
        this.Vertices = this.Mesh.vertices;
        this.Indices = this.Mesh.triangles;
        this.Colors = this.Mesh.colors;
        this.UVs = this.Mesh.uv;
        this.IndiceChanged = (this.ColorChanged = (this.UVChanged = (this.VertChanged = true)));
    }

    protected void InitArrays()
    {
        this.Vertices = new Vector3[4];
        this.UVs = new Vector2[4];
        this.Colors = new Color[4];
        this.Indices = new int[6];
        this.VertexTotal = 4;
        this.IndexTotal = 6;
    }

    public RibbonTrail AddRibbonTrail(float width, int maxelemnt, float len, Vector3 pos, int stretchType, float maxFps)
    {
        VertexPool.VertexSegment vertices = this.GetVertices(maxelemnt * 2, (maxelemnt - 1) * 6);
        return new RibbonTrail(vertices, width, maxelemnt, len, pos, stretchType, maxFps);
    }

    public global::Sprite AddSprite(float width, float height, STYPE type, ORIPOINT ori, Camera cam, int uvStretch, float maxFps)
    {
        VertexPool.VertexSegment vertices = this.GetVertices(4, 6);
        return new global::Sprite(vertices, width, height, type, ori, cam, uvStretch, maxFps);
    }

    public void EnlargeArrays(int count, int icount)
    {
        Vector3[] vertices = this.Vertices;
        this.Vertices = new Vector3[this.Vertices.Length + count];
        vertices.CopyTo(this.Vertices, 0);
        Vector2[] uvs = this.UVs;
        this.UVs = new Vector2[this.UVs.Length + count];
        uvs.CopyTo(this.UVs, 0);
        Color[] colors = this.Colors;
        this.Colors = new Color[this.Colors.Length + count];
        colors.CopyTo(this.Colors, 0);
        int[] indices = this.Indices;
        this.Indices = new int[this.Indices.Length + icount];
        indices.CopyTo(this.Indices, 0);
        this.VertCountChanged = true;
        this.IndiceChanged = true;
        this.ColorChanged = true;
        this.UVChanged = true;
        this.VertChanged = true;
    }

    public Material GetMaterial()
    {
        return this.Material;
    }

    public VertexPool.VertexSegment GetVertices(int vcount, int icount)
    {
        int num = 0;
        int num2 = 0;
        if (this.VertexUsed + vcount >= this.VertexTotal)
        {
            num = (vcount / 36 + 1) * 36;
        }
        if (this.IndexUsed + icount >= this.IndexTotal)
        {
            num2 = (icount / 36 + 1) * 36;
        }
        this.VertexUsed += vcount;
        this.IndexUsed += icount;
        if (num != 0 || num2 != 0)
        {
            this.EnlargeArrays(num, num2);
            this.VertexTotal += num;
            this.IndexTotal += num2;
        }
        return new VertexPool.VertexSegment(this.VertexUsed - vcount, vcount, this.IndexUsed - icount, icount, this);
    }

    public void LateUpdate()
    {
        if (this.VertCountChanged)
        {
            this.Mesh.Clear();
        }
        this.Mesh.vertices = this.Vertices;
        if (this.UVChanged)
        {
            this.Mesh.uv = this.UVs;
        }
        if (this.ColorChanged)
        {
            this.Mesh.colors = this.Colors;
        }
        if (this.IndiceChanged)
        {
            this.Mesh.triangles = this.Indices;
        }
        this.ElapsedTime += Time.deltaTime;
        if (this.ElapsedTime > this.BoundsScheduleTime || this.FirstUpdate)
        {
            this.RecalculateBounds();
            this.ElapsedTime = 0f;
        }
        if (this.ElapsedTime > this.BoundsScheduleTime)
        {
            this.FirstUpdate = false;
        }
        this.VertCountChanged = false;
        this.IndiceChanged = false;
        this.ColorChanged = false;
        this.UVChanged = false;
        this.VertChanged = false;
    }

    public void RecalculateBounds()
    {
        this.Mesh.RecalculateBounds();
    }

    public class VertexSegment
    {
        public int IndexCount;
        public int IndexStart;
        public VertexPool Pool;
        public int VertCount;
        public int VertStart;

        public VertexSegment(int start, int count, int istart, int icount, VertexPool pool)
        {
            this.VertStart = start;
            this.VertCount = count;
            this.IndexCount = icount;
            this.IndexStart = istart;
            this.Pool = pool;
        }
    }
}