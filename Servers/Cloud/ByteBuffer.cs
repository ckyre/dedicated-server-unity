using System;
using System.Collections.Generic;

namespace Server
{
    public class ByteBuffer : IDisposable
    {
        private List<byte> buff;
        private byte[] readBuff;
        private int readPos;
        private bool buffUpdated = false;

        public ByteBuffer()
        {
            buff = new List<byte>();
            readPos = 0;
        }

        #region Get informations methods
        public int GetReadPosition()
        {
            return readPos;
        }

        public byte[] ToArray()
        {
            return buff.ToArray();
        }

        public int Count()
        {
            return buff.Count;
        }

        public int Length()
        {
            return (Count() - readPos);
        }
        #endregion

        public void Clear()
        {
            buff.Clear();
            readPos = 0;
        }

        #region Writing data
        public void WriteByte(byte input)
        {
            buff.Add(input);
            buffUpdated = true;
        }

        public void WriteBytes(byte[] input)
        {
            buff.AddRange(input);
            buffUpdated = true;
        }

        public void WriteShort(short input)
        {
            buff.AddRange(BitConverter.GetBytes(input));
            buffUpdated = true;
        }

        public void WriteInteger(int input)
        {
            buff.AddRange(BitConverter.GetBytes(input));
            buffUpdated = true;
        }

        public void WriteLong(long input)
        {
            buff.AddRange(BitConverter.GetBytes(input));
            buffUpdated = true;
        }

        public void WriteFloat(float input)
        {
            buff.AddRange(BitConverter.GetBytes(input));
            buffUpdated = true;
        }

        public void WriteBool(bool input)
        {
            buff.AddRange(BitConverter.GetBytes(input));
            buffUpdated = true;
        }

        public void WriteString(string input)
        {
            buff.AddRange(BitConverter.GetBytes(input.Length));
            buff.AddRange(System.Text.Encoding.ASCII.GetBytes(input));
            buffUpdated = true;
        }

        #endregion

        #region Read data
        public byte ReadByte(bool peek = true)
        {
            if (buff.Count > readPos)
            {
                if (buffUpdated)
                {
                    readBuff = buff.ToArray();
                    buffUpdated = false;
                }

                byte value = readBuff[readPos];
                if (peek & buff.Count > readPos)
                {
                    readPos += 1;
                }

                return value;
            }
            else { throw new Exception("You are not trying to read out a byte"); }
        }

        public byte[] ReadBytes(int Lenght, bool peek = true)
        {
            if (buff.Count > readPos)
            {
                if (buffUpdated)
                {
                    readBuff = buff.ToArray();
                    buffUpdated = false;
                }

                byte[] value = buff.GetRange(readPos, Lenght).ToArray();
                if (peek)
                {
                    readPos += 1;
                }

                return value;
            }
            else { throw new Exception("You are not trying to read out a byte[]"); }
        }

        public short ReadShort(bool peek = true)
        {
            if (buff.Count > readPos)
            {
                if (buffUpdated)
                {
                    readBuff = buff.ToArray();
                    buffUpdated = false;
                }

                short value = BitConverter.ToInt16(readBuff, readPos);
                if (peek & buff.Count > readPos)
                {
                    readPos += 2;
                }

                return value;
            }
            else { throw new Exception("You are not trying to read out a short"); }
        }

        public int ReadInteger(bool peek = true)
        {
            if (buff.Count > readPos)
            {
                if (buffUpdated)
                {
                    readBuff = buff.ToArray();
                    buffUpdated = false;
                }

                int value = BitConverter.ToInt32(readBuff, readPos);
                if (peek & buff.Count > readPos)
                {
                    readPos += 4;
                }

                return value;
            }
            else { throw new Exception("You are not trying to read out a int"); }
        }

        public long ReadLong(bool peek = true)
        {
            if (buff.Count > readPos)
            {
                if (buffUpdated)
                {
                    readBuff = buff.ToArray();
                    buffUpdated = false;
                }

                long value = BitConverter.ToInt64(readBuff, readPos);
                if (peek & buff.Count > readPos)
                {
                    readPos += 8;
                }

                return value;
            }
            else { throw new Exception("You are not trying to read out a long"); }
        }

        public float ReadFloat(bool peek = true)
        {
            if (buff.Count > readPos)
            {
                if (buffUpdated)
                {
                    readBuff = buff.ToArray();
                    buffUpdated = false;
                }

                float value = BitConverter.ToSingle(readBuff, readPos);
                if (peek & buff.Count > readPos)
                {
                    readPos += 4;
                }

                return value;
            }
            else { throw new Exception("You are not trying to read out a float"); }
        }

        public bool ReadBool(bool peek = true)
        {
            if (buff.Count > readPos)
            {
                if (buffUpdated)
                {
                    readBuff = buff.ToArray();
                    buffUpdated = false;
                }

                bool value = BitConverter.ToBoolean(readBuff, readPos);
                if (peek & buff.Count > readPos)
                {
                    readPos += 1;
                }

                return value;
            }
            else { throw new Exception("You are not trying to read out a bool"); }
        }

        public string ReadString(bool peek = true)
        {
            try
            {
                int length = ReadInteger();
                if (buffUpdated)
                {
                    readBuff = buff.ToArray();
                    buffUpdated = false;
                }

                string value = System.Text.Encoding.ASCII.GetString(readBuff, readPos, length);
                if (peek & buff.Count > readPos)
                {
                    if (value.Length > 0)
                    {
                        readPos += length;
                    }
                }

                return value;
            }
            catch (Exception e)
            {
                throw new Exception("You are not trying to read out a string");
            }
        }
        #endregion

        private bool disposedValue = false;
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (disposedValue)
                {
                    buff.Clear();
                    readPos = 0;
                }
                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
