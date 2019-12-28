using Optimization.Caching;
using UnityEngine;

public class Emitter
{
    private float EmitDelayTime;
    private float EmitLoop;
    private float EmitterElapsedTime;
    private bool IsFirstEmit = true;
    private Vector3 LastClientPos = Vectors.zero;
    public EffectLayer Layer;

    public Emitter(EffectLayer owner)
    {
        this.Layer = owner;
        this.EmitLoop = (float)this.Layer.EmitLoop;
        this.LastClientPos = this.Layer.ClientTransform.position;
    }

    protected int EmitByDistance()
    {
        if ((this.Layer.ClientTransform.position - this.LastClientPos).magnitude >= this.Layer.DiffDistance)
        {
            this.LastClientPos = this.Layer.ClientTransform.position;
            return 1;
        }
        return 0;
    }

    protected int EmitByRate()
    {
        int num = UnityEngine.Random.Range(0, 100);
        if (num >= 0 && (float)num > this.Layer.ChanceToEmit)
        {
            return 0;
        }
        this.EmitDelayTime += Time.deltaTime;
        if (this.EmitDelayTime < this.Layer.EmitDelay && !this.IsFirstEmit)
        {
            return 0;
        }
        this.EmitterElapsedTime += Time.deltaTime;
        if (this.EmitterElapsedTime >= this.Layer.EmitDuration)
        {
            if (this.EmitLoop > 0f)
            {
                this.EmitLoop -= 1f;
            }
            this.EmitterElapsedTime = 0f;
            this.EmitDelayTime = 0f;
            this.IsFirstEmit = false;
        }
        if (this.EmitLoop == 0f)
        {
            return 0;
        }
        if (this.Layer.AvailableNodeCount == 0)
        {
            return 0;
        }
        int num2 = (int)(this.EmitterElapsedTime * (float)this.Layer.EmitRate) - (this.Layer.ActiveENodes.Length - this.Layer.AvailableNodeCount);
        int num3;
        if (num2 > this.Layer.AvailableNodeCount)
        {
            num3 = this.Layer.AvailableNodeCount;
        }
        else
        {
            num3 = num2;
        }
        if (num3 <= 0)
        {
            return 0;
        }
        return num3;
    }

    public Vector3 GetEmitRotation(EffectNode node)
    {
        Vector3 result = Vectors.zero;
        if (this.Layer.EmitType == 2)
        {
            if (!this.Layer.SyncClient)
            {
                result = node.Position - (this.Layer.ClientTransform.position + this.Layer.EmitPoint);
            }
            else
            {
                result = node.Position - this.Layer.EmitPoint;
            }
        }
        else if (this.Layer.EmitType == 3)
        {
            Vector3 vector;
            if (!this.Layer.SyncClient)
            {
                vector = node.Position - (this.Layer.ClientTransform.position + this.Layer.EmitPoint);
            }
            else
            {
                vector = node.Position - this.Layer.EmitPoint;
            }
            Vector3 toDirection = Vector3.RotateTowards(vector, this.Layer.CircleDir, (float)(90 - this.Layer.AngleAroundAxis) * 0.0174532924f, 1f);
            Quaternion rotation = Quaternion.FromToRotation(vector, toDirection);
            result = rotation * vector;
        }
        else if (this.Layer.IsRandomDir)
        {
            Quaternion rhs = Quaternion.Euler(0f, 0f, (float)this.Layer.AngleAroundAxis);
            Quaternion rhs2 = Quaternion.Euler(0f, (float)UnityEngine.Random.Range(0, 360), 0f);
            Quaternion lhs = Quaternion.FromToRotation(Vectors.up, this.Layer.OriVelocityAxis);
            result = lhs * rhs2 * rhs * Vectors.up;
        }
        else
        {
            result = this.Layer.OriVelocityAxis;
        }
        return result;
    }

    public int GetNodes()
    {
        if (this.Layer.IsEmitByDistance)
        {
            return this.EmitByDistance();
        }
        return this.EmitByRate();
    }

    public void Reset()
    {
        this.EmitterElapsedTime = 0f;
        this.EmitDelayTime = 0f;
        this.IsFirstEmit = true;
        this.EmitLoop = (float)this.Layer.EmitLoop;
    }

    public void SetEmitPosition(EffectNode node)
    {
        Vector3 vector = Vectors.zero;
        if (this.Layer.EmitType == 1)
        {
            Vector3 emitPoint = this.Layer.EmitPoint;
            float x = UnityEngine.Random.Range(emitPoint.x - this.Layer.BoxSize.x / 2f, emitPoint.x + this.Layer.BoxSize.x / 2f);
            float y = UnityEngine.Random.Range(emitPoint.y - this.Layer.BoxSize.y / 2f, emitPoint.y + this.Layer.BoxSize.y / 2f);
            float z = UnityEngine.Random.Range(emitPoint.z - this.Layer.BoxSize.z / 2f, emitPoint.z + this.Layer.BoxSize.z / 2f);
            vector.x = x;
            vector.y = y;
            vector.z = z;
            if (!this.Layer.SyncClient)
            {
                vector = this.Layer.ClientTransform.position + vector;
            }
        }
        else if (this.Layer.EmitType == 0)
        {
            vector = this.Layer.EmitPoint;
            if (!this.Layer.SyncClient)
            {
                vector = this.Layer.ClientTransform.position + this.Layer.EmitPoint;
            }
        }
        else if (this.Layer.EmitType == 2)
        {
            vector = this.Layer.EmitPoint;
            if (!this.Layer.SyncClient)
            {
                vector = this.Layer.ClientTransform.position + this.Layer.EmitPoint;
            }
            Vector3 point = Vectors.up * this.Layer.Radius;
            Quaternion rotation = Quaternion.Euler((float)UnityEngine.Random.Range(0, 360), (float)UnityEngine.Random.Range(0, 360), (float)UnityEngine.Random.Range(0, 360));
            vector = rotation * point + vector;
        }
        else if (this.Layer.EmitType == 4)
        {
            Vector3 vector2 = this.Layer.EmitPoint + this.Layer.ClientTransform.localRotation * Vectors.forward * this.Layer.LineLengthLeft;
            Vector3 a = this.Layer.EmitPoint + this.Layer.ClientTransform.localRotation * Vectors.forward * this.Layer.LineLengthRight;
            Vector3 vector3 = a - vector2;
            float num = (float)(node.Index + 1) / (float)this.Layer.MaxENodes;
            float d = vector3.magnitude * num;
            vector = vector2 + vector3.normalized * d;
            if (!this.Layer.SyncClient)
            {
                vector = this.Layer.ClientTransform.TransformPoint(vector);
            }
        }
        else if (this.Layer.EmitType == 3)
        {
            float num2 = (float)(node.Index + 1) / (float)this.Layer.MaxENodes;
            float y2 = 360f * num2;
            Quaternion rotation2 = Quaternion.Euler(0f, y2, 0f);
            Vector3 point2 = rotation2 * (Vectors.right * this.Layer.Radius);
            Quaternion rotation3 = Quaternion.FromToRotation(Vectors.up, this.Layer.CircleDir);
            vector = rotation3 * point2;
            if (!this.Layer.SyncClient)
            {
                vector = this.Layer.ClientTransform.position + vector + this.Layer.EmitPoint;
            }
            else
            {
                vector += this.Layer.EmitPoint;
            }
        }
        node.SetLocalPosition(vector);
    }
}