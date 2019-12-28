using System;

public static class CLZF2
{
    private static readonly long[] HashTable = new long[CLZF2.HSIZE];

    private static readonly uint HLOG = 14u;

    private static readonly uint HSIZE = 16384u;

    private static readonly uint MAX_LIT = 32u;

    private static readonly uint MAX_OFF = 8192u;

    private static readonly uint MAX_REF = 264u;

    public static byte[] Compress(byte[] inputBytes)
    {
        int num = inputBytes.Length * 2;
        byte[] src = new byte[num];
        int num2;
        for (num2 = CLZF2.lzf_compress(inputBytes, ref src); num2 == 0; num2 = CLZF2.lzf_compress(inputBytes, ref src))
        {
            num *= 2;
            src = new byte[num];
        }
        byte[] array = new byte[num2];
        Buffer.BlockCopy(src, 0, array, 0, num2);
        return array;
    }

    public static byte[] Decompress(byte[] inputBytes)
    {
        int num = inputBytes.Length * 2;
        byte[] src = new byte[num];
        int num2;
        for (num2 = CLZF2.lzf_decompress(inputBytes, ref src); num2 == 0; num2 = CLZF2.lzf_decompress(inputBytes, ref src))
        {
            num *= 2;
            src = new byte[num];
        }
        byte[] array = new byte[num2];
        Buffer.BlockCopy(src, 0, array, 0, num2);
        return array;
    }

    public static int lzf_compress(byte[] input, ref byte[] output)
    {
        int num = input.Length;
        int num2 = output.Length;
        Array.Clear(CLZF2.HashTable, 0, (int)CLZF2.HSIZE);
        uint num3 = 0u;
        uint num4 = 0u;
        uint num5 = (uint)((int)input[(int)((UIntPtr)num3)] << 8 | (int)input[(int)((UIntPtr)(num3 + 1u))]);
        int num6 = 0;
        for (; ; )
        {
            if ((ulong)num3 < (ulong)((long)(num - 2)))
            {
                num5 = (num5 << 8 | (uint)input[(int)((UIntPtr)(num3 + 2u))]);
                long num7 = (long)((ulong)((num5 ^ num5 << 5) >> (int)(24u - CLZF2.HLOG - num5 * 5u) & CLZF2.HSIZE - 1u));
                long num8 = CLZF2.HashTable[(int)(checked((IntPtr)num7))];
                CLZF2.HashTable[(int)(checked((IntPtr)num7))] = (long)((ulong)num3);
                long num9;
                if ((num9 = (long)((ulong)num3 - (ulong)num8 - 1UL)) < (long)((ulong)CLZF2.MAX_OFF) && (ulong)(num3 + 4u) < (ulong)((long)num) && num8 > 0L && input[(int)(checked((IntPtr)num8))] == input[(int)((UIntPtr)num3)] && input[(int)(checked((IntPtr)(unchecked(num8 + 1L))))] == input[(int)((UIntPtr)(num3 + 1u))] && input[(int)(checked((IntPtr)(unchecked(num8 + 2L))))] == input[(int)((UIntPtr)(num3 + 2u))])
                {
                    uint num10 = 2u;
                    uint num11 = (uint)(num - (int)num3 - (int)num10);
                    num11 = ((num11 <= CLZF2.MAX_REF) ? num11 : CLZF2.MAX_REF);
                    if ((ulong)num4 + (ulong)((long)num6) + 1UL + 3UL >= (ulong)((long)num2))
                    {
                        break;
                    }
                    do
                    {
                        num10 += 1u;
                    }
                    while (num10 < num11 && input[(int)(checked((IntPtr)(unchecked(num8 + (long)((ulong)num10)))))] == input[(int)((UIntPtr)(num3 + num10))]);
                    if (num6 != 0)
                    {
                        output[(int)((UIntPtr)(num4++))] = (byte)(num6 - 1);
                        num6 = -num6;
                        do
                        {
                            output[(int)((UIntPtr)(num4++))] = input[(int)(checked((IntPtr)(unchecked((ulong)num3 + (ulong)((long)num6)))))];
                        }
                        while (++num6 != 0);
                    }
                    num10 -= 2u;
                    num3 += 1u;
                    if (num10 < 7u)
                    {
                        output[(int)((UIntPtr)(num4++))] = (byte)((num9 >> 8) + (long)((ulong)((ulong)num10 << 5)));
                    }
                    else
                    {
                        output[(int)((UIntPtr)(num4++))] = (byte)((num9 >> 8) + 224L);
                        output[(int)((UIntPtr)(num4++))] = (byte)(num10 - 7u);
                    }
                    output[(int)((UIntPtr)(num4++))] = (byte)num9;
                    num3 += num10 - 1u;
                    num5 = (uint)((int)input[(int)((UIntPtr)num3)] << 8 | (int)input[(int)((UIntPtr)(num3 + 1u))]);
                    num5 = (num5 << 8 | (uint)input[(int)((UIntPtr)(num3 + 2u))]);
                    CLZF2.HashTable[(int)((UIntPtr)((num5 ^ num5 << 5) >> (int)(24u - CLZF2.HLOG - num5 * 5u) & CLZF2.HSIZE - 1u))] = (long)((ulong)num3);
                    num3 += 1u;
                    num5 = (num5 << 8 | (uint)input[(int)((UIntPtr)(num3 + 2u))]);
                    CLZF2.HashTable[(int)((UIntPtr)((num5 ^ num5 << 5) >> (int)(24u - CLZF2.HLOG - num5 * 5u) & CLZF2.HSIZE - 1u))] = (long)((ulong)num3);
                    num3 += 1u;
                    continue;
                }
            }
            else if ((ulong)num3 == (ulong)((long)num))
            {
                goto Block_13;
            }
            num6++;
            num3 += 1u;
            if ((long)num6 == (long)((ulong)CLZF2.MAX_LIT))
            {
                if ((ulong)(num4 + 1u + CLZF2.MAX_LIT) >= (ulong)((long)num2))
                {
                    return 0;
                }
                output[(int)((UIntPtr)(num4++))] = (byte)(CLZF2.MAX_LIT - 1u);
                num6 = -num6;
                do
                {
                    output[(int)((UIntPtr)(num4++))] = input[(int)(checked((IntPtr)(unchecked((ulong)num3 + (ulong)((long)num6)))))];
                }
                while (++num6 != 0);
            }
        }
        return 0;
        Block_13:
        if (num6 != 0)
        {
            if ((ulong)num4 + (ulong)((long)num6) + 1UL >= (ulong)((long)num2))
            {
                return 0;
            }
            output[(int)((UIntPtr)(num4++))] = (byte)(num6 - 1);
            num6 = -num6;
            do
            {
                output[(int)((UIntPtr)(num4++))] = input[(int)(checked((IntPtr)(unchecked((ulong)num3 + (ulong)((long)num6)))))];
            }
            while (++num6 != 0);
        }
        return (int)num4;
    }

    public static int lzf_decompress(byte[] input, ref byte[] output)
    {
        int num = input.Length;
        int num2 = output.Length;
        uint num3 = 0u;
        uint num4 = 0u;
        for (; ; )
        {
            uint num5 = (uint)input[(int)((UIntPtr)(num3++))];
            if (num5 < 32u)
            {
                num5 += 1u;
                if ((ulong)(num4 + num5) > (ulong)((long)num2))
                {
                    break;
                }
                do
                {
                    output[(int)((UIntPtr)(num4++))] = input[(int)((UIntPtr)(num3++))];
                }
                while ((num5 -= 1u) != 0u);
            }
            else
            {
                uint num6 = num5 >> 5;
                int num7 = (int)(num4 - ((num5 & 31u) << 8) - 1u);
                if (num6 == 7u)
                {
                    num6 += (uint)input[(int)((UIntPtr)(num3++))];
                }
                num7 -= (int)input[(int)((UIntPtr)(num3++))];
                if ((ulong)(num4 + num6 + 2u) > (ulong)((long)num2))
                {
                    return 0;
                }
                if (num7 < 0)
                {
                    return 0;
                }
                output[(int)((UIntPtr)(num4++))] = output[num7++];
                output[(int)((UIntPtr)(num4++))] = output[num7++];
                do
                {
                    output[(int)((UIntPtr)(num4++))] = output[num7++];
                }
                while ((num6 -= 1u) != 0u);
            }
            if ((ulong)num3 >= (ulong)((long)num))
            {
                return (int)num4;
            }
        }
        return 0;
    }
}