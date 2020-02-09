using System;
using System.Collections.Generic;
using ExitGames.Client.Photon.RpcProtocols.GpBinaryV16;

namespace ExitGames.Client.Photon
{
    public class Protocol
    {
        internal static readonly Dictionary<byte, CustomType> CodeDict = new Dictionary<byte, CustomType>();
        private static readonly byte[] memDeserialize = new byte[4];
        private static readonly float[] memFloatBlock = new float[1];
        private static IProtocol ProtocolDefault;
        internal static readonly Dictionary<Type, CustomType> TypeDict = new Dictionary<Type, CustomType>();

        public static bool TryRegisterType(Type type, byte typeCode, SerializeMethod serializeFunction, DeserializeMethod deserializeFunction)
        {
            if (CodeDict.ContainsKey(typeCode) || TypeDict.ContainsKey(type))
            {
                return false;
            }
            CustomType value = new CustomType(type, typeCode, serializeFunction, deserializeFunction);
            CodeDict.Add(typeCode, value);
            TypeDict.Add(type, value);
            return true;
        }

        public static bool TryRegisterType(Type type, byte typeCode, SerializeStreamMethod serializeFunction, DeserializeStreamMethod deserializeFunction)
        {
            if (CodeDict.ContainsKey(typeCode) || TypeDict.ContainsKey(type))
            {
                return false;
            }
            CustomType value = new CustomType(type, typeCode, serializeFunction, deserializeFunction);
            CodeDict.Add(typeCode, value);
            TypeDict.Add(type, value);
            return true;
        }

        [Obsolete]
        public static byte[] Serialize(object obj)
        {
            if (ProtocolDefault == null)
            {
                ProtocolDefault = new Protocol16();
            }
            lock (ProtocolDefault)
            {
                return ProtocolDefault.Serialize(obj);
            }
        }

        [Obsolete]
        public static object Deserialize(byte[] serializedData)
        {
            if (ProtocolDefault == null)
            {
                ProtocolDefault = new Protocol16();
            }
            lock (ProtocolDefault)
            {
                return ProtocolDefault.Deserialize(serializedData);
            }
        }

        public static void Serialize(short value, byte[] target, ref int targetOffset)
        {
            target[targetOffset++] = (byte)(value >> 8);
            target[targetOffset++] = (byte)value;
        }

        public static void Serialize(int value, byte[] target, ref int targetOffset)
        {
            target[targetOffset++] = (byte)(value >> 24);
            target[targetOffset++] = (byte)(value >> 16);
            target[targetOffset++] = (byte)(value >> 8);
            target[targetOffset++] = (byte)value;
        }

        public static void Serialize(float value, byte[] target, ref int targetOffset)
        {
            lock (memFloatBlock)
            {
                memFloatBlock[0] = value;
                Buffer.BlockCopy(memFloatBlock, 0, target, targetOffset, 4);
            }
            if (BitConverter.IsLittleEndian)
            {
                byte b = target[targetOffset];
                byte b2 = target[targetOffset + 1];
                target[targetOffset] = target[targetOffset + 3];
                target[targetOffset + 1] = target[targetOffset + 2];
                target[targetOffset + 2] = b2;
                target[targetOffset + 3] = b;
            }
            targetOffset += 4;
        }

        public static void Deserialize(out int value, byte[] source, ref int offset)
        {
            value = (source[offset++] << 24 | source[offset++] << 16 | source[offset++] << 8 | source[offset++]);
        }

        public static void Deserialize(out short value, byte[] source, ref int offset)
        {
            value = (short)(source[offset++] << 8 | source[offset++]);
        }

        public static void Deserialize(out float value, byte[] source, ref int offset)
        {
            if (BitConverter.IsLittleEndian)
            {
                lock (memDeserialize)
                {
                    byte[] array = memDeserialize;
                    array[3] = source[offset++];
                    array[2] = source[offset++];
                    array[1] = source[offset++];
                    array[0] = source[offset++];
                    value = BitConverter.ToSingle(array, 0);
                }
            }
            else
            {
                value = BitConverter.ToSingle(source, offset);
                offset += 4;
            }
        }
    }
}
