using System.Collections.Generic;
using UnityEngine;

public class PhotonStream
{
    private byte currentItem;

    private bool write;

    internal List<object> data;

    public PhotonStream(bool write, object[] incomingData)
    {
        this.write = write;
        if (incomingData == null)
        {
            this.data = new List<object>();
        }
        else
        {
            this.data = new List<object>(incomingData);
        }
    }

    public int Count
    {
        get
        {
            return this.data.Count;
        }
    }

    public bool isReading
    {
        get
        {
            return !this.write;
        }
    }

    public bool isWriting
    {
        get
        {
            return this.write;
        }
    }

    public object ReceiveNext()
    {
        if (this.write)
        {
            Debug.LogError("Error: you cannot read this stream that you are writing!");
            return null;
        }
        return this.data[currentItem++];
    }

    public void SendNext(object obj)
    {
        if (!this.write)
        {
            Debug.LogError("Error: you cannot write/send to this stream that you are reading!");
            return;
        }
        this.data.Add(obj);
    }

    public void Serialize(ref bool myBool)
    {
        if (this.write)
        {
            this.data.Add(myBool);
        }
        else if (this.data.Count > (int)this.currentItem)
        {
            myBool = (bool)this.data[(int)this.currentItem];
            this.currentItem += 1;
        }
    }

    public void Serialize(ref int myInt)
    {
        if (this.write)
        {
            this.data.Add(myInt);
        }
        else if (this.data.Count > (int)this.currentItem)
        {
            myInt = (int)this.data[(int)this.currentItem];
            this.currentItem += 1;
        }
    }

    public void Serialize(ref string value)
    {
        if (this.write)
        {
            this.data.Add(value);
        }
        else if (this.data.Count > (int)this.currentItem)
        {
            value = (string)this.data[(int)this.currentItem];
            this.currentItem += 1;
        }
    }

    public void Serialize(ref char value)
    {
        if (this.write)
        {
            this.data.Add(value);
        }
        else if (this.data.Count > (int)this.currentItem)
        {
            value = (char)this.data[(int)this.currentItem];
            this.currentItem += 1;
        }
    }

    public void Serialize(ref short value)
    {
        if (this.write)
        {
            this.data.Add(value);
        }
        else if (this.data.Count > (int)this.currentItem)
        {
            value = (short)this.data[(int)this.currentItem];
            this.currentItem += 1;
        }
    }

    public void Serialize(ref float obj)
    {
        if (this.write)
        {
            this.data.Add(obj);
        }
        else if (this.data.Count > (int)this.currentItem)
        {
            obj = (float)this.data[(int)this.currentItem];
            this.currentItem += 1;
        }
    }

    public void Serialize(ref PhotonPlayer obj)
    {
        if (this.write)
        {
            this.data.Add(obj);
        }
        else if (this.data.Count > (int)this.currentItem)
        {
            obj = (PhotonPlayer)this.data[(int)this.currentItem];
            this.currentItem += 1;
        }
    }

    public void Serialize(ref Vector3 obj)
    {
        if (this.write)
        {
            this.data.Add(obj);
        }
        else if (this.data.Count > (int)this.currentItem)
        {
            obj = (Vector3)this.data[(int)this.currentItem];
            this.currentItem += 1;
        }
    }

    public void Serialize(ref Vector2 obj)
    {
        if (this.write)
        {
            this.data.Add(obj);
        }
        else if (this.data.Count > (int)this.currentItem)
        {
            obj = (Vector2)this.data[(int)this.currentItem];
            this.currentItem += 1;
        }
    }

    public void Serialize(ref Quaternion obj)
    {
        if (this.write)
        {
            this.data.Add(obj);
        }
        else if (this.data.Count > (int)this.currentItem)
        {
            obj = (Quaternion)this.data[(int)this.currentItem];
            this.currentItem += 1;
        }
    }

    public object[] ToArray()
    {
        return this.data.ToArray();
    }
}