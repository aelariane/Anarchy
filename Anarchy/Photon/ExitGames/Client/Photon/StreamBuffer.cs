using System;
using System.IO;

namespace ExitGames.Client.Photon
{
    public class StreamBuffer
    {
        private const int DefaultInitialSize = 0;

        private int pos;

        private int len;

        private byte[] buf;

        public bool CanRead
        {
            get
            {
                return true;
            }
        }

        public bool CanSeek
        {
            get
            {
                return true;
            }
        }

        public bool CanWrite
        {
            get
            {
                return true;
            }
        }

        public int Length
        {
            get
            {
                return this.len;
            }
        }

        public int Position
        {
            get
            {
                return this.pos;
            }
            set
            {
                this.pos = value;
                if (this.len < this.pos)
                {
                    this.len = this.pos;
                    this.CheckSize(this.len);
                }
            }
        }

        public StreamBuffer(int size = 0)
        {
            this.buf = new byte[size];
        }

        public StreamBuffer(byte[] buf)
        {
            this.buf = buf;
            this.len = buf.Length;
        }

        public byte[] ToArray()
        {
            byte[] array = new byte[this.len];
            Buffer.BlockCopy(this.buf, 0, array, 0, this.len);
            return array;
        }

        public byte[] ToArrayFromPos()
        {
            int num = this.len - this.pos;
            if (num <= 0)
            {
                return new byte[0];
            }
            byte[] array = new byte[num];
            Buffer.BlockCopy(this.buf, this.pos, array, 0, num);
            return array;
        }

        public void Compact()
        {
            long num = this.Length - this.Position;
            if (num > 0)
            {
                Buffer.BlockCopy(this.buf, this.Position, this.buf, 0, (int)num);
            }
            this.Position = 0;
            this.SetLength(num);
        }

        public byte[] GetBuffer()
        {
            return this.buf;
        }

        public byte[] GetBufferAndAdvance(int length, out int offset)
        {
            offset = this.Position;
            this.Position += length;
            return this.buf;
        }

        public void Flush()
        {
        }

        public long Seek(long offset, SeekOrigin origin)
        {
            int num = 0;
            switch (origin)
            {
                case SeekOrigin.Begin:
                    num = (int)offset;
                    break;
                case SeekOrigin.Current:
                    num = this.pos + (int)offset;
                    break;
                case SeekOrigin.End:
                    num = this.len + (int)offset;
                    break;
                default:
                    throw new ArgumentException("Invalid seek origin");
            }
            if (num < 0)
            {
                throw new ArgumentException("Seek before begin");
            }
            if (num > this.len)
            {
                throw new ArgumentException("Seek after end");
            }
            this.pos = num;
            return this.pos;
        }

        public void SetLength(long value)
        {
            this.len = (int)value;
            this.CheckSize(this.len);
            if (this.pos > this.len)
            {
                this.pos = this.len;
            }
        }

        public void SetCapacityMinimum(int neededSize)
        {
            this.CheckSize(neededSize);
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            int num = this.len - this.pos;
            if (num <= 0)
            {
                return 0;
            }
            if (count > num)
            {
                count = num;
            }
            Buffer.BlockCopy(this.buf, this.pos, buffer, offset, count);
            this.pos += count;
            return count;
        }

        public void Write(byte[] buffer, int srcOffset, int count)
        {
            int num = this.pos + count;
            this.CheckSize(num);
            if (num > this.len)
            {
                this.len = num;
            }
            Buffer.BlockCopy(buffer, srcOffset, this.buf, this.pos, count);
            this.pos = num;
        }

        public byte ReadByte()
        {
            if (this.pos >= this.len)
            {
                throw new EndOfStreamException("SteamBuffer.ReadByte() failed. pos:" + this.pos + "len:" + this.len);
            }
            return this.buf[this.pos++];
        }

        public void WriteByte(byte value)
        {
            if (this.pos >= this.len)
            {
                this.len = this.pos + 1;
                this.CheckSize(this.len);
            }
            this.buf[this.pos++] = value;
        }

        public void WriteBytes(byte v0, byte v1)
        {
            int num = this.pos + 2;
            if (this.len < num)
            {
                this.len = num;
                this.CheckSize(this.len);
            }
            this.buf[this.pos++] = v0;
            this.buf[this.pos++] = v1;
        }

        public void WriteBytes(byte v0, byte v1, byte v2)
        {
            int num = this.pos + 3;
            if (this.len < num)
            {
                this.len = num;
                this.CheckSize(this.len);
            }
            this.buf[this.pos++] = v0;
            this.buf[this.pos++] = v1;
            this.buf[this.pos++] = v2;
        }

        public void WriteBytes(byte v0, byte v1, byte v2, byte v3)
        {
            int num = this.pos + 4;
            if (this.len < num)
            {
                this.len = num;
                this.CheckSize(this.len);
            }
            this.buf[this.pos++] = v0;
            this.buf[this.pos++] = v1;
            this.buf[this.pos++] = v2;
            this.buf[this.pos++] = v3;
        }

        public void WriteBytes(byte v0, byte v1, byte v2, byte v3, byte v4, byte v5, byte v6, byte v7)
        {
            int num = this.pos + 8;
            if (this.len < num)
            {
                this.len = num;
                this.CheckSize(this.len);
            }
            this.buf[this.pos++] = v0;
            this.buf[this.pos++] = v1;
            this.buf[this.pos++] = v2;
            this.buf[this.pos++] = v3;
            this.buf[this.pos++] = v4;
            this.buf[this.pos++] = v5;
            this.buf[this.pos++] = v6;
            this.buf[this.pos++] = v7;
        }

        private bool CheckSize(int size)
        {
            if (size <= this.buf.Length)
            {
                return false;
            }
            int num = this.buf.Length;
            if (num == 0)
            {
                num = 1;
            }
            while (size > num)
            {
                num *= 2;
            }
            byte[] dst = new byte[num];
            Buffer.BlockCopy(this.buf, 0, dst, 0, this.buf.Length);
            this.buf = dst;
            return true;
        }
    }
}
