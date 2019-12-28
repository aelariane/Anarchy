using Optimization.Caching;
using System.Collections;
using UnityEngine;

public class EffectLayer : MonoBehaviour
{
    protected Emitter emitter;
    protected Camera MainCamera;
    public EffectNode[] ActiveENodes;
    public bool AlongVelocity;
    public int AngleAroundAxis;
    public bool AttractionAffectorEnable;
    public AnimationCurve AttractionCurve;
    public Vector3 AttractionPosition;
    public float AttractMag = 0.1f;
    public EffectNode[] AvailableENodes;
    public int AvailableNodeCount;
    public Vector3 BoxSize;
    public float ChanceToEmit = 100f;
    public Vector3 CircleDir;
    public Transform ClientTransform;
    public Color Color1 = Color.white;
    public Color Color2;
    public Color Color3;
    public Color Color4;
    public bool ColorAffectorEnable;
    public int ColorAffectType;
    public float ColorGradualTimeLength = 1f;
    public COLOR_GRADUAL_TYPE ColorGradualType;
    public int Cols = 1;
    public float DeltaRot;
    public float DeltaScaleX;
    public float DeltaScaleY;
    public float DiffDistance = 0.1f;
    public int EanIndex;
    public string EanPath = "none";
    public float EmitDelay;
    public float EmitDuration = 10f;
    public int EmitLoop = 1;
    public Vector3 EmitPoint;
    public int EmitRate = 20;
    public int EmitType;
    public bool IsEmitByDistance;
    public bool IsNodeLifeLoop = true;
    public bool IsRandomDir;
    public bool JetAffectorEnable;
    public float JetMax;
    public float JetMin;
    public Vector3 LastClientPos;
    public Vector3 LinearForce;
    public bool LinearForceAffectorEnable;
    public float LinearMagnitude = 1f;
    public float LineLengthLeft = -1f;
    public float LineLengthRight = 1f;
    public int LoopCircles = -1;
    public Material Material;
    public int MaxENodes = 1;
    public float MaxFps = 60f;
    public int MaxRibbonElements = 6;
    public float NodeLifeMax = 1f;
    public float NodeLifeMin = 1f;
    public Vector2 OriLowerLeftUV = Vectors.v2zero;
    public int OriPoint;
    public int OriRotationMax;
    public int OriRotationMin;
    public float OriScaleXMax = 1f;
    public float OriScaleXMin = 1f;
    public float OriScaleYMax = 1f;
    public float OriScaleYMin = 1f;
    public float OriSpeed;
    public Vector2 OriUVDimensions = Vectors.v2one;
    public Vector3 OriVelocityAxis;
    public float Radius;
    public bool RandomOriRot;
    public bool RandomOriScale;
    public int RenderType;
    public float RibbonLen = 1f;
    public float RibbonWidth = 0.5f;
    public bool RotAffectorEnable;
    public AnimationCurve RotateCurve;
    public RSTYPE RotateType;
    public int Rows = 1;
    public bool ScaleAffectorEnable;
    public RSTYPE ScaleType;
    public AnimationCurve ScaleXCurve;
    public AnimationCurve ScaleYCurve;
    public float SpriteHeight = 1f;
    public int SpriteType;
    public int SpriteUVStretch;
    public float SpriteWidth = 1f;
    public float StartTime;
    public int StretchType;
    public bool SyncClient;
    public float TailDistance;
    public bool UseAttractCurve;
    public bool UseVortexCurve;
    public bool UVAffectorEnable;
    public float UVTime = 30f;
    public int UVType;
    public VertexPool Vertexpool;

    public bool VortexAffectorEnable;
    public AnimationCurve VortexCurve;
    public Vector3 VortexDirection;
    public float VortexMag = 0.1f;

    private void OnDrawGizmosSelected()
    {
    }

    protected void AddNodes(int num)
    {
        int num2 = 0;
        for (int i = 0; i < this.MaxENodes; i++)
        {
            if (num2 == num)
            {
                break;
            }
            EffectNode effectNode = this.AvailableENodes[i];
            if (effectNode != null)
            {
                this.AddActiveNode(effectNode);
                num2++;
                this.emitter.SetEmitPosition(effectNode);
                float life;
                if (this.IsNodeLifeLoop)
                {
                    life = -1f;
                }
                else
                {
                    life = UnityEngine.Random.Range(this.NodeLifeMin, this.NodeLifeMax);
                }
                effectNode.Init(this.emitter.GetEmitRotation(effectNode).normalized, this.OriSpeed, life, UnityEngine.Random.Range(this.OriRotationMin, this.OriRotationMax), UnityEngine.Random.Range(this.OriScaleXMin, this.OriScaleXMax), UnityEngine.Random.Range(this.OriScaleYMin, this.OriScaleYMax), this.Color1, this.OriLowerLeftUV, this.OriUVDimensions);
            }
        }
    }

    protected void Init()
    {
        this.AvailableENodes = new EffectNode[this.MaxENodes];
        this.ActiveENodes = new EffectNode[this.MaxENodes];
        for (int i = 0; i < this.MaxENodes; i++)
        {
            EffectNode effectNode = new EffectNode(i, this.ClientTransform, this.SyncClient, this);
            ArrayList affectorList = this.InitAffectors(effectNode);
            effectNode.SetAffectorList(affectorList);
            if (this.RenderType == 0)
            {
                effectNode.SetType(this.SpriteWidth, this.SpriteHeight, (STYPE)this.SpriteType, (ORIPOINT)this.OriPoint, this.SpriteUVStretch, this.MaxFps);
            }
            else
            {
                effectNode.SetType(this.RibbonWidth, this.MaxRibbonElements, this.RibbonLen, this.ClientTransform.position, this.StretchType, this.MaxFps);
            }
            this.AvailableENodes[i] = effectNode;
        }
        this.AvailableNodeCount = this.MaxENodes;
        this.emitter = new Emitter(this);
    }

    protected ArrayList InitAffectors(EffectNode node)
    {
        ArrayList arrayList = new ArrayList();
        if (this.UVAffectorEnable)
        {
            UVAnimation uvanimation = new UVAnimation();
            Texture texture = this.Vertexpool.GetMaterial().GetTexture("_MainTex");
            if (this.UVType == 2)
            {
                uvanimation.BuildFromFile(this.EanPath, this.EanIndex, this.UVTime, texture);
                this.OriLowerLeftUV = uvanimation.frames[0];
                this.OriUVDimensions = uvanimation.UVDimensions[0];
            }
            else if (this.UVType == 1)
            {
                float num = (float)(texture.width / this.Cols);
                float num2 = (float)(texture.height / this.Rows);
                Vector2 vector = new Vector2(num / (float)texture.width, num2 / (float)texture.height);
                Vector2 vector2 = new Vector2(0f, 1f);
                uvanimation.BuildUVAnim(vector2, vector, this.Cols, this.Rows, this.Cols * this.Rows);
                this.OriLowerLeftUV = vector2;
                this.OriUVDimensions = vector;
                this.OriUVDimensions.y = -this.OriUVDimensions.y;
            }
            if (uvanimation.frames.Length == 1)
            {
                this.OriLowerLeftUV = uvanimation.frames[0];
                this.OriUVDimensions = uvanimation.UVDimensions[0];
            }
            else
            {
                uvanimation.loopCycles = this.LoopCircles;
                Affector value = new UVAffector(uvanimation, this.UVTime, node);
                arrayList.Add(value);
            }
        }
        if (this.RotAffectorEnable && this.RotateType != RSTYPE.NONE)
        {
            Affector value2;
            if (this.RotateType == RSTYPE.CURVE)
            {
                value2 = new RotateAffector(this.RotateCurve, node);
            }
            else
            {
                value2 = new RotateAffector(this.DeltaRot, node);
            }
            arrayList.Add(value2);
        }
        if (this.ScaleAffectorEnable && this.ScaleType != RSTYPE.NONE)
        {
            Affector value3;
            if (this.ScaleType == RSTYPE.CURVE)
            {
                value3 = new ScaleAffector(this.ScaleXCurve, this.ScaleYCurve, node);
            }
            else
            {
                value3 = new ScaleAffector(this.DeltaScaleX, this.DeltaScaleY, node);
            }
            arrayList.Add(value3);
        }
        if (this.ColorAffectorEnable && this.ColorAffectType != 0)
        {
            ColorAffector value4;
            if (this.ColorAffectType == 2)
            {
                value4 = new ColorAffector(new Color[]
                {
                    this.Color1,
                    this.Color2,
                    this.Color3,
                    this.Color4
                }, this.ColorGradualTimeLength, this.ColorGradualType, node);
            }
            else
            {
                value4 = new ColorAffector(new Color[]
                {
                    this.Color1,
                    this.Color2
                }, this.ColorGradualTimeLength, this.ColorGradualType, node);
            }
            arrayList.Add(value4);
        }
        if (this.LinearForceAffectorEnable)
        {
            Affector value5 = new LinearForceAffector(this.LinearForce.normalized * this.LinearMagnitude, node);
            arrayList.Add(value5);
        }
        if (this.JetAffectorEnable)
        {
            Affector value6 = new JetAffector(this.JetMin, this.JetMax, node);
            arrayList.Add(value6);
        }
        if (this.VortexAffectorEnable)
        {
            Affector value7;
            if (this.UseVortexCurve)
            {
                value7 = new VortexAffector(this.VortexCurve, this.VortexDirection, node);
            }
            else
            {
                value7 = new VortexAffector(this.VortexMag, this.VortexDirection, node);
            }
            arrayList.Add(value7);
        }
        if (this.AttractionAffectorEnable)
        {
            Affector value8;
            if (this.UseVortexCurve)
            {
                value8 = new AttractionForceAffector(this.AttractionCurve, this.AttractionPosition, node);
            }
            else
            {
                value8 = new AttractionForceAffector(this.AttractMag, this.AttractionPosition, node);
            }
            arrayList.Add(value8);
        }
        return arrayList;
    }

    public void AddActiveNode(EffectNode node)
    {
        if (this.AvailableNodeCount == 0)
        {
            Debug.LogError("out index!");
        }
        if (this.AvailableENodes[node.Index] == null)
        {
            return;
        }
        this.ActiveENodes[node.Index] = node;
        this.AvailableENodes[node.Index] = null;
        this.AvailableNodeCount--;
    }

    public void FixedUpdateCustom()
    {
        int nodes = this.emitter.GetNodes();
        this.AddNodes(nodes);
        for (int i = 0; i < this.MaxENodes; i++)
        {
            EffectNode effectNode = this.ActiveENodes[i];
            if (effectNode != null)
            {
                effectNode.Update();
            }
        }
    }

    public RibbonTrail GetRibbonTrail()
    {
        if ((this.ActiveENodes == null | this.ActiveENodes.Length != 1) || this.MaxENodes != 1 || this.RenderType != 1)
        {
            return null;
        }
        return this.ActiveENodes[0].Ribbon;
    }

    public VertexPool GetVertexPool()
    {
        return this.Vertexpool;
    }

    public void RemoveActiveNode(EffectNode node)
    {
        if (this.AvailableNodeCount == this.MaxENodes)
        {
            Debug.LogError("out index!");
        }
        if (this.ActiveENodes[node.Index] == null)
        {
            return;
        }
        this.ActiveENodes[node.Index] = null;
        this.AvailableENodes[node.Index] = node;
        this.AvailableNodeCount++;
    }

    public void Reset()
    {
        for (int i = 0; i < this.MaxENodes; i++)
        {
            if (this.ActiveENodes == null)
            {
                return;
            }
            EffectNode effectNode = this.ActiveENodes[i];
            if (effectNode != null)
            {
                effectNode.Reset();
                this.RemoveActiveNode(effectNode);
            }
        }
        this.emitter.Reset();
    }

    public void StartCustom()
    {
        if (this.MainCamera == null)
        {
            this.MainCamera = IN_GAME_MAIN_CAMERA.BaseCamera;
        }
        this.Init();
        this.LastClientPos = this.ClientTransform.position;
    }
}