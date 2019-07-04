using System;
using System.Text;

namespace Engine
{
    class ByteStream
    {
        private byte[] _Bytes;
        private int _ptWriter;		// 追加字节的偏移量
        private int _ptReader;		// 读数据的偏移量

        public ByteStream()
        {
            _ptWriter = 0;
            _ptReader = 0;
            _Bytes = new byte[2048];
        }


        public ByteStream(int len)
        {
            _ptWriter = 0;
            _ptReader = 0;
            _Bytes = new byte[len];
        }

        /// <summary>
        /// 使用字节数据创建对象
        /// </summary>
        /// <param name="bytes">字节数组</param>
        /// <param name="writestart">数组中已有数据的长度，关系到续写数据时的光标位置</param>
        public ByteStream(byte[] bytes, int datalen)
        {
            _ptReader = 0;
            _Bytes = new byte[bytes.Length];
            bytes.CopyTo(_Bytes, 0);
            _ptWriter = (datalen >= 0) ? datalen : 0;
        }

        public byte[] Bytes
        {
            get { return _Bytes; }
        }

        /// <summary>
        /// 总长度
        /// </summary>
        public int Capacity
        {
            get { return _Bytes.Length; }
        }

        public int Length
        {
            get { return _ptWriter;  }
        }

        /// <summary>
        /// 获取和设置写光标的位置
        /// </summary>
        public int WriteCursor
        {
            get { return _ptWriter; }
            set { _ptWriter = value; }
        }

        /// <summary>
        /// 获取和设置读光标的位置
        /// </summary>
        public int ReadCursor
        {
            get { return _ptReader; }
            set { _ptReader = value; }
        }

        public void Write(int n)
        {
            byte[] buf = BitConverter.GetBytes(n);
            buf.CopyTo(_Bytes, _ptWriter);
            _ptWriter += buf.Length;
        }
        public void Write(uint n)
        {
            byte[] buf = BitConverter.GetBytes(n);
            buf.CopyTo(_Bytes, _ptWriter);
            _ptWriter += buf.Length;
        }
        public void Write(short n)
        {
            byte[] buf = BitConverter.GetBytes(n);
            buf.CopyTo(_Bytes, _ptWriter);
            _ptWriter += buf.Length;
        }
        public void Write(ushort n)
        {
            byte[] buf = BitConverter.GetBytes(n);
            buf.CopyTo(_Bytes, _ptWriter);
            _ptWriter += buf.Length;
        }
        public void Write(byte b)
        {
            _Bytes[_ptWriter++] = b;
        }

        public void Write(byte[] btValue)
        {
            btValue.CopyTo(_Bytes, _ptWriter);
            _ptWriter += btValue.Length;
        }

        public void Write(byte[] btValue, int nLen)
        {
            btValue.CopyTo(_Bytes, _ptWriter);
            _ptWriter += nLen;
        }

        public void Write(string strValue, string encode)
        {
            byte[] buf = Encoding.GetEncoding(encode).GetBytes(strValue);
            buf.CopyTo(_Bytes, _ptWriter);
            _ptWriter += buf.Length;
            _Bytes[_ptWriter] = 0x00;
        }

        public void Write(string strValue, string encode, int nFixLen)
        {
            byte[] buf = Encoding.GetEncoding(encode).GetBytes(strValue);
            buf.CopyTo(_Bytes, _ptWriter);
            _ptWriter += nFixLen;
            _Bytes[_ptWriter] = 0x00;
        }


        public int Read(ref int val)
        {
            val= BitConverter.ToInt32(_Bytes, _ptReader);
            _ptReader += 4;
            return val;
        }


        public uint Read(ref uint val)
        {
            val  = BitConverter.ToUInt32(_Bytes, _ptReader);
            _ptReader += 4;
            return val;
        }

        public short Read(ref short val)
        {
            val = BitConverter.ToInt16(_Bytes, _ptReader);
            _ptReader += 2;
            return val;
        }

        public ushort Read(ref ushort val)
        {
            val = BitConverter.ToUInt16(_Bytes, _ptReader);
            _ptReader += 2;
            return val;
        }

        public byte Read(ref byte val)
        {
            val = _Bytes[_ptReader++];
            return val;
        }

        public byte[] Read(ref byte[] bytes)
        {            
            Array.Copy(_Bytes, _ptReader, bytes, 0, bytes.Length);
            _ptReader += bytes.Length;
            return bytes;
        }

        public byte[] Read(ref byte[] bytes, int startIdx, int nLen)
        {
            if (bytes.Length - startIdx < nLen) return null;
            Array.Copy(_Bytes, _ptReader, bytes, startIdx, nLen);
            _ptReader += nLen;
            return bytes;
        }

        public string Read(ref string val, string encode)
        {
            int i = _ptReader;
            for (; i < _Bytes.Length && (_Bytes[i] != 0); i++) { }		// 搜索字符串结束符
            val = Encoding.GetEncoding(encode).GetString(_Bytes, _ptReader, i - _ptReader);
            //调整_ptReader = i;因为字符串代表的尾巴是0，我们也需要读取略不过，因此变更为_ptReader = i + 1;
            //_ptReader = i;
            _ptReader = i + 1;
            return val;
        }

        public string Read(ref string val, string encode, int fixLength)
        {
            int i = _ptReader;
            for (; i < _Bytes.Length && (_Bytes[i] != 0); i++) { }		// 搜索字符串结束符
            val = Encoding.GetEncoding(encode).GetString(_Bytes, _ptReader, i - _ptReader);
            //调整_ptReader = i;因为字符串代表的尾巴是0，我们也需要读取略不过，因此变更为_ptReader = i + 1;
            //_ptReader = i;
            _ptReader += fixLength;
            return val;
        }

        public static ByteStream operator +(ByteStream a, ByteStream b)
        {
            if ((a.WriteCursor + b.Capacity) <= a.Capacity)
            {
                b.Bytes.CopyTo(a.Bytes, a.WriteCursor);
                a.WriteCursor += b.WriteCursor;
                return a;
            }
            else
            {
                ByteStream newStream = new ByteStream(a.WriteCursor + b.Capacity);
                a.Bytes.CopyTo(newStream.Bytes, 0);
                b.Bytes.CopyTo(newStream.Bytes, a.WriteCursor);
                newStream.WriteCursor = a.WriteCursor + b.WriteCursor;
                return newStream;
            }
        }
    }
}
