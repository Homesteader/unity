using System;

namespace Engine
{
	class Message : ByteStream
	{
        private int mMsgType = 0;
        private int mMsgLen = 0;
        private int mConnectSerial = 0;
        private bool mRead;

        public static int MessageHeadSize = 6;    // Length of message packet header.
        
        /// <summary>
        /// Message Type
        /// </summary>
        public int MsgType
        {
            get
            {
                return mMsgType;
            }
            set
            {
                byte[] newType = BitConverter.GetBytes(value);
                Array.Copy(newType, 0, this.Bytes, 0, 4);
            }
        }

        /// <summary>
        /// Message Length
        /// </summary>
        public int MsgLen
        {
            get
            {
                if (mRead)
                {
                    return mMsgLen;
                }
                return this.Length - Message.MessageHeadSize;
            }
        }

        /// <summary>
        /// The connection serial no. default, client donnot have the field.
        /// </summary>
        public int ConnectSerial
        {
            get
            {
                return mConnectSerial;
            }
            set
            {
                byte[] newConnectSeiral = BitConverter.GetBytes(value);
                Array.Copy(newConnectSeiral, 0, this.Bytes, 8, 4);
            }
        }

        public Message(int msgType)            
        {
            mRead = false;
            mMsgType = msgType;            
            this.Write(msgType);
            this.Write(0);
            this.Write(0);                       //ConnectSeiral, client all is 0, for occupy the postion.
        }

        public Message(byte[] msgData)
            : base(msgData, msgData.Length)
        {
            mRead = true;
            this.Read(ref mMsgType);
            this.Read(ref mMsgLen);
            this.Read(ref mConnectSerial);   
        }



        /// <summary>
        /// return the message data.
        /// </summary>
        public byte[] Data
        {
            get
            {
                byte[] bodyLen = BitConverter.GetBytes(MsgLen);
                Array.Copy(bodyLen, 0, this.Bytes, 4, 4);
                byte[] realPakcet = new byte[this.Length];
                Array.Copy(this.Bytes, realPakcet, realPakcet.Length);

                return realPakcet;
            }
        }

	}
}
