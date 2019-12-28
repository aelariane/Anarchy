using Optimization.Caching;
using System.Collections;
using UnityEngine;

public class EffectNode
{
    protected ArrayList AffectorList;
    protected Vector3 CurDirection;
    protected Vector3 CurWorldPos;
    protected float ElapsedTime;
    protected Vector3 LastWorldPos = Vectors.zero;
    protected float LifeTime;
    protected Vector3 OriDirection;
    protected int OriRotateAngle;
    protected float OriScaleX;
    protected float OriScaleY;
    protected int Type;
    public float Acceleration;
    public Transform ClientTrans;
    public Color Color;
    public int Index;
    public Vector2 LowerLeftUV;
    public EffectLayer Owner;
    public Vector3 Position;
    public RibbonTrail Ribbon;
    public float RotateAngle;
    public Vector2 Scale;
    public global::Sprite Sprite;
    public bool SyncClient;
    public Vector2 UVDimensions;
    public Vector3 Velocity;

    public EffectNode(int index, Transform clienttrans, bool sync, EffectLayer owner)
    {
        this.Index = index;
        this.ClientTrans = clienttrans;
        this.SyncClient = sync;
        this.Owner = owner;
        this.LowerLeftUV = Vectors.v2zero;
        this.UVDimensions = Vectors.v2one;
        this.Scale = Vectors.v2one;
        this.RotateAngle = 0f;
        this.Color = Color.white;
    }

    public float GetElapsedTime()
    {
        return this.ElapsedTime;
    }

    public float GetLifeTime()
    {
        return this.LifeTime;
    }

    public Vector3 GetLocalPosition()
    {
        return this.Position;
    }

    public void Init(Vector3 oriDir, float speed, float life, int oriRot, float oriScaleX, float oriScaleY, Color oriColor, Vector2 oriLowerUv, Vector2 oriUVDimension)
    {
        this.OriDirection = oriDir;
        this.LifeTime = life;
        this.OriRotateAngle = oriRot;
        this.OriScaleX = oriScaleX;
        this.OriScaleY = oriScaleY;
        this.Color = oriColor;
        this.ElapsedTime = 0f;
        this.Velocity = this.OriDirection * speed;
        this.Acceleration = 0f;
        this.LowerLeftUV = oriLowerUv;
        this.UVDimensions = oriUVDimension;
        if (this.Type == 1)
        {
            this.Sprite.SetUVCoord(this.LowerLeftUV, this.UVDimensions);
            this.Sprite.SetColor(oriColor);
        }
        else if (this.Type == 2)
        {
            this.Ribbon.SetUVCoord(this.LowerLeftUV, this.UVDimensions);
            this.Ribbon.SetColor(oriColor);
            this.Ribbon.SetHeadPosition(this.ClientTrans.position + this.Position + this.OriDirection.normalized * this.Owner.TailDistance);
            this.Ribbon.ResetElementsPos();
        }
        if (this.Type == 1)
        {
            this.Sprite.SetRotationTo(this.OriDirection);
        }
    }

    public void Remove()
    {
        this.Owner.RemoveActiveNode(this);
    }

    public void Reset()
    {
        this.Position = Vectors.up * 9999f;
        this.Velocity = Vectors.zero;
        this.Acceleration = 0f;
        this.ElapsedTime = 0f;
        this.LastWorldPos = (this.CurWorldPos = Vectors.zero);
        foreach (object obj in this.AffectorList)
        {
            Affector affector = (Affector)obj;
            affector.Reset();
        }
        if (this.Type == 1)
        {
            this.Sprite.SetRotation((float)this.OriRotateAngle);
            this.Sprite.SetPosition(this.Position);
            this.Sprite.SetColor(Color.clear);
            this.Sprite.Update(true);
            this.Scale = Vectors.v2one;
        }
        else if (this.Type == 2)
        {
            this.Ribbon.SetHeadPosition(this.ClientTrans.position + this.OriDirection.normalized * this.Owner.TailDistance);
            this.Ribbon.Reset();
            this.Ribbon.SetColor(Color.clear);
            this.Ribbon.UpdateVertices(Vectors.zero);
        }
    }

    public void SetAffectorList(ArrayList afts)
    {
        this.AffectorList = afts;
    }

    public void SetLocalPosition(Vector3 pos)
    {
        this.Position = pos;
    }

    public void SetType(float width, float height, STYPE type, ORIPOINT orip, int uvStretch, float maxFps)
    {
        this.Type = 1;
        this.Sprite = this.Owner.GetVertexPool().AddSprite(width, height, type, orip, IN_GAME_MAIN_CAMERA.BaseCamera, uvStretch, maxFps);
    }

    public void SetType(float width, int maxelemnt, float len, Vector3 pos, int stretchType, float maxFps)
    {
        this.Type = 2;
        this.Ribbon = this.Owner.GetVertexPool().AddRibbonTrail(width, maxelemnt, len, pos, stretchType, maxFps);
    }

    public void Update()
    {
        this.ElapsedTime += Time.deltaTime;
        foreach (object obj in this.AffectorList)
        {
            Affector affector = (Affector)obj;
            affector.Update();
        }
        this.Position += this.Velocity * Time.deltaTime;
        if ((double)Mathf.Abs(this.Acceleration) > 0.0001)
        {
            this.Velocity += this.Velocity.normalized * this.Acceleration * Time.deltaTime;
        }
        if (this.SyncClient)
        {
            this.CurWorldPos = this.ClientTrans.TransformPoint(this.Position);
        }
        else
        {
            this.CurWorldPos = this.Position;
        }
        if (this.Type == 1)
        {
            this.UpdateSprite();
        }
        else if (this.Type == 2)
        {
            this.UpdateRibbonTrail();
        }
        this.LastWorldPos = this.CurWorldPos;
        if (this.ElapsedTime > this.LifeTime && this.LifeTime > 0f)
        {
            this.Reset();
            this.Remove();
        }
    }

    public void UpdateRibbonTrail()
    {
        this.Ribbon.SetHeadPosition(this.CurWorldPos);
        if (this.Owner.UVAffectorEnable)
        {
            this.Ribbon.SetUVCoord(this.LowerLeftUV, this.UVDimensions);
        }
        this.Ribbon.SetColor(this.Color);
        this.Ribbon.Update();
    }

    public void UpdateSprite()
    {
        if (this.Owner.AlongVelocity)
        {
            Vector3 vector = Vectors.zero;
            if (!(this.LastWorldPos != Vectors.zero))
            {
                return;
            }
            vector = this.CurWorldPos - this.LastWorldPos;
            if (vector != Vectors.zero)
            {
                this.CurDirection = vector;
                this.Sprite.SetRotationTo(this.CurDirection);
            }
        }
        this.Sprite.SetScale(this.Scale.x * this.OriScaleX, this.Scale.y * this.OriScaleY);
        if (this.Owner.ColorAffectorEnable)
        {
            this.Sprite.SetColor(this.Color);
        }
        if (this.Owner.UVAffectorEnable)
        {
            this.Sprite.SetUVCoord(this.LowerLeftUV, this.UVDimensions);
        }
        this.Sprite.SetRotation((float)this.OriRotateAngle + this.RotateAngle);
        this.Sprite.SetPosition(this.CurWorldPos);
        this.Sprite.Update(false);
    }
}