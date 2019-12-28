using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;

namespace ExitGames.Client.Photon
{
    public class Protocol18 : IProtocol
    {
        public enum GpType : byte
        {
            Unknown,
            Boolean = 2,
            Byte,
            Short,
            Float,
            Double,
            String,
            Null,
            CompressedInt,
            CompressedLong,
            Int1,
            Int1_,
            Int2,
            Int2_,
            L1,
            L1_,
            L2,
            L2_,
            Custom,
            CustomTypeSlim = 0x80,
            Dictionary = 20,
            Hashtable,
            ObjectArray = 23,
            OperationRequest,
            OperationResponse,
            EventData,
            BooleanFalse,
            BooleanTrue,
            ShortZero,
            IntZero,
            LongZero,
            FloatZero,
            DoubleZero,
            ByteZero,
            Array = 0x40,
            BooleanArray = 66,
            ByteArray,
            ShortArray,
            DoubleArray = 70,
            FloatArray = 69,
            StringArray = 71,
            HashtableArray = 85,
            DictionaryArray = 84,
            CustomTypeArray = 83,
            CompressedIntArray = 73,
            CompressedLongArray
        }

        private readonly byte[] versionBytes = new byte[2]
        {
        1,
        8
        };

        private readonly byte[] memDouble = new byte[8];

        private static readonly byte[] boolMasks = new byte[8]
        {
        1,
        2,
        4,
        8,
        16,
        32,
        64,
        128
        };

        private readonly double[] memDoubleBlock = new double[1];

        private readonly float[] memFloatBlock = new float[1];

        private readonly byte[] memFloat = new byte[4];

        private readonly byte[] memCustomTypeBodyLengthSerialized = new byte[5];

        private readonly byte[] memCompressedUInt32 = new byte[5];

        private byte[] memCompressedUInt64 = new byte[10];

        public override string ProtocolType
        {
            get
            {
                return "GpBinaryV18";
            }
        }

        public override byte[] VersionBytes
        {
            get
            {
                return this.versionBytes;
            }
        }

        public override void Serialize(StreamBuffer dout, object serObject, bool setType)
        {
            this.Write(dout, serObject, setType);
        }

        public override void SerializeShort(StreamBuffer dout, short serObject, bool setType)
        {
            this.WriteInt16(dout, serObject, setType);
        }

        public override void SerializeString(StreamBuffer dout, string serObject, bool setType)
        {
            this.WriteString(dout, serObject, setType);
        }

        public override object Deserialize(StreamBuffer din, byte type)
        {
            return this.Read(din, type);
        }

        public override short DeserializeShort(StreamBuffer din)
        {
            return this.ReadInt16(din);
        }

        public override byte DeserializeByte(StreamBuffer din)
        {
            return this.ReadByte(din);
        }

        private static Type GetAllowedDictionaryKeyTypes(GpType gpType)
        {
            switch (gpType)
            {
                case GpType.Byte:
                case GpType.ByteZero:
                    return typeof(byte);
                case GpType.Short:
                case GpType.ShortZero:
                    return typeof(short);
                case GpType.Float:
                case GpType.FloatZero:
                    return typeof(float);
                case GpType.Double:
                case GpType.DoubleZero:
                    return typeof(double);
                case GpType.String:
                    return typeof(string);
                case GpType.CompressedInt:
                case GpType.Int1:
                case GpType.Int1_:
                case GpType.Int2:
                case GpType.Int2_:
                case GpType.IntZero:
                    return typeof(int);
                case GpType.CompressedLong:
                case GpType.L1:
                case GpType.L1_:
                case GpType.L2:
                case GpType.L2_:
                case GpType.LongZero:
                    return typeof(long);
                default:
                    throw new Exception(string.Format("{0} is not a valid value type.", gpType));
            }
        }

        private static Type GetClrArrayType(GpType gpType)
        {
            switch (gpType)
            {
                case GpType.Boolean:
                case GpType.BooleanFalse:
                case GpType.BooleanTrue:
                    return typeof(bool);
                case GpType.Byte:
                case GpType.ByteZero:
                    return typeof(byte);
                case GpType.Short:
                case GpType.ShortZero:
                    return typeof(short);
                case GpType.Float:
                case GpType.FloatZero:
                    return typeof(float);
                case GpType.Double:
                case GpType.DoubleZero:
                    return typeof(double);
                case GpType.String:
                    return typeof(string);
                case GpType.CompressedInt:
                case GpType.Int1:
                case GpType.Int1_:
                case GpType.Int2:
                case GpType.Int2_:
                case GpType.IntZero:
                    return typeof(int);
                case GpType.CompressedLong:
                case GpType.L1:
                case GpType.L1_:
                case GpType.L2:
                case GpType.L2_:
                case GpType.LongZero:
                    return typeof(long);
                case GpType.Hashtable:
                    return typeof(ExitGames.Client.Photon.Hashtable);
                case GpType.OperationRequest:
                    return typeof(OperationRequest);
                case GpType.OperationResponse:
                    return typeof(OperationResponse);
                case GpType.EventData:
                    return typeof(EventData);
                case GpType.BooleanArray:
                    return typeof(bool[]);
                case GpType.ByteArray:
                    return typeof(byte[]);
                case GpType.ShortArray:
                    return typeof(short[]);
                case GpType.DoubleArray:
                    return typeof(double[]);
                case GpType.FloatArray:
                    return typeof(float[]);
                case GpType.StringArray:
                    return typeof(string[]);
                case GpType.HashtableArray:
                    return typeof(ExitGames.Client.Photon.Hashtable[]);
                case GpType.CompressedIntArray:
                    return typeof(int[]);
                case GpType.CompressedLongArray:
                    return typeof(long[]);
                default:
                    return null;
            }
        }

        private GpType GetCodeOfType(Type type)
        {
            if (type == null)
            {
                return GpType.Null;
            }
            if (type.IsPrimitive || type.IsEnum)
            {
                TypeCode typeCode = Type.GetTypeCode(type);
                return this.GetCodeOfTypeCode(typeCode);
            }
            if (type == typeof(string))
            {
                return GpType.String;
            }
            if (type.IsArray)
            {
                Type elementType = type.GetElementType();
                if (elementType == null)
                {
                    throw new ExitGames.Client.Photon.InvalidDataException(string.Format("Arrays of type {0} are not supported", type));
                }
                if (elementType.IsPrimitive)
                {
                    switch (Type.GetTypeCode(elementType))
                    {
                        case TypeCode.Byte:
                            return GpType.ByteArray;
                        case TypeCode.Int16:
                            return GpType.ShortArray;
                        case TypeCode.Int32:
                            return GpType.CompressedIntArray;
                        case TypeCode.Int64:
                            return GpType.CompressedLongArray;
                        case TypeCode.Boolean:
                            return GpType.BooleanArray;
                        case TypeCode.Single:
                            return GpType.FloatArray;
                        case TypeCode.Double:
                            return GpType.DoubleArray;
                    }
                }
                if (elementType.IsArray)
                {
                    return GpType.Array;
                }
                if (elementType == typeof(string))
                {
                    return GpType.StringArray;
                }
                if (elementType == typeof(object))
                {
                    return GpType.ObjectArray;
                }
                if (elementType == typeof(ExitGames.Client.Photon.Hashtable))
                {
                    return GpType.HashtableArray;
                }
                if (elementType.IsGenericType)
                {
                    return GpType.DictionaryArray;
                }
                return GpType.CustomTypeArray;
            }
            if (type == typeof(ExitGames.Client.Photon.Hashtable))
            {
                return GpType.Hashtable;
            }
            if (type == typeof(List<object>))
            {
                return GpType.ObjectArray;
            }
            if (type.IsGenericType && typeof(Dictionary<,>) == type.GetGenericTypeDefinition())
            {
                return GpType.Dictionary;
            }
            if (type == typeof(EventData))
            {
                return GpType.EventData;
            }
            if (type == typeof(OperationRequest))
            {
                return GpType.OperationRequest;
            }
            if (type == typeof(OperationResponse))
            {
                return GpType.OperationResponse;
            }
            return GpType.Unknown;
        }

        private GpType GetCodeOfTypeCode(TypeCode type)
        {
            switch (type)
            {
                case TypeCode.Byte:
                    return GpType.Byte;
                case TypeCode.String:
                    return GpType.String;
                case TypeCode.Boolean:
                    return GpType.Boolean;
                case TypeCode.Int16:
                    return GpType.Short;
                case TypeCode.Int32:
                    return GpType.CompressedInt;
                case TypeCode.Int64:
                    return GpType.CompressedLong;
                case TypeCode.Single:
                    return GpType.Float;
                case TypeCode.Double:
                    return GpType.Double;
                default:
                    return GpType.Unknown;
            }
        }

        private object Read(StreamBuffer stream)
        {
            return this.Read(stream, this.ReadByte(stream));
        }

        private object Read(StreamBuffer stream, byte gpType)
        {
            if (gpType >= 128 && gpType <= 228)
            {
                return this.ReadCustomType(stream, gpType);
            }
            switch (gpType)
            {
                case 2:
                    return this.ReadBoolean(stream);
                case 28:
                    return true;
                case 27:
                    return false;
                case 3:
                    return this.ReadByte(stream);
                case 34:
                    return (byte)0;
                case 4:
                    return this.ReadInt16(stream);
                case 29:
                    return (short)0;
                case 5:
                    return this.ReadSingle(stream);
                case 32:
                    return 0f;
                case 6:
                    return this.ReadDouble(stream);
                case 33:
                    return 0.0;
                case 7:
                    return this.ReadString(stream);
                case 11:
                    return this.ReadInt1(stream, false);
                case 13:
                    return this.ReadInt2(stream, false);
                case 12:
                    return this.ReadInt1(stream, true);
                case 14:
                    return this.ReadInt2(stream, true);
                case 9:
                    return this.ReadCompressedInt32(stream);
                case 30:
                    return 0;
                case 15:
                    return (long)this.ReadInt1(stream, false);
                case 17:
                    return (long)this.ReadInt2(stream, false);
                case 16:
                    return (long)this.ReadInt1(stream, true);
                case 18:
                    return (long)this.ReadInt2(stream, true);
                case 10:
                    return this.ReadCompressedInt64(stream);
                case 31:
                    return 0L;
                case 21:
                    return this.ReadHashtable(stream);
                case 20:
                    return this.ReadDictionary(stream);
                case 19:
                    return this.ReadCustomType(stream, 0);
                case 24:
                    return this.DeserializeOperationRequest(stream);
                case 25:
                    return this.DeserializeOperationResponse(stream);
                case 26:
                    return this.DeserializeEventData(stream, null);
                case 23:
                    return this.ReadObjectArray(stream);
                case 66:
                    return this.ReadBooleanArray(stream);
                case 67:
                    return this.ReadByteArray(stream);
                case 68:
                    return this.ReadInt16Array(stream);
                case 70:
                    return this.ReadDoubleArray(stream);
                case 69:
                    return this.ReadSingleArray(stream);
                case 71:
                    return this.ReadStringArray(stream);
                case 85:
                    return this.ReadHashtableArray(stream);
                case 84:
                    return this.ReadDictionaryArray(stream);
                case 83:
                    return this.ReadCustomTypeArray(stream);
                case 73:
                    return this.ReadCompressedInt32Array(stream);
                case 74:
                    return this.ReadCompressedInt64Array(stream);
                case 64:
                    return this.ReadArrayInArray(stream);
                default:
                    return null;
            }
        }

        internal bool ReadBoolean(StreamBuffer stream)
        {
            return stream.ReadByte() > 0;
        }

        internal byte ReadByte(StreamBuffer stream)
        {
            return stream.ReadByte();
        }

        internal short ReadInt16(StreamBuffer stream)
        {
            int num = default(int);
            byte[] bufferAndAdvance = stream.GetBufferAndAdvance(2, out num);
            return (short)(bufferAndAdvance[num++] | bufferAndAdvance[num] << 8);
        }

        internal ushort ReadUShort(StreamBuffer stream)
        {
            int num = default(int);
            byte[] bufferAndAdvance = stream.GetBufferAndAdvance(2, out num);
            return (ushort)(bufferAndAdvance[num++] | bufferAndAdvance[num] << 8);
        }

        internal int ReadInt32(StreamBuffer stream)
        {
            int num = default(int);
            byte[] bufferAndAdvance = stream.GetBufferAndAdvance(4, out num);
            return bufferAndAdvance[num++] << 24 | bufferAndAdvance[num++] << 16 | bufferAndAdvance[num++] << 8 | bufferAndAdvance[num];
        }

        internal long ReadInt64(StreamBuffer stream)
        {
            int num = default(int);
            byte[] bufferAndAdvance = stream.GetBufferAndAdvance(4, out num);
            return (long)((ulong)bufferAndAdvance[num++] << 56 | (ulong)bufferAndAdvance[num++] << 48 | (ulong)bufferAndAdvance[num++] << 40 | (ulong)bufferAndAdvance[num++] << 32 | (ulong)bufferAndAdvance[num++] << 24 | (ulong)bufferAndAdvance[num++] << 16 | (ulong)bufferAndAdvance[num++] << 8 | bufferAndAdvance[num]);
        }

        internal float ReadSingle(StreamBuffer stream)
        {
            int num = 0;
            byte[] bufferAndAdvance = stream.GetBufferAndAdvance(4, out num);
            return BitConverter.ToSingle(bufferAndAdvance, num);
        }

        internal double ReadDouble(StreamBuffer stream)
        {
            int num = default(int);
            byte[] bufferAndAdvance = stream.GetBufferAndAdvance(8, out num);
            return BitConverter.ToDouble(bufferAndAdvance, num);
        }

        internal byte[] ReadByteArray(StreamBuffer stream)
        {
            uint num = this.ReadCompressedUInt32(stream);
            byte[] array = new byte[num];
            stream.Read(array, 0, (int)num);
            return array;
        }

        public object ReadCustomType(StreamBuffer stream, byte gpType = 0)
        {
            byte b = 0;
            b = ((gpType != 0) ? ((byte)(gpType - 128)) : stream.ReadByte());
            CustomType customType = default(CustomType);
            if (!Protocol.CodeDict.TryGetValue(b, out customType))
            {
                throw new Exception("Read failed. Custom type not found: " + b);
            }
            int num = (int)this.ReadCompressedUInt32(stream);
            if (customType.SerializeStreamFunction == null)
            {
                byte[] array = new byte[num];
                stream.Read(array, 0, num);
                return customType.DeserializeFunction(array);
            }
            return customType.DeserializeStreamFunction(stream, (short)num);
        }

        public override EventData DeserializeEventData(StreamBuffer din, EventData target = null)
        {
            EventData eventData;
            if (target != null)
            {
                target.Reset();
                eventData = target;
            }
            else
            {
                eventData = new EventData();
            }
            eventData.Code = this.ReadByte(din);
            eventData.Parameters = this.ReadParameterTable(din, eventData.Parameters);
            return eventData;
        }

        private Dictionary<byte, object> ReadParameterTable(StreamBuffer stream, Dictionary<byte, object> target = null)
        {
            short num = this.ReadByte(stream);
            Dictionary<byte, object> dictionary = (target != null) ? target : new Dictionary<byte, object>(num);
            for (int i = 0; i < num; i++)
            {
                byte key = stream.ReadByte();
                object obj2 = dictionary[key] = this.Read(stream, stream.ReadByte());
            }
            return dictionary;
        }

        public ExitGames.Client.Photon.Hashtable ReadHashtable(StreamBuffer stream)
        {
            int num = (int)this.ReadCompressedUInt32(stream);
            ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable(num);
            for (int i = 0; i < num; i++)
            {
                object key = this.Read(stream);
                object obj2 = hashtable[key] = this.Read(stream);
            }
            return hashtable;
        }

        public int[] ReadIntArray(StreamBuffer stream)
        {
            int num = this.ReadInt32(stream);
            int[] array = new int[num];
            for (int i = 0; i < num; i++)
            {
                array[i] = this.ReadInt32(stream);
            }
            return array;
        }

        public override OperationRequest DeserializeOperationRequest(StreamBuffer din)
        {
            OperationRequest operationRequest = new OperationRequest();
            operationRequest.OperationCode = this.ReadByte(din);
            operationRequest.Parameters = this.ReadParameterTable(din, null);
            return operationRequest;
        }

        public override OperationResponse DeserializeOperationResponse(StreamBuffer stream)
        {
            OperationResponse operationResponse = new OperationResponse();
            operationResponse.OperationCode = this.ReadByte(stream);
            operationResponse.ReturnCode = this.ReadInt16(stream);
            operationResponse.DebugMessage = (this.Read(stream, this.ReadByte(stream)) as string);
            operationResponse.Parameters = this.ReadParameterTable(stream, null);
            return operationResponse;
        }

        internal string ReadString(StreamBuffer stream)
        {
            int num = (int)this.ReadCompressedUInt32(stream);
            if (num == 0)
            {
                return string.Empty;
            }
            int index = 0;
            byte[] bufferAndAdvance = stream.GetBufferAndAdvance(num, out index);
            return Encoding.UTF8.GetString(bufferAndAdvance, index, num);
        }

        private object ReadCustomTypeArray(StreamBuffer stream)
        {
            uint num = this.ReadCompressedUInt32(stream);
            byte b = stream.ReadByte();
            CustomType customType = default(CustomType);
            if (!Protocol.CodeDict.TryGetValue(b, out customType))
            {
                throw new Exception("Serialization failed. Custom type not found: " + b);
            }
            Array array = Array.CreateInstance(customType.Type, (int)num);
            for (short num2 = 0; num2 < num; num2 = (short)(num2 + 1))
            {
                uint num3 = this.ReadCompressedUInt32(stream);
                object value;
                if (customType.SerializeStreamFunction == null)
                {
                    byte[] array2 = new byte[num3];
                    stream.Read(array2, 0, (int)num3);
                    value = customType.DeserializeFunction(array2);
                }
                else
                {
                    value = customType.DeserializeStreamFunction(stream, (short)num3);
                }
                array.SetValue(value, num2);
            }
            return array;
        }

        private Type ReadDictionaryType(StreamBuffer stream, out GpType keyReadType, out GpType valueReadType)
        {
            keyReadType = (GpType)stream.ReadByte();
            GpType gpType = (GpType)stream.ReadByte();
            valueReadType = gpType;
            Type type = (keyReadType != 0) ? Protocol18.GetAllowedDictionaryKeyTypes(keyReadType) : typeof(object);
            Type type2;
            switch (gpType)
            {
                case GpType.Unknown:
                    type2 = typeof(object);
                    break;
                case GpType.Dictionary:
                    type2 = this.ReadDictionaryType(stream);
                    break;
                case GpType.Array:
                    type2 = this.GetDictArrayType(stream);
                    valueReadType = GpType.Unknown;
                    break;
                case GpType.ObjectArray:
                    type2 = typeof(object[]);
                    break;
                case GpType.HashtableArray:
                    type2 = typeof(ExitGames.Client.Photon.Hashtable[]);
                    break;
                default:
                    type2 = Protocol18.GetClrArrayType(gpType);
                    break;
            }
            return typeof(Dictionary<,>).MakeGenericType(type, type2);
        }

        private Type ReadDictionaryType(StreamBuffer stream)
        {
            GpType gpType = (GpType)stream.ReadByte();
            GpType gpType2 = (GpType)stream.ReadByte();
            Type type = (gpType != 0) ? Protocol18.GetAllowedDictionaryKeyTypes(gpType) : typeof(object);
            Type type2;
            switch (gpType2)
            {
                case GpType.Unknown:
                    type2 = typeof(object);
                    break;
                case GpType.Dictionary:
                    type2 = this.ReadDictionaryType(stream);
                    break;
                case GpType.Array:
                    type2 = this.GetDictArrayType(stream);
                    break;
                default:
                    type2 = Protocol18.GetClrArrayType(gpType2);
                    break;
            }
            return typeof(Dictionary<,>).MakeGenericType(type, type2);
        }

        private Type GetDictArrayType(StreamBuffer stream)
        {
            GpType gpType = (GpType)stream.ReadByte();
            int num = 0;
            while (gpType == GpType.Array)
            {
                num++;
                gpType = (GpType)stream.ReadByte();
            }
            Type clrArrayType = Protocol18.GetClrArrayType(gpType);
            Type type = clrArrayType.MakeArrayType();
            for (int i = 0; i < num; i++)
            {
                type = type.MakeArrayType();
            }
            return type;
        }

        private IDictionary ReadDictionary(StreamBuffer stream)
        {
            GpType keyReadType = default(GpType);
            GpType valueReadType = default(GpType);
            Type type = this.ReadDictionaryType(stream, out keyReadType, out valueReadType);
            if (type == null)
            {
                return null;
            }
            IDictionary dictionary = Activator.CreateInstance(type) as IDictionary;
            if (dictionary == null)
            {
                return null;
            }
            this.ReadDictionaryElements(stream, keyReadType, valueReadType, dictionary);
            return dictionary;
        }

        private bool ReadDictionaryElements(StreamBuffer stream, GpType keyReadType, GpType valueReadType, IDictionary dictionary)
        {
            uint num = this.ReadCompressedUInt32(stream);
            for (int i = 0; i < num; i++)
            {
                object key = (keyReadType == GpType.Unknown) ? this.Read(stream) : this.Read(stream, (byte)keyReadType);
                object value = (valueReadType == GpType.Unknown) ? this.Read(stream) : this.Read(stream, (byte)valueReadType);
                dictionary.Add(key, value);
            }
            return true;
        }

        private object[] ReadObjectArray(StreamBuffer stream)
        {
            uint num = this.ReadCompressedUInt32(stream);
            object[] array = new object[num];
            for (short num2 = 0; num2 < num; num2 = (short)(num2 + 1))
            {
                object obj = array[num2] = this.Read(stream);
            }
            return array;
        }

        private bool[] ReadBooleanArray(StreamBuffer stream)
        {
            uint num = this.ReadCompressedUInt32(stream);
            bool[] array = new bool[num];
            int num2 = (int)num / 8;
            int num3 = 0;
            while (num2 > 0)
            {
                byte b = stream.ReadByte();
                array[num3++] = ((b & 1) == 1);
                array[num3++] = ((b & 2) == 2);
                array[num3++] = ((b & 4) == 4);
                array[num3++] = ((b & 8) == 8);
                array[num3++] = ((b & 0x10) == 16);
                array[num3++] = ((b & 0x20) == 32);
                array[num3++] = ((b & 0x40) == 64);
                array[num3++] = ((b & 0x80) == 128);
                num2--;
            }
            if (num3 < num)
            {
                byte b2 = stream.ReadByte();
                int num12 = 0;
                while (num3 < num)
                {
                    array[num3++] = ((b2 & Protocol18.boolMasks[num12]) == Protocol18.boolMasks[num12]);
                    num12++;
                }
            }
            return array;
        }

        internal short[] ReadInt16Array(StreamBuffer stream)
        {
            uint num = this.ReadCompressedUInt32(stream);
            short[] array = new short[num];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = this.ReadInt16(stream);
            }
            return array;
        }


        private double[] ReadDoubleArray(StreamBuffer stream)
        {
            int num = (int)this.ReadCompressedUInt32(stream);
            byte[] array = new byte[num * 8];
            double[] array2 = new double[num];
            stream.Read(array, 0, num * 8);
            for (int i = 0; i < num; i++)
            {
                array2[i] = ReadDouble(stream);
            }
            return array2;
        }

        private float[] ReadSingleArray(StreamBuffer stream)
        {
            int num = (int)this.ReadCompressedUInt32(stream);
            byte[] array = new byte[num * 4];
            float[] array2 = new float[num];
            stream.Read(array, 0, num * 4);
            for (int i = 0; i < num; i++)
            {
                array2[i] = ReadSingle(stream);
            }
            return array2;
        }

        internal string[] ReadStringArray(StreamBuffer stream)
        {
            uint num = this.ReadCompressedUInt32(stream);
            string[] array = new string[num];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = this.ReadString(stream);
            }
            return array;
        }

        private ExitGames.Client.Photon.Hashtable[] ReadHashtableArray(StreamBuffer stream)
        {
            uint num = this.ReadCompressedUInt32(stream);
            ExitGames.Client.Photon.Hashtable[] array = new ExitGames.Client.Photon.Hashtable[num];
            for (int i = 0; i < num; i++)
            {
                array[i] = this.ReadHashtable(stream);
            }
            return array;
        }

        private IDictionary[] ReadDictionaryArray(StreamBuffer stream)
        {
            GpType keyReadType = default(GpType);
            GpType valueReadType = default(GpType);
            Type type = this.ReadDictionaryType(stream, out keyReadType, out valueReadType);
            uint num = this.ReadCompressedUInt32(stream);
            IDictionary[] array = (IDictionary[])Array.CreateInstance(type, (int)num);
            for (int i = 0; i < num; i++)
            {
                array[i] = (IDictionary)Activator.CreateInstance(type);
                this.ReadDictionaryElements(stream, keyReadType, valueReadType, array[i]);
            }
            return array;
        }

        private Array ReadArrayInArray(StreamBuffer stream)
        {
            uint num = this.ReadCompressedUInt32(stream);
            object obj = this.Read(stream);
            Array array = obj as Array;
            if (array != null)
            {
                Type type = array.GetType();
                Array array2 = Array.CreateInstance(type, (int)num);
                array2.SetValue(array, 0);
                for (short num2 = 1; num2 < num; num2 = (short)(num2 + 1))
                {
                    array = (Array)this.Read(stream);
                    array2.SetValue(array, num2);
                }
                return array2;
            }
            return null;
        }

        internal int ReadInt1(StreamBuffer stream, bool signNegative)
        {
            if (signNegative)
            {
                return -stream.ReadByte();
            }
            return stream.ReadByte();
        }

        internal int ReadInt2(StreamBuffer stream, bool signNegative)
        {
            if (signNegative)
            {
                return -this.ReadUShort(stream);
            }
            return this.ReadUShort(stream);
        }

        internal int ReadCompressedInt32(StreamBuffer stream)
        {
            uint value = this.ReadCompressedUInt32(stream);
            return this.DecodeZigZag32(value);
        }

        private uint ReadCompressedUInt32(StreamBuffer stream)
        {
            uint num = 0u;
            int num2 = 0;
            byte[] buffer = stream.GetBuffer();
            int num3 = stream.Position;
            while (num2 != 35)
            {
                if (num3 >= buffer.Length)
                {
                    throw new EndOfStreamException("Failed to read full uint.");
                }
                byte b = buffer[num3];
                num3++;
                num = (uint)((int)num | (b & 0x7F) << num2);
                num2 += 7;
                if ((b & 0x80) == 0)
                {
                    break;
                }
            }
            stream.Position = num3;
            return num;
        }

        internal long ReadCompressedInt64(StreamBuffer stream)
        {
            ulong value = this.ReadCompressedUInt64(stream);
            return this.DecodeZigZag64(value);
        }

        private ulong ReadCompressedUInt64(StreamBuffer stream)
        {
            ulong num = 0uL;
            int num2 = 0;
            byte[] buffer = stream.GetBuffer();
            int num3 = stream.Position;
            while (num2 != 70)
            {
                if (num3 >= buffer.Length)
                {
                    throw new EndOfStreamException("Failed to read full ulong.");
                }
                byte b = buffer[num3];
                num3++;
                num = (ulong)((long)num | (long)(b & 0x7F) << num2);
                num2 += 7;
                if ((b & 0x80) == 0)
                {
                    break;
                }
            }
            stream.Position = num3;
            return num;
        }

        internal int[] ReadCompressedInt32Array(StreamBuffer stream)
        {
            uint num = this.ReadCompressedUInt32(stream);
            int[] array = new int[num];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = this.ReadCompressedInt32(stream);
            }
            return array;
        }

        internal long[] ReadCompressedInt64Array(StreamBuffer stream)
        {
            uint num = this.ReadCompressedUInt32(stream);
            long[] array = new long[num];
            for (int i = 0; i < array.Length; i++)
            {
                array[i] = this.ReadCompressedInt64(stream);
            }
            return array;
        }

        private int DecodeZigZag32(uint value)
        {
            return (int)(value >> 1 ^ 0L - (value & 1));
        }

        private long DecodeZigZag64(ulong value)
        {
            return (long)(value >> 1 ^ 0L - (value & 1));
        }

        internal void Write(StreamBuffer stream, object value, bool writeType)
        {
            if (value == null)
            {
                this.Write(stream, value, GpType.Null, writeType);
            }
            else
            {
                this.Write(stream, value, this.GetCodeOfType(value.GetType()), writeType);
            }
        }

        private void Write(StreamBuffer stream, object value, GpType gpType, bool writeType)
        {
            switch (gpType)
            {
                case GpType.Unknown:
                    if (value is ArraySegment<byte>)
                    {
                        ArraySegment<byte> arraySegment = (ArraySegment<byte>)value;
                        this.WriteByteArraySegment(stream, arraySegment.Array, arraySegment.Offset, arraySegment.Count, writeType);
                        break;
                    }
                    goto case GpType.Custom;
                case GpType.Custom:
                    this.WriteCustomType(stream, value, writeType);
                    break;
                case GpType.CustomTypeArray:
                    this.WriteCustomTypeArray(stream, value, writeType);
                    break;
                case GpType.Array:
                    this.WriteArrayInArray(stream, value, writeType);
                    break;
                case GpType.CompressedInt:
                    this.WriteCompressedInt32(stream, (int)value, writeType);
                    break;
                case GpType.CompressedLong:
                    this.WriteCompressedInt64(stream, (long)value, writeType);
                    break;
                case GpType.Dictionary:
                    this.WriteDictionary(stream, (IDictionary)value, writeType);
                    break;
                case GpType.Byte:
                    this.WriteByte(stream, (byte)value, writeType);
                    break;
                case GpType.Double:
                    this.WriteDouble(stream, (double)value, writeType);
                    break;
                case GpType.EventData:
                    this.SerializeEventData(stream, (EventData)value, writeType);
                    break;
                case GpType.Float:
                    this.WriteSingle(stream, (float)value, writeType);
                    break;
                case GpType.Hashtable:
                    this.WriteHashtable(stream, (ExitGames.Client.Photon.Hashtable)value, writeType);
                    break;
                case GpType.Short:
                    this.WriteInt16(stream, (short)value, writeType);
                    break;
                case GpType.CompressedIntArray:
                    this.WriteInt32ArrayCompressed(stream, (int[])value, writeType);
                    break;
                case GpType.CompressedLongArray:
                    this.WriteInt64ArrayCompressed(stream, (long[])value, writeType);
                    break;
                case GpType.Boolean:
                    this.WriteBoolean(stream, (bool)value, writeType);
                    break;
                case GpType.OperationResponse:
                    this.SerializeOperationResponse(stream, (OperationResponse)value, writeType);
                    break;
                case GpType.OperationRequest:
                    this.SerializeOperationRequest(stream, (OperationRequest)value, writeType);
                    break;
                case GpType.String:
                    this.WriteString(stream, (string)value, writeType);
                    break;
                case GpType.ByteArray:
                    this.WriteByteArray(stream, (byte[])value, writeType);
                    break;
                case GpType.ObjectArray:
                    this.WriteObjectArray(stream, (IList)value, writeType);
                    break;
                case GpType.DictionaryArray:
                    this.WriteDictionaryArray(stream, (IDictionary[])value, writeType);
                    break;
                case GpType.DoubleArray:
                    this.WriteDoubleArray(stream, (double[])value, writeType);
                    break;
                case GpType.FloatArray:
                    this.WriteSingleArray(stream, (float[])value, writeType);
                    break;
                case GpType.HashtableArray:
                    this.WriteHashtableArray(stream, value, writeType);
                    break;
                case GpType.ShortArray:
                    this.WriteInt16Array(stream, (short[])value, writeType);
                    break;
                case GpType.BooleanArray:
                    this.WriteBoolArray(stream, (bool[])value, writeType);
                    break;
                case GpType.StringArray:
                    this.WriteStringArray(stream, value, writeType);
                    break;
                case GpType.Null:
                    if (writeType)
                    {
                        stream.WriteByte(8);
                    }
                    break;
            }
        }

        public override void SerializeEventData(StreamBuffer stream, EventData serObject, bool setType)
        {
            if (setType)
            {
                stream.WriteByte(26);
            }
            stream.WriteByte(serObject.Code);
            this.WriteParameterTable(stream, serObject.Parameters);
        }

        private void WriteParameterTable(StreamBuffer stream, Dictionary<byte, object> parameters)
        {
            if (parameters == null || parameters.Count == 0)
            {
                this.WriteByte(stream, 0, false);
            }
            else
            {
                this.WriteByte(stream, (byte)parameters.Count, false);
                foreach (KeyValuePair<byte, object> parameter in parameters)
                {
                    stream.WriteByte(parameter.Key);
                    this.Write(stream, parameter.Value, true);
                }
            }
        }

        private void SerializeOperationRequest(StreamBuffer stream, OperationRequest serObject, bool setType)
        {
            this.SerializeOperationRequest(stream, serObject.OperationCode, serObject.Parameters, setType);
        }

        public override void SerializeOperationRequest(StreamBuffer stream, byte operationCode, Dictionary<byte, object> parameters, bool setType)
        {
            if (setType)
            {
                stream.WriteByte(24);
            }
            stream.WriteByte(operationCode);
            this.WriteParameterTable(stream, parameters);
        }

        public override void SerializeOperationResponse(StreamBuffer stream, OperationResponse serObject, bool setType)
        {
            if (setType)
            {
                stream.WriteByte(25);
            }
            stream.WriteByte(serObject.OperationCode);
            this.WriteInt16(stream, serObject.ReturnCode, false);
            if (string.IsNullOrEmpty(serObject.DebugMessage))
            {
                stream.WriteByte(8);
            }
            else
            {
                stream.WriteByte(7);
                this.WriteString(stream, serObject.DebugMessage, false);
            }
            this.WriteParameterTable(stream, serObject.Parameters);
        }

        internal void WriteByte(StreamBuffer stream, byte value, bool writeType)
        {
            if (writeType)
            {
                if (value == 0)
                {
                    stream.WriteByte(34);
                    return;
                }
                stream.WriteByte(3);
            }
            stream.WriteByte(value);
        }

        internal void WriteBoolean(StreamBuffer stream, bool value, bool writeType)
        {
            if (writeType)
            {
                if (value)
                {
                    stream.WriteByte(28);
                }
                else
                {
                    stream.WriteByte(27);
                }
            }
            else
            {
                stream.WriteByte((byte)(value ? 1 : 0));
            }
        }

        internal void WriteUShort(StreamBuffer stream, ushort value)
        {
            stream.WriteBytes((byte)value, (byte)(value >> 8));
        }

        internal void WriteInt16(StreamBuffer stream, short value, bool writeType)
        {
            if (writeType)
            {
                if (value == 0)
                {
                    stream.WriteByte(29);
                    return;
                }
                stream.WriteByte(4);
            }
            stream.WriteBytes((byte)value, (byte)(value >> 8));
        }

        internal void WriteDouble(StreamBuffer stream, double value, bool writeType)
        {
            if (writeType)
            {
                stream.WriteByte(6);
            }
            int num = 0;
            byte[] bufferAndAdvance = stream.GetBufferAndAdvance(8, out num);
            lock (memDoubleBlock)
            {
                memDoubleBlock[0] = value;
                Buffer.BlockCopy(memDoubleBlock, 0, bufferAndAdvance, num, 8);
            }
        }

        internal void WriteSingle(StreamBuffer stream, float value, bool writeType)
        {
            if (writeType)
            {
                stream.WriteByte(5);
            }
            int num = 0;
            byte[] bufferAndAdvance = stream.GetBufferAndAdvance(4, out num);
            lock (memFloatBlock)
            {
                memFloatBlock[0] = value;
                Buffer.BlockCopy(memFloatBlock, 0, bufferAndAdvance, num, 4);
            }
        }

        internal void WriteString(StreamBuffer stream, string value, bool writeType)
        {
            if (writeType)
            {
                stream.WriteByte(7);
            }
            byte[] bytes = Encoding.UTF8.GetBytes(value);
            this.WriteIntLength(stream, bytes.Length);
            stream.Write(bytes, 0, bytes.Length);
        }

        private void WriteHashtable(StreamBuffer stream, object value, bool writeType)
        {
            ExitGames.Client.Photon.Hashtable hashtable = (ExitGames.Client.Photon.Hashtable)value;
            if (writeType)
            {
                stream.WriteByte(21);
            }
            this.WriteIntLength(stream, hashtable.Count);
            Dictionary<object, object>.KeyCollection keys = hashtable.Keys;
            foreach (object item in keys)
            {
                this.Write(stream, item, true);
                this.Write(stream, hashtable[item], true);
            }
        }

        internal void WriteByteArray(StreamBuffer stream, byte[] value, bool writeType)
        {
            if (writeType)
            {
                stream.WriteByte(67);
            }
            this.WriteIntLength(stream, value.Length);
            stream.Write(value, 0, value.Length);
        }

        private void WriteByteArraySegment(StreamBuffer stream, byte[] value, int offset, int count, bool writeType)
        {
            if (writeType)
            {
                stream.WriteByte(67);
            }
            this.WriteIntLength(stream, count);
            stream.Write(value, offset, count);
        }

        internal void WriteInt32ArrayCompressed(StreamBuffer stream, int[] value, bool writeType)
        {
            if (writeType)
            {
                stream.WriteByte(73);
            }
            this.WriteIntLength(stream, value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                this.WriteCompressedInt32(stream, value[i], false);
            }
        }

        private void WriteInt64ArrayCompressed(StreamBuffer stream, long[] values, bool setType)
        {
            if (setType)
            {
                stream.WriteByte(74);
            }
            this.WriteIntLength(stream, values.Length);
            for (int i = 0; i < values.Length; i++)
            {
                this.WriteCompressedInt64(stream, values[i], false);
            }
        }

        internal void WriteBoolArray(StreamBuffer stream, bool[] value, bool writeType)
        {
            if (writeType)
            {
                stream.WriteByte(66);
            }
            this.WriteIntLength(stream, value.Length);
            int num = value.Length >> 3;
            uint num2 = (uint)(num + 1);
            byte[] array = new byte[num2];
            int num3 = 0;
            int i = 0;
            while (num > 0)
            {
                byte b = 0;
                if (value[i++])
                {
                    b = (byte)(b | 1);
                }
                if (value[i++])
                {
                    b = (byte)(b | 2);
                }
                if (value[i++])
                {
                    b = (byte)(b | 4);
                }
                if (value[i++])
                {
                    b = (byte)(b | 8);
                }
                if (value[i++])
                {
                    b = (byte)(b | 0x10);
                }
                if (value[i++])
                {
                    b = (byte)(b | 0x20);
                }
                if (value[i++])
                {
                    b = (byte)(b | 0x40);
                }
                if (value[i++])
                {
                    b = (byte)(b | 0x80);
                }
                array[num3] = b;
                num--;
                num3++;
            }
            if (i < value.Length)
            {
                byte b2 = 0;
                int num12 = 0;
                for (; i < value.Length; i++)
                {
                    if (value[i])
                    {
                        b2 = (byte)(b2 | (byte)(1 << num12));
                    }
                    num12++;
                }
                array[num3] = b2;
                num3++;
            }
            stream.Write(array, 0, num3);
        }

        internal void WriteInt16Array(StreamBuffer stream, short[] value, bool writeType)
        {
            if (writeType)
            {
                stream.WriteByte(68);
            }
            this.WriteIntLength(stream, value.Length);
            for (int i = 0; i < value.Length; i++)
            {
                this.WriteInt16(stream, value[i], false);
            }
        }

        internal void WriteDoubleArray(StreamBuffer stream, double[] values, bool setType)
        {
            if (setType)
            {
                stream.WriteByte(70);
            }
            this.WriteIntLength(stream, values.Length);
            byte[] array = new byte[values.Length * 8];
            Buffer.BlockCopy(values, 0, array, 0, values.Length * 8);
            for (int i = 0; i < values.Length; i++)
            {
                WriteDouble(stream, values[i], false);
            }
        }
        internal void WriteSingleArray(StreamBuffer stream, float[] values, bool setType)
        {
            if (setType)
            {
                stream.WriteByte(69);
            }
            this.WriteIntLength(stream, values.Length);
            byte[] array = new byte[values.Length * 4];
            Buffer.BlockCopy(values, 0, array, 0, values.Length * 4);
            for (int i = 0; i < values.Length; i++)
            {
                WriteSingle(stream, values[i], false);
            }
        }

        internal void WriteStringArray(StreamBuffer stream, object value0, bool writeType)
        {
            string[] array = (string[])value0;
            if (writeType)
            {
                stream.WriteByte(71);
            }
            this.WriteIntLength(stream, array.Length);
            int num = 0;
            while (true)
            {
                if (num < array.Length)
                {
                    if (array[num] != null)
                    {
                        this.WriteString(stream, array[num], false);
                        num++;
                        continue;
                    }
                    break;
                }
                return;
            }
            throw new ExitGames.Client.Photon.InvalidDataException("Unexpected - cannot serialize string array with null element " + num);
        }

        private void WriteObjectArray(StreamBuffer stream, object array, bool writeType)
        {
            this.WriteObjectArray(stream, (IList)array, writeType);
        }

        private void WriteObjectArray(StreamBuffer stream, IList array, bool writeType)
        {
            if (writeType)
            {
                stream.WriteByte(23);
            }
            this.WriteIntLength(stream, array.Count);
            for (int i = 0; i < array.Count; i++)
            {
                object value = array[i];
                this.Write(stream, value, true);
            }
        }

        private void WriteArrayInArray(StreamBuffer stream, object value, bool writeType)
        {
            object[] array = (object[])value;
            stream.WriteByte(64);
            this.WriteIntLength(stream, array.Length);
            object[] array2 = array;
            foreach (object value2 in array2)
            {
                this.Write(stream, value2, true);
            }
        }

        private void WriteCustomTypeBody(CustomType customType, StreamBuffer stream, object value)
        {
            if (customType.SerializeStreamFunction == null)
            {
                byte[] array = customType.SerializeFunction(value);
                this.WriteIntLength(stream, array.Length);
                stream.Write(array, 0, array.Length);
            }
            else
            {
                int position = stream.Position;
                stream.Position++;
                uint num = (uint)customType.SerializeStreamFunction(stream, value);
                int num2 = this.WriteCompressedUInt32(this.memCustomTypeBodyLengthSerialized, num);
                if (num2 == 1)
                {
                    stream.GetBuffer()[position] = this.memCustomTypeBodyLengthSerialized[0];
                }
                else
                {
                    for (int i = 0; i < num2 - 1; i++)
                    {
                        stream.WriteByte(0);
                    }
                    Buffer.BlockCopy(stream.GetBuffer(), position + 1, stream.GetBuffer(), position + num2, (int)num);
                    Buffer.BlockCopy(this.memCustomTypeBodyLengthSerialized, 0, stream.GetBuffer(), position, num2);
                    stream.Position = (int)(position + num2 + num);
                }
            }
        }

        private void WriteCustomType(StreamBuffer stream, object value, bool writeType)
        {
            Type type = value.GetType();
            CustomType customType = default(CustomType);
            if (Protocol.TypeDict.TryGetValue(type, out customType))
            {
                if (writeType)
                {
                    if (customType.Code < 100)
                    {
                        stream.WriteByte((byte)(128 + customType.Code));
                    }
                    else
                    {
                        stream.WriteByte(19);
                        stream.WriteByte(customType.Code);
                    }
                }
                else
                {
                    stream.WriteByte(customType.Code);
                }
                this.WriteCustomTypeBody(customType, stream, value);
                return;
            }
            throw new Exception("Write failed. Custom type not found: " + type);
        }

        private void WriteCustomTypeArray(StreamBuffer stream, object value, bool writeType)
        {
            IList list = (IList)value;
            Type elementType = value.GetType().GetElementType();
            CustomType customType = default(CustomType);
            if (Protocol.TypeDict.TryGetValue(elementType, out customType))
            {
                if (writeType)
                {
                    stream.WriteByte(83);
                }
                this.WriteIntLength(stream, list.Count);
                stream.WriteByte(customType.Code);
                foreach (object item in list)
                {
                    this.WriteCustomTypeBody(customType, stream, item);
                }
                return;
            }
            throw new Exception("Write failed. Custom type of element not found: " + elementType);
        }

        private bool WriteArrayHeader(StreamBuffer stream, Type type)
        {
            Type elementType = type.GetElementType();
            while (elementType.IsArray)
            {
                stream.WriteByte(64);
                elementType = elementType.GetElementType();
            }
            GpType codeOfType = this.GetCodeOfType(elementType);
            if (codeOfType == GpType.Unknown)
            {
                return false;
            }
            stream.WriteByte((byte)(codeOfType | GpType.CustomTypeSlim));
            return true;
        }

        private void WriteDictionaryElements(StreamBuffer stream, IDictionary dictionary, GpType keyWriteType, GpType valueWriteType)
        {
            this.WriteIntLength(stream, dictionary.Count);
            IDictionaryEnumerator enumerator = dictionary.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    DictionaryEntry dictionaryEntry = (DictionaryEntry)enumerator.Current;
                    this.Write(stream, dictionaryEntry.Key, keyWriteType == GpType.Unknown);
                    this.Write(stream, dictionaryEntry.Value, valueWriteType == GpType.Unknown);
                }
            }
            finally
            {
                IDisposable disposable = enumerator as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
        }

        private void WriteDictionary(StreamBuffer stream, object dict, bool setType)
        {
            if (setType)
            {
                stream.WriteByte(20);
            }
            GpType keyWriteType = default(GpType);
            GpType valueWriteType = default(GpType);
            this.WriteDictionaryHeader(stream, dict.GetType(), out keyWriteType, out valueWriteType);
            IDictionary dictionary = (IDictionary)dict;
            this.WriteDictionaryElements(stream, dictionary, keyWriteType, valueWriteType);
        }

        private void WriteDictionaryHeader(StreamBuffer stream, Type type, out GpType keyWriteType, out GpType valueWriteType)
        {
            Type[] genericArguments = type.GetGenericArguments();
            if (genericArguments[0] == typeof(object))
            {
                stream.WriteByte(0);
                keyWriteType = GpType.Unknown;
            }
            else
            {
                if (!genericArguments[0].IsPrimitive && genericArguments[0] != typeof(string))
                {
                    throw new ExitGames.Client.Photon.InvalidDataException("Unexpected - cannot serialize Dictionary with key type: " + genericArguments[0]);
                }
                keyWriteType = this.GetCodeOfType(genericArguments[0]);
                if (keyWriteType == GpType.Unknown)
                {
                    throw new ExitGames.Client.Photon.InvalidDataException("Unexpected - cannot serialize Dictionary with key type: " + genericArguments[0]);
                }
                stream.WriteByte((byte)keyWriteType);
            }
            if (genericArguments[1] == typeof(object))
            {
                stream.WriteByte(0);
                valueWriteType = GpType.Unknown;
            }
            else
            {
                if (genericArguments[1].IsArray)
                {
                    if (this.WriteArrayType(stream, genericArguments[1], out valueWriteType))
                    {
                        return;
                    }
                    throw new ExitGames.Client.Photon.InvalidDataException("Unexpected - cannot serialize Dictionary with value type: " + genericArguments[1]);
                }
                valueWriteType = this.GetCodeOfType(genericArguments[1]);
                if (valueWriteType == GpType.Unknown)
                {
                    throw new ExitGames.Client.Photon.InvalidDataException("Unexpected - cannot serialize Dictionary with value type: " + genericArguments[1]);
                }
                if (valueWriteType == GpType.Array)
                {
                    if (this.WriteArrayHeader(stream, genericArguments[1]))
                    {
                        return;
                    }
                    throw new ExitGames.Client.Photon.InvalidDataException("Unexpected - cannot serialize Dictionary with value type: " + genericArguments[1]);
                }
                if (valueWriteType == GpType.Dictionary)
                {
                    stream.WriteByte((byte)valueWriteType);
                    GpType gpType = default(GpType);
                    GpType gpType2 = default(GpType);
                    this.WriteDictionaryHeader(stream, genericArguments[1], out gpType, out gpType2);
                }
                else
                {
                    stream.WriteByte((byte)valueWriteType);
                }
            }
        }

        private bool WriteArrayType(StreamBuffer stream, Type type, out GpType writeType)
        {
            Type elementType = type.GetElementType();
            if (elementType == null)
            {
                throw new ExitGames.Client.Photon.InvalidDataException("Unexpected - cannot serialize array with type: " + type);
            }
            if (elementType.IsArray)
            {
                while (elementType != null && elementType.IsArray)
                {
                    stream.WriteByte(64);
                    elementType = elementType.GetElementType();
                }
                byte value = (byte)(this.GetCodeOfType(elementType) | GpType.Array);
                stream.WriteByte(value);
                writeType = GpType.Array;
                return true;
            }
            if (elementType.IsPrimitive)
            {
                byte b = (byte)(this.GetCodeOfType(elementType) | GpType.Array);
                if (b == 226)
                {
                    b = 67;
                }
                stream.WriteByte(b);
                if (Enum.IsDefined(typeof(GpType), b))
                {
                    writeType = (GpType)b;
                    return true;
                }
                writeType = GpType.Unknown;
                return false;
            }
            if (elementType == typeof(string))
            {
                stream.WriteByte(71);
                writeType = GpType.StringArray;
                return true;
            }
            if (elementType == typeof(object))
            {
                stream.WriteByte(23);
                writeType = GpType.ObjectArray;
                return true;
            }
            if (elementType == typeof(ExitGames.Client.Photon.Hashtable))
            {
                stream.WriteByte(85);
                writeType = GpType.HashtableArray;
                return true;
            }
            writeType = GpType.Unknown;
            return false;
        }

        private void WriteHashtableArray(StreamBuffer stream, object value, bool writeType)
        {
            ExitGames.Client.Photon.Hashtable[] array = (ExitGames.Client.Photon.Hashtable[])value;
            if (writeType)
            {
                stream.WriteByte(85);
            }
            this.WriteIntLength(stream, array.Length);
            ExitGames.Client.Photon.Hashtable[] array2 = array;
            foreach (ExitGames.Client.Photon.Hashtable value2 in array2)
            {
                this.WriteHashtable(stream, value2, false);
            }
        }

        private void WriteDictionaryArray(StreamBuffer stream, IDictionary[] dictArray, bool writeType)
        {
            stream.WriteByte(84);
            GpType keyWriteType = default(GpType);
            GpType valueWriteType = default(GpType);
            this.WriteDictionaryHeader(stream, dictArray.GetType().GetElementType(), out keyWriteType, out valueWriteType);
            this.WriteIntLength(stream, dictArray.Length);
            foreach (IDictionary dictionary in dictArray)
            {
                this.WriteDictionaryElements(stream, dictionary, keyWriteType, valueWriteType);
            }
        }

        private void WriteIntLength(StreamBuffer stream, int value)
        {
            this.WriteCompressedUInt32(stream, (uint)value);
        }

        private void WriteVarInt32(StreamBuffer stream, int value, bool writeType)
        {
            this.WriteCompressedInt32(stream, value, writeType);
        }

        private void WriteCompressedInt32(StreamBuffer stream, int value, bool writeType)
        {
            if (writeType)
            {
                if (value == 0)
                {
                    stream.WriteByte(30);
                    return;
                }
                if (value > 0)
                {
                    if (value <= 255)
                    {
                        stream.WriteByte(11);
                        stream.WriteByte((byte)value);
                        return;
                    }
                    if (value <= 65535)
                    {
                        stream.WriteByte(13);
                        this.WriteUShort(stream, (ushort)value);
                        return;
                    }
                }
                else if (value >= -65535)
                {
                    if (value >= -255)
                    {
                        stream.WriteByte(12);
                        stream.WriteByte((byte)(-value));
                        return;
                    }
                    if (value >= -65535)
                    {
                        stream.WriteByte(14);
                        this.WriteUShort(stream, (ushort)(-value));
                        return;
                    }
                }
            }
            if (writeType)
            {
                stream.WriteByte(9);
            }
            uint value2 = this.EncodeZigZag32(value);
            this.WriteCompressedUInt32(stream, value2);
        }

        private void WriteCompressedInt64(StreamBuffer stream, long value, bool writeType)
        {
            if (writeType)
            {
                if (value == 0)
                {
                    stream.WriteByte(31);
                    return;
                }
                if (value > 0)
                {
                    if (value <= 255)
                    {
                        stream.WriteByte(15);
                        stream.WriteByte((byte)value);
                        return;
                    }
                    if (value <= 65535)
                    {
                        stream.WriteByte(17);
                        this.WriteUShort(stream, (ushort)value);
                        return;
                    }
                }
                else if (value >= -65535)
                {
                    if (value >= -255)
                    {
                        stream.WriteByte(16);
                        stream.WriteByte((byte)(-value));
                        return;
                    }
                    if (value >= -65535)
                    {
                        stream.WriteByte(18);
                        this.WriteUShort(stream, (ushort)(-value));
                        return;
                    }
                }
            }
            if (writeType)
            {
                stream.WriteByte(10);
            }
            ulong value2 = this.EncodeZigZag64(value);
            this.WriteCompressedUInt64(stream, value2);
        }

        private void WriteCompressedUInt32(StreamBuffer stream, uint value)
        {
            byte[] obj = this.memCompressedUInt32;
            lock (obj)
            {
                stream.Write(this.memCompressedUInt32, 0, this.WriteCompressedUInt32(this.memCompressedUInt32, value));
            }
        }

        private int WriteCompressedUInt32(byte[] buffer, uint value)
        {
            int num = 0;
            buffer[num] = (byte)(value & 0x7F);
            for (value >>= 7; value != 0; value >>= 7)
            {
                buffer[num] |= 128;
                buffer[++num] = (byte)(value & 0x7F);
            }
            return num + 1;
        }

        private void WriteCompressedUInt64(StreamBuffer stream, ulong value)
        {
            int num = 0;
            byte[] obj = this.memCompressedUInt64;
            lock (obj)
            {
                this.memCompressedUInt64[num] = (byte)(value & 0x7F);
                for (value >>= 7; value != 0; value >>= 7)
                {
                    this.memCompressedUInt64[num] |= 128;
                    this.memCompressedUInt64[++num] = (byte)(value & 0x7F);
                }
                num++;
                stream.Write(this.memCompressedUInt64, 0, num);
            }
        }

        private uint EncodeZigZag32(int value)
        {
            return (uint)(value << 1 ^ value >> 31);
        }

        private ulong EncodeZigZag64(long value)
        {
            return (ulong)(value << 1 ^ value >> 63);
        }
    }
}
