using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class ByteReader
{
    private byte[] mBuffer;

    private int mOffset;

    public ByteReader(byte[] bytes)
    {
        this.mBuffer = bytes;
    }

    public ByteReader(TextAsset asset)
    {
        this.mBuffer = asset.bytes;
    }

    public bool canRead
    {
        get
        {
            return this.mBuffer != null && this.mOffset < this.mBuffer.Length;
        }
    }

    private static string ReadLine(byte[] buffer, int start, int count)
    {
        return Encoding.UTF8.GetString(buffer, start, count);
    }

    public Dictionary<string, string> ReadDictionary()
    {
        Dictionary<string, string> dictionary = new Dictionary<string, string>();
        char[] separator = new char[]
        {
            '='
        };
        while (this.canRead)
        {
            string text = this.ReadLine();
            if (text == null)
            {
                break;
            }
            if (!text.StartsWith("//"))
            {
                string[] array = text.Split(separator, 2, StringSplitOptions.RemoveEmptyEntries);
                if (array.Length == 2)
                {
                    string key = array[0].Trim();
                    string value = array[1].Trim().Replace("\\n", "\n");
                    dictionary[key] = value;
                }
            }
        }
        return dictionary;
    }

    public string ReadLine()
    {
        int num = this.mBuffer.Length;
        while (this.mOffset < num && this.mBuffer[this.mOffset] < 32)
        {
            this.mOffset++;
        }
        int i = this.mOffset;
        if (i < num)
        {
            while (i < num)
            {
                int num2 = (int)this.mBuffer[i++];
                if (num2 == 10 || num2 == 13)
                {
                    string result = ReadLine(this.mBuffer, this.mOffset, i - this.mOffset - 1);
                    this.mOffset = i;
                    return result;
                }
            }
            i++;
        }
        this.mOffset = num;
        return null;
    }
}