using System;
using UnityEngine;

public class RCRegion
{
    private float dimX;

    private float dimY;

    private float dimZ;

    public Vector3 location;

    public GameObject myBox;

    public RCRegion(Vector3 loc, float x, float y, float z)
    {
        this.location = loc;
        this.dimX = x;
        this.dimY = y;
        this.dimZ = z;
    }

    public float GetRandomX()
    {
        return this.location.x + UnityEngine.Random.Range(-this.dimX / 2f, this.dimX / 2f);
    }

    public float GetRandomY()
    {
        return this.location.y + UnityEngine.Random.Range(-this.dimY / 2f, this.dimY / 2f);
    }

    public float GetRandomZ()
    {
        return this.location.z + UnityEngine.Random.Range(-this.dimZ / 2f, this.dimZ / 2f);
    }
}
