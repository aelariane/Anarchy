using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace ExitGames.Client.Photon.RpcProtocols.GpBinaryV16
{
    internal class Protocol16 : IProtocol
    {
        private readonly byte[] versionBytes = new byte[2] { 1, 6 };

        private readonly byte[] memShort = new byte[2];

        private readonly long[] memLongBlock = new long[1];

        private readonly byte[] memLongBlockBytes = new byte[8];

        private static readonly float[] memFloatBlock = new float[1];

        private static readonly byte[] memFloatBlockBytes = new byte[4];

        private readonly double[] memDoubleBlock = new double[1];

        private readonly byte[] memDoubleBlockBytes = new byte[8];

        private readonly byte[] memInteger = new byte[4];

        private readonly byte[] memLong = new byte[8];

        private readonly byte[] memFloat = new byte[4];

        private readonly byte[] memDouble = new byte[8];

        private byte[] memString;

        public override string ProtocolType
        {
            get
            {
                return "GpBinaryV16";
            }
        }

        public override byte[] VersionBytes
        {
            get
            {
                return this.versionBytes;
            }
        }

        private bool SerializeCustom(StreamBuffer dout, object serObject)
        {
            CustomType customType = default(CustomType);
            if (Protocol.TypeDict.TryGetValue(serObject.GetType(), out customType))
            {
                if (customType.SerializeStreamFunction == null)
                {
                    byte[] array = customType.SerializeFunction(serObject);
                    dout.WriteByte(99);
                    dout.WriteByte(customType.Code);
                    this.SerializeShort(dout, (short)array.Length, false);
                    dout.Write(array, 0, array.Length);
                    return true;
                }
                dout.WriteByte(99);
                dout.WriteByte(customType.Code);
                int position = dout.IntPosition;
                dout.IntPosition += 2;
                short num = customType.SerializeStreamFunction(dout, serObject);
                long num2 = dout.IntPosition;
                dout.IntPosition = position;
                this.SerializeShort(dout, num, false);
                dout.IntPosition += num;
                if (dout.IntPosition != num2)
                {
                    throw new Exception("Serialization failed. Stream position corrupted. Should be " + num2 + " is now: " + dout.IntPosition + " serializedLength: " + num);
                }
                return true;
            }
            return false;
        }

        private object DeserializeCustom(StreamBuffer din, byte customTypeCode)
        {
            short num = this.DeserializeShort(din);
            CustomType customType = default(CustomType);
            if (Protocol.CodeDict.TryGetValue(customTypeCode, out customType))
            {
                if (customType.DeserializeStreamFunction == null)
                {
                    byte[] array = new byte[num];
                    din.Read(array, 0, num);
                    return customType.DeserializeFunction(array);
                }
                int position = din.IntPosition;
                object result = customType.DeserializeStreamFunction(din, num);
                int num2 = din.IntPosition - position;
                if (num2 != num)
                {
                    din.IntPosition = position + num;
                }
                return result;
            }
            byte[] array2 = new byte[num];
            din.Read(array2, 0, num);
            return array2;
        }

        private Type GetTypeOfCode(byte typeCode)
        {
            switch (typeCode)
            {
                case 105:
                    return typeof(int);
                case 115:
                    return typeof(string);
                case 97:
                    return typeof(string[]);
                case 120:
                    return typeof(byte[]);
                case 110:
                    return typeof(int[]);
                case 104:
                    return typeof(ExitGames.Client.Photon.Hashtable);
                case 68:
                    return typeof(IDictionary);
                case 111:
                    return typeof(bool);
                case 107:
                    return typeof(short);
                case 108:
                    return typeof(long);
                case 98:
                    return typeof(byte);
                case 102:
                    return typeof(float);
                case 100:
                    return typeof(double);
                case 121:
                    return typeof(Array);
                case 99:
                    return typeof(CustomType);
                case 122:
                    return typeof(object[]);
                case 101:
                    return typeof(EventData);
                case 113:
                    return typeof(OperationRequest);
                case 112:
                    return typeof(OperationResponse);
                case 0:
                case 42:
                    return typeof(object);
                default:
                    Debug.WriteLine("missing type: " + typeCode);
                    throw new Exception("deserialize(): " + typeCode);
            }
        }

        private GpTypeV16 GetCodeOfType(Type type)
        {
            switch (Type.GetTypeCode(type))
            {
                case TypeCode.Byte:
                    return GpTypeV16.Byte;
                case TypeCode.String:
                    return GpTypeV16.String;
                case TypeCode.Boolean:
                    return GpTypeV16.Boolean;
                case TypeCode.Int16:
                    return GpTypeV16.Short;
                case TypeCode.Int32:
                    return GpTypeV16.Integer;
                case TypeCode.Int64:
                    return GpTypeV16.Long;
                case TypeCode.Single:
                    return GpTypeV16.Float;
                case TypeCode.Double:
                    return GpTypeV16.Double;
                default:
                    if (type.IsArray)
                    {
                        if (type == typeof(byte[]))
                        {
                            return GpTypeV16.ByteArray;
                        }
                        return GpTypeV16.Array;
                    }
                    if (type == typeof(ExitGames.Client.Photon.Hashtable))
                    {
                        return GpTypeV16.Hashtable;
                    }
                    if (type == typeof(List<object>))
                    {
                        return GpTypeV16.ObjectArray;
                    }
                    if (type.IsGenericType && typeof(Dictionary<,>) == type.GetGenericTypeDefinition())
                    {
                        return GpTypeV16.Dictionary;
                    }
                    if (type == typeof(EventData))
                    {
                        return GpTypeV16.EventData;
                    }
                    if (type == typeof(OperationRequest))
                    {
                        return GpTypeV16.OperationRequest;
                    }
                    if (type == typeof(OperationResponse))
                    {
                        return GpTypeV16.OperationResponse;
                    }
                    return GpTypeV16.Unknown;
            }
        }

        private Array CreateArrayByType(byte arrayType, short length)
        {
            return Array.CreateInstance(this.GetTypeOfCode(arrayType), length);
        }

        private void SerializeOperationRequest(StreamBuffer stream, OperationRequest serObject, bool setType)
        {
            this.SerializeOperationRequest(stream, serObject.OperationCode, serObject.Parameters, setType);
        }

        public override void SerializeOperationRequest(StreamBuffer stream, byte operationCode, Dictionary<byte, object> parameters, bool setType)
        {
            if (setType)
            {
                stream.WriteByte(113);
            }
            stream.WriteByte(operationCode);
            this.SerializeParameterTable(stream, parameters);
        }

        public override OperationRequest DeserializeOperationRequest(StreamBuffer din)
        {
            OperationRequest operationRequest = new OperationRequest();
            operationRequest.OperationCode = this.DeserializeByte(din);
            operationRequest.Parameters = this.DeserializeParameterTable(din);
            return operationRequest;
        }

        public override void SerializeOperationResponse(StreamBuffer stream, OperationResponse serObject, bool setType)
        {
            if (setType)
            {
                stream.WriteByte(112);
            }
            stream.WriteByte(serObject.OperationCode);
            this.SerializeShort(stream, serObject.ReturnCode, false);
            if (string.IsNullOrEmpty(serObject.DebugMessage))
            {
                stream.WriteByte(42);
            }
            else
            {
                this.SerializeString(stream, serObject.DebugMessage, false);
            }
            this.SerializeParameterTable(stream, serObject.Parameters);
        }

        public override OperationResponse DeserializeOperationResponse(StreamBuffer stream)
        {
            OperationResponse operationResponse = new OperationResponse();
            operationResponse.OperationCode = this.DeserializeByte(stream);
            operationResponse.ReturnCode = this.DeserializeShort(stream);
            operationResponse.DebugMessage = (this.Deserialize(stream, this.DeserializeByte(stream)) as string);
            operationResponse.Parameters = this.DeserializeParameterTable(stream);
            return operationResponse;
        }

        public override void SerializeEventData(StreamBuffer stream, EventData serObject, bool setType)
        {
            if (setType)
            {
                stream.WriteByte(101);
            }
            stream.WriteByte(serObject.Code);
            this.SerializeParameterTable(stream, serObject.Parameters);
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
            eventData.Code = this.DeserializeByte(din);
            eventData.Parameters = this.DeserializeParameterTable(din);
            return eventData;
        }

        private void SerializeParameterTable(StreamBuffer stream, Dictionary<byte, object> parameters)
        {
            if (parameters == null || parameters.Count == 0)
            {
                this.SerializeShort(stream, 0, false);
            }
            else
            {
                this.SerializeShort(stream, (short)parameters.Count, false);
                foreach (KeyValuePair<byte, object> parameter in parameters)
                {
                    stream.WriteByte(parameter.Key);
                    this.Serialize(stream, parameter.Value, true);
                }
            }
        }

        private Dictionary<byte, object> DeserializeParameterTable(StreamBuffer stream)
        {
            short num = this.DeserializeShort(stream);
            Dictionary<byte, object> dictionary = new Dictionary<byte, object>(num);
            for (int i = 0; i < num; i++)
            {
                byte key = stream.ReadByteAsByte();
                object obj2 = dictionary[key] = this.Deserialize(stream, stream.ReadByteAsByte());
            }
            return dictionary;
        }

        public override void Serialize(StreamBuffer dout, object serObject, bool setType)
        {
            if (serObject == null)
            {
                if (setType)
                {
                    dout.WriteByte(42);
                }
            }
            else
            {
                switch (this.GetCodeOfType(serObject.GetType()))
                {
                    case GpTypeV16.Byte:
                        this.SerializeByte(dout, (byte)serObject, setType);
                        break;
                    case GpTypeV16.String:
                        this.SerializeString(dout, (string)serObject, setType);
                        break;
                    case GpTypeV16.Boolean:
                        this.SerializeBoolean(dout, (bool)serObject, setType);
                        break;
                    case GpTypeV16.Short:
                        this.SerializeShort(dout, (short)serObject, setType);
                        break;
                    case GpTypeV16.Integer:
                        this.SerializeInteger(dout, (int)serObject, setType);
                        break;
                    case GpTypeV16.Long:
                        this.SerializeLong(dout, (long)serObject, setType);
                        break;
                    case GpTypeV16.Float:
                        this.SerializeFloat(dout, (float)serObject, setType);
                        break;
                    case GpTypeV16.Double:
                        this.SerializeDouble(dout, (double)serObject, setType);
                        break;
                    case GpTypeV16.Hashtable:
                        this.SerializeHashTable(dout, (ExitGames.Client.Photon.Hashtable)serObject, setType);
                        break;
                    case GpTypeV16.ByteArray:
                        this.SerializeByteArray(dout, (byte[])serObject, setType);
                        break;
                    case GpTypeV16.ObjectArray:
                        this.SerializeObjectArray(dout, (IList)serObject, setType);
                        break;
                    case GpTypeV16.Array:
                        if (serObject is int[])
                        {
                            this.SerializeIntArrayOptimized(dout, (int[])serObject, setType);
                        }
                        else if (serObject.GetType().GetElementType() == typeof(object))
                        {
                            this.SerializeObjectArray(dout, serObject as object[], setType);
                        }
                        else
                        {
                            this.SerializeArray(dout, (Array)serObject, setType);
                        }
                        break;
                    case GpTypeV16.Dictionary:
                        this.SerializeDictionary(dout, (IDictionary)serObject, setType);
                        break;
                    case GpTypeV16.EventData:
                        this.SerializeEventData(dout, (EventData)serObject, setType);
                        break;
                    case GpTypeV16.OperationResponse:
                        this.SerializeOperationResponse(dout, (OperationResponse)serObject, setType);
                        break;
                    case GpTypeV16.OperationRequest:
                        this.SerializeOperationRequest(dout, (OperationRequest)serObject, setType);
                        break;
                    default:
                        if (serObject is ArraySegment<byte>)
                        {
                            ArraySegment<byte> arraySegment = (ArraySegment<byte>)serObject;
                            this.SerializeByteArraySegment(dout, arraySegment.Array, arraySegment.Offset, arraySegment.Count, setType);
                            break;
                        }
                        if (this.SerializeCustom(dout, serObject))
                        {
                            break;
                        }
                        throw new Exception("cannot serialize(): " + serObject.GetType());
                }
            }
        }

        private void SerializeByte(StreamBuffer dout, byte serObject, bool setType)
        {
            if (setType)
            {
                dout.WriteByte(98);
            }
            dout.WriteByte(serObject);
        }

        private void SerializeBoolean(StreamBuffer dout, bool serObject, bool setType)
        {
            if (setType)
            {
                dout.WriteByte(111);
            }
            dout.WriteByte((byte)(serObject ? 1 : 0));
        }

        public override void SerializeShort(StreamBuffer dout, short serObject, bool setType)
        {
            if (setType)
            {
                dout.WriteByte(107);
            }
            byte[] obj = this.memShort;
            lock (obj)
            {
                byte[] array = this.memShort;
                array[0] = (byte)(serObject >> 8);
                array[1] = (byte)serObject;
                dout.Write(array, 0, 2);
            }
        }

        private void SerializeInteger(StreamBuffer dout, int serObject, bool setType)
        {
            if (setType)
            {
                dout.WriteByte(105);
            }
            byte[] obj = this.memInteger;
            lock (obj)
            {
                byte[] array = this.memInteger;
                array[0] = (byte)(serObject >> 24);
                array[1] = (byte)(serObject >> 16);
                array[2] = (byte)(serObject >> 8);
                array[3] = (byte)serObject;
                dout.Write(array, 0, 4);
            }
        }

        private void SerializeLong(StreamBuffer dout, long serObject, bool setType)
        {
            if (setType)
            {
                dout.WriteByte(108);
            }
            long[] obj = this.memLongBlock;
            lock (obj)
            {
                this.memLongBlock[0] = serObject;
                Buffer.BlockCopy(this.memLongBlock, 0, this.memLongBlockBytes, 0, 8);
                byte[] array = this.memLongBlockBytes;
                if (BitConverter.IsLittleEndian)
                {
                    byte b = array[0];
                    byte b2 = array[1];
                    byte b3 = array[2];
                    byte b4 = array[3];
                    array[0] = array[7];
                    array[1] = array[6];
                    array[2] = array[5];
                    array[3] = array[4];
                    array[4] = b4;
                    array[5] = b3;
                    array[6] = b2;
                    array[7] = b;
                }
                dout.Write(array, 0, 8);
            }
        }

        private void SerializeFloat(StreamBuffer dout, float serObject, bool setType)
        {
            if (setType)
            {
                dout.WriteByte(102);
            }
            lock (memFloatBlockBytes)
            {
                memFloatBlock[0] = serObject;
                Buffer.BlockCopy(memFloatBlock, 0, memFloatBlockBytes, 0, 4);
                if (BitConverter.IsLittleEndian)
                {
                    byte b = memFloatBlockBytes[0];
                    byte b2 = memFloatBlockBytes[1];
                    memFloatBlockBytes[0] = memFloatBlockBytes[3];
                    memFloatBlockBytes[1] = memFloatBlockBytes[2];
                    memFloatBlockBytes[2] = b2;
                    memFloatBlockBytes[3] = b;
                }
                dout.Write(memFloatBlockBytes, 0, 4);
            }
        }

        private void SerializeDouble(StreamBuffer dout, double serObject, bool setType)
        {
            if (setType)
            {
                dout.WriteByte(100);
            }
            byte[] obj = this.memDoubleBlockBytes;
            lock (obj)
            {
                this.memDoubleBlock[0] = serObject;
                Buffer.BlockCopy(this.memDoubleBlock, 0, this.memDoubleBlockBytes, 0, 8);
                byte[] array = this.memDoubleBlockBytes;
                if (BitConverter.IsLittleEndian)
                {
                    byte b = array[0];
                    byte b2 = array[1];
                    byte b3 = array[2];
                    byte b4 = array[3];
                    array[0] = array[7];
                    array[1] = array[6];
                    array[2] = array[5];
                    array[3] = array[4];
                    array[4] = b4;
                    array[5] = b3;
                    array[6] = b2;
                    array[7] = b;
                }
                dout.Write(array, 0, 8);
            }
        }

        public override void SerializeString(StreamBuffer dout, string serObject, bool setType)
        {
            if (setType)
            {
                dout.WriteByte(115);
            }
            byte[] bytes = Encoding.UTF8.GetBytes(serObject);
            if (bytes.Length > 32767)
            {
                throw new NotSupportedException("Strings that exceed a UTF8-encoded byte-length of 32767 (short.MaxValue) are not supported. Yours is: " + bytes.Length);
            }
            this.SerializeShort(dout, (short)bytes.Length, false);
            dout.Write(bytes, 0, bytes.Length);
        }

        private void SerializeArray(StreamBuffer dout, Array serObject, bool setType)
        {
            if (setType)
            {
                dout.WriteByte(121);
            }
            if (serObject.Length > 32767)
            {
                throw new NotSupportedException("String[] that exceed 32767 (short.MaxValue) entries are not supported. Yours is: " + serObject.Length);
            }
            this.SerializeShort(dout, (short)serObject.Length, false);
            Type elementType = serObject.GetType().GetElementType();
            GpTypeV16 codeOfType = this.GetCodeOfType(elementType);
            if (codeOfType != 0)
            {
                dout.WriteByte((byte)codeOfType);
                if (codeOfType == GpTypeV16.Dictionary)
                {
                    bool setKeyType = default(bool);
                    bool setValueType = default(bool);
                    this.SerializeDictionaryHeader(dout, (object)serObject, out setKeyType, out setValueType);
                    for (int i = 0; i < serObject.Length; i++)
                    {
                        object value = serObject.GetValue(i);
                        this.SerializeDictionaryElements(dout, value, setKeyType, setValueType);
                    }
                }
                else
                {
                    for (int j = 0; j < serObject.Length; j++)
                    {
                        object value2 = serObject.GetValue(j);
                        this.Serialize(dout, value2, false);
                    }
                }
                return;
            }
            CustomType customType = default(CustomType);
            if (Protocol.TypeDict.TryGetValue(elementType, out customType))
            {
                dout.WriteByte(99);
                dout.WriteByte(customType.Code);
                int num = 0;
                short num2;
                long num3;
                while (true)
                {
                    if (num < serObject.Length)
                    {
                        object value3 = serObject.GetValue(num);
                        if (customType.SerializeStreamFunction == null)
                        {
                            byte[] array = customType.SerializeFunction(value3);
                            this.SerializeShort(dout, (short)array.Length, false);
                            dout.Write(array, 0, array.Length);
                        }
                        else
                        {
                            int position = dout.IntPosition;
                            dout.IntPosition += 2;
                            num2 = customType.SerializeStreamFunction(dout, value3);
                            num3 = dout.IntPosition;
                            dout.IntPosition = position;
                            this.SerializeShort(dout, num2, false);
                            dout.IntPosition += num2;
                            if (dout.IntPosition != num3)
                            {
                                break;
                            }
                        }
                        num++;
                        continue;
                    }
                    return;
                }
                throw new Exception("Serialization failed. Stream position corrupted. Should be " + num3 + " is now: " + dout.IntPosition + " serializedLength: " + num2);
            }
            throw new NotSupportedException("cannot serialize array of type " + elementType);
        }

        private void SerializeByteArray(StreamBuffer dout, byte[] serObject, bool setType)
        {
            if (setType)
            {
                dout.WriteByte(120);
            }
            this.SerializeInteger(dout, serObject.Length, false);
            dout.Write(serObject, 0, serObject.Length);
        }

        private void SerializeByteArraySegment(StreamBuffer dout, byte[] serObject, int offset, int count, bool setType)
        {
            if (setType)
            {
                dout.WriteByte(120);
            }
            this.SerializeInteger(dout, count, false);
            dout.Write(serObject, offset, count);
        }

        private void SerializeIntArrayOptimized(StreamBuffer inWriter, int[] serObject, bool setType)
        {
            if (setType)
            {
                inWriter.WriteByte(121);
            }
            this.SerializeShort(inWriter, (short)serObject.Length, false);
            inWriter.WriteByte(105);
            byte[] array = new byte[serObject.Length * 4];
            int num = 0;
            for (int i = 0; i < serObject.Length; i++)
            {
                array[num++] = (byte)(serObject[i] >> 24);
                array[num++] = (byte)(serObject[i] >> 16);
                array[num++] = (byte)(serObject[i] >> 8);
                array[num++] = (byte)serObject[i];
            }
            inWriter.Write(array, 0, array.Length);
        }

        private void SerializeStringArray(StreamBuffer dout, string[] serObject, bool setType)
        {
            if (setType)
            {
                dout.WriteByte(97);
            }
            this.SerializeShort(dout, (short)serObject.Length, false);
            for (int i = 0; i < serObject.Length; i++)
            {
                this.SerializeString(dout, serObject[i], false);
            }
        }

        private void SerializeObjectArray(StreamBuffer dout, IList objects, bool setType)
        {
            if (setType)
            {
                dout.WriteByte(122);
            }
            this.SerializeShort(dout, (short)objects.Count, false);
            for (int i = 0; i < objects.Count; i++)
            {
                object serObject = objects[i];
                this.Serialize(dout, serObject, true);
            }
        }

        private void SerializeHashTable(StreamBuffer dout, ExitGames.Client.Photon.Hashtable serObject, bool setType)
        {
            if (setType)
            {
                dout.WriteByte(104);
            }
            this.SerializeShort(dout, (short)serObject.Count, false);
            Dictionary<object, object>.KeyCollection keys = serObject.Keys;
            foreach (object item in keys)
            {
                this.Serialize(dout, item, true);
                this.Serialize(dout, serObject[item], true);
            }
        }

        private void SerializeDictionary(StreamBuffer dout, IDictionary serObject, bool setType)
        {
            if (setType)
            {
                dout.WriteByte(68);
            }
            bool setKeyType = default(bool);
            bool setValueType = default(bool);
            this.SerializeDictionaryHeader(dout, (object)serObject, out setKeyType, out setValueType);
            this.SerializeDictionaryElements(dout, serObject, setKeyType, setValueType);
        }

        private void SerializeDictionaryHeader(StreamBuffer writer, Type dictType)
        {
            bool flag = default(bool);
            bool flag2 = default(bool);
            this.SerializeDictionaryHeader(writer, (object)dictType, out flag, out flag2);
        }

        private void SerializeDictionaryHeader(StreamBuffer writer, object dict, out bool setKeyType, out bool setValueType)
        {
            Type[] genericArguments = dict.GetType().GetGenericArguments();
            setKeyType = (genericArguments[0] == typeof(object));
            setValueType = (genericArguments[1] == typeof(object));
            if (setKeyType)
            {
                writer.WriteByte(0);
            }
            else
            {
                GpTypeV16 codeOfType = this.GetCodeOfType(genericArguments[0]);
                if (codeOfType == GpTypeV16.Unknown || codeOfType == GpTypeV16.Dictionary)
                {
                    throw new Exception("Unexpected - cannot serialize Dictionary with key type: " + genericArguments[0]);
                }
                writer.WriteByte((byte)codeOfType);
            }
            if (setValueType)
            {
                writer.WriteByte(0);
            }
            else
            {
                GpTypeV16 codeOfType2 = this.GetCodeOfType(genericArguments[1]);
                if (codeOfType2 == GpTypeV16.Unknown)
                {
                    throw new Exception("Unexpected - cannot serialize Dictionary with value type: " + genericArguments[0]);
                }
                writer.WriteByte((byte)codeOfType2);
                if (codeOfType2 == GpTypeV16.Dictionary)
                {
                    this.SerializeDictionaryHeader(writer, genericArguments[1]);
                }
            }
        }

        private void SerializeDictionaryElements(StreamBuffer writer, object dict, bool setKeyType, bool setValueType)
        {
            IDictionary dictionary = (IDictionary)dict;
            this.SerializeShort(writer, (short)dictionary.Count, false);
            IDictionaryEnumerator enumerator = dictionary.GetEnumerator();
            try
            {
                while (enumerator.MoveNext())
                {
                    DictionaryEntry dictionaryEntry = (DictionaryEntry)enumerator.Current;
                    if (!setValueType && dictionaryEntry.Value == null)
                    {
                        throw new Exception("Can't serialize null in Dictionary with specific value-type.");
                    }
                    if (!setKeyType && dictionaryEntry.Key == null)
                    {
                        throw new Exception("Can't serialize null in Dictionary with specific key-type.");
                    }
                    this.Serialize(writer, dictionaryEntry.Key, setKeyType);
                    this.Serialize(writer, dictionaryEntry.Value, setValueType);
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

        public override object Deserialize(StreamBuffer din, byte type)
        {
            switch (type)
            {
                case 105:
                    return this.DeserializeInteger(din);
                case 115:
                    return this.DeserializeString(din);
                case 97:
                    return this.DeserializeStringArray(din);
                case 120:
                    return this.DeserializeByteArray(din, -1);
                case 110:
                    return this.DeserializeIntArray(din, -1);
                case 104:
                    return this.DeserializeHashTable(din);
                case 68:
                    return this.DeserializeDictionary(din);
                case 111:
                    return this.DeserializeBoolean(din);
                case 107:
                    return this.DeserializeShort(din);
                case 108:
                    return this.DeserializeLong(din);
                case 98:
                    return this.DeserializeByte(din);
                case 102:
                    return this.DeserializeFloat(din);
                case 100:
                    return this.DeserializeDouble(din);
                case 121:
                    return this.DeserializeArray(din);
                case 99:
                    {
                        byte customTypeCode = din.ReadByteAsByte();
                        return this.DeserializeCustom(din, customTypeCode);
                    }
                case 122:
                    return this.DeserializeObjectArray(din);
                case 101:
                    return this.DeserializeEventData(din, null);
                case 113:
                    return this.DeserializeOperationRequest(din);
                case 112:
                    return this.DeserializeOperationResponse(din);
                case 0:
                case 42:
                    return null;
                default:
                    throw new Exception("Deserialize(): " + type + " pos: " + din.IntPosition + " bytes: " + din.IntLength + ". " + SupportClass.ByteArrayToString(din.GetBuffer()));
            }
        }

        public override byte DeserializeByte(StreamBuffer din)
        {
            return din.ReadByteAsByte();
        }

        private bool DeserializeBoolean(StreamBuffer din)
        {
            return din.ReadByteAsByte() != 0;
        }

        public override short DeserializeShort(StreamBuffer din)
        {
            byte[] obj = this.memShort;
            lock (obj)
            {
                byte[] array = this.memShort;
                din.Read(array, 0, 2);
                return (short)(array[0] << 8 | array[1]);
            }
        }

        private int DeserializeInteger(StreamBuffer din)
        {
            byte[] obj = this.memInteger;
            lock (obj)
            {
                byte[] array = this.memInteger;
                din.Read(array, 0, 4);
                return array[0] << 24 | array[1] << 16 | array[2] << 8 | array[3];
            }
        }

        private long DeserializeLong(StreamBuffer din)
        {
            byte[] obj = this.memLong;
            lock (obj)
            {
                byte[] array = this.memLong;
                din.Read(array, 0, 8);
                if (BitConverter.IsLittleEndian)
                {
                    return (long)((ulong)array[0] << 56 | (ulong)array[1] << 48 | (ulong)array[2] << 40 | (ulong)array[3] << 32 | (ulong)array[4] << 24 | (ulong)array[5] << 16 | (ulong)array[6] << 8 | array[7]);
                }
                return BitConverter.ToInt64(array, 0);
            }
        }

        private float DeserializeFloat(StreamBuffer din)
        {
            lock (memFloat)
            {
                byte[] array = this.memFloat;
                din.Read(array, 0, 4);
                if (BitConverter.IsLittleEndian)
                {
                    byte b = array[0];
                    byte b2 = array[1];
                    array[0] = array[3];
                    array[1] = array[2];
                    array[2] = b2;
                    array[3] = b;
                }
                return BitConverter.ToSingle(array, 0);
            }
        }

        private double DeserializeDouble(StreamBuffer din)
        {
            byte[] obj = this.memDouble;
            lock (obj)
            {
                byte[] array = this.memDouble;
                din.Read(array, 0, 8);
                if (BitConverter.IsLittleEndian)
                {
                    byte b = array[0];
                    byte b2 = array[1];
                    byte b3 = array[2];
                    byte b4 = array[3];
                    array[0] = array[7];
                    array[1] = array[6];
                    array[2] = array[5];
                    array[3] = array[4];
                    array[4] = b4;
                    array[5] = b3;
                    array[6] = b2;
                    array[7] = b;
                }
                return BitConverter.ToDouble(array, 0);
            }
        }

        private string DeserializeString(StreamBuffer din)
        {
            short num = this.DeserializeShort(din);
            if (num == 0)
            {
                return string.Empty;
            }
            if (this.memString == null || this.memString.Length < num)
            {
                this.memString = new byte[num];
            }
            din.Read(this.memString, 0, num);
            return Encoding.UTF8.GetString(this.memString, 0, num);
        }

        private Array DeserializeArray(StreamBuffer din)
        {
            short num = this.DeserializeShort(din);
            byte b = din.ReadByteAsByte();
            Array array = null;
            switch (b)
            {
                case 121:
                    {
                        Array array3 = this.DeserializeArray(din);
                        Type type = array3.GetType();
                        array = Array.CreateInstance(type, num);
                        array.SetValue(array3, 0);
                        for (short num4 = 1; num4 < num; num4 = (short)(num4 + 1))
                        {
                            array3 = this.DeserializeArray(din);
                            array.SetValue(array3, num4);
                        }
                        goto IL_0226;
                    }
                case 120:
                    array = Array.CreateInstance(typeof(byte[]), num);
                    for (short num5 = 0; num5 < num; num5 = (short)(num5 + 1))
                    {
                        Array value = this.DeserializeByteArray(din, -1);
                        array.SetValue(value, num5);
                    }
                    goto IL_0226;
                case 98:
                    array = this.DeserializeByteArray(din, num);
                    goto IL_0226;
                case 105:
                    array = this.DeserializeIntArray(din, num);
                    goto IL_0226;
                case 99:
                    {
                        byte b2 = din.ReadByteAsByte();
                        CustomType customType = default(CustomType);
                        if (Protocol.CodeDict.TryGetValue(b2, out customType))
                        {
                            array = Array.CreateInstance(customType.Type, num);
                            for (int i = 0; i < num; i++)
                            {
                                short num3 = this.DeserializeShort(din);
                                if (customType.DeserializeStreamFunction == null)
                                {
                                    byte[] array2 = new byte[num3];
                                    din.Read(array2, 0, num3);
                                    array.SetValue(customType.DeserializeFunction(array2), i);
                                }
                                else
                                {
                                    array.SetValue(customType.DeserializeStreamFunction(din, num3), i);
                                }
                            }
                            goto IL_0226;
                        }
                        throw new Exception("Cannot find deserializer for custom type: " + b2);
                    }
                case 68:
                    {
                        Array result = null;
                        this.DeserializeDictionaryArray(din, num, out result);
                        return result;
                    }
                default:
                    {
                        array = this.CreateArrayByType(b, num);
                        for (short num2 = 0; num2 < num; num2 = (short)(num2 + 1))
                        {
                            array.SetValue(this.Deserialize(din, b), num2);
                        }
                        goto IL_0226;
                    }
                IL_0226:
                    return array;
            }
        }

        private byte[] DeserializeByteArray(StreamBuffer din, int size = -1)
        {
            if (size == -1)
            {
                size = this.DeserializeInteger(din);
            }
            byte[] array = new byte[size];
            din.Read(array, 0, size);
            return array;
        }

        private int[] DeserializeIntArray(StreamBuffer din, int size = -1)
        {
            if (size == -1)
            {
                size = this.DeserializeInteger(din);
            }
            int[] array = new int[size];
            for (int i = 0; i < size; i++)
            {
                array[i] = this.DeserializeInteger(din);
            }
            return array;
        }

        private string[] DeserializeStringArray(StreamBuffer din)
        {
            int num = this.DeserializeShort(din);
            string[] array = new string[num];
            for (int i = 0; i < num; i++)
            {
                array[i] = this.DeserializeString(din);
            }
            return array;
        }

        private object[] DeserializeObjectArray(StreamBuffer din)
        {
            short num = this.DeserializeShort(din);
            object[] array = new object[num];
            for (int i = 0; i < num; i++)
            {
                byte type = din.ReadByteAsByte();
                array[i] = this.Deserialize(din, type);
            }
            return array;
        }

        private ExitGames.Client.Photon.Hashtable DeserializeHashTable(StreamBuffer din)
        {
            int num = this.DeserializeShort(din);
            ExitGames.Client.Photon.Hashtable hashtable = new ExitGames.Client.Photon.Hashtable(num);
            for (int i = 0; i < num; i++)
            {
                object key = this.Deserialize(din, din.ReadByteAsByte());
                object obj2 = hashtable[key] = this.Deserialize(din, din.ReadByteAsByte());
            }
            return hashtable;
        }

        private IDictionary DeserializeDictionary(StreamBuffer din)
        {
            byte b = din.ReadByteAsByte();
            byte b2 = din.ReadByteAsByte();
            int num = this.DeserializeShort(din);
            bool flag = b == 0 || b == 42;
            bool flag2 = b2 == 0 || b2 == 42;
            Type typeOfCode = this.GetTypeOfCode(b);
            Type typeOfCode2 = this.GetTypeOfCode(b2);
            Type type = typeof(Dictionary<,>).MakeGenericType(typeOfCode, typeOfCode2);
            IDictionary dictionary = Activator.CreateInstance(type) as IDictionary;
            for (int i = 0; i < num; i++)
            {
                object key = this.Deserialize(din, flag ? din.ReadByteAsByte() : b);
                object value = this.Deserialize(din, flag2 ? din.ReadByteAsByte() : b2);
                dictionary.Add(key, value);
            }
            return dictionary;
        }

        private bool DeserializeDictionaryArray(StreamBuffer din, short size, out Array arrayResult)
        {
            byte b = default(byte);
            byte b2 = default(byte);
            Type type = this.DeserializeDictionaryType(din, out b, out b2);
            arrayResult = Array.CreateInstance(type, size);
            for (short num = 0; num < size; num = (short)(num + 1))
            {
                IDictionary dictionary = Activator.CreateInstance(type) as IDictionary;
                if (dictionary == null)
                {
                    return false;
                }
                short num2 = this.DeserializeShort(din);
                for (int i = 0; i < num2; i++)
                {
                    object key;
                    if (b != 0)
                    {
                        key = this.Deserialize(din, b);
                    }
                    else
                    {
                        byte type2 = din.ReadByteAsByte();
                        key = this.Deserialize(din, type2);
                    }
                    object value;
                    if (b2 != 0)
                    {
                        value = this.Deserialize(din, b2);
                    }
                    else
                    {
                        byte type3 = din.ReadByteAsByte();
                        value = this.Deserialize(din, type3);
                    }
                    dictionary.Add(key, value);
                }
                arrayResult.SetValue(dictionary, num);
            }
            return true;
        }

        private Type DeserializeDictionaryType(StreamBuffer reader, out byte keyTypeCode, out byte valTypeCode)
        {
            keyTypeCode = reader.ReadByteAsByte();
            valTypeCode = reader.ReadByteAsByte();
            GpTypeV16 gpType = (GpTypeV16)keyTypeCode;
            GpTypeV16 gpType2 = (GpTypeV16)valTypeCode;
            Type type = (gpType != 0) ? this.GetTypeOfCode(keyTypeCode) : typeof(object);
            Type type2 = (gpType2 != 0) ? this.GetTypeOfCode(valTypeCode) : typeof(object);
            return typeof(Dictionary<,>).MakeGenericType(type, type2);
        }
    }
}
