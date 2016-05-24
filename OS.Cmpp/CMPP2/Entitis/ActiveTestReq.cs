using System;

namespace OS.Cmpp.CMPP2.Entitis
{
    public class ActiveTestReq : BaseMsg
    {

        /// <summary>
        ///   运营商发送请求过来处理
        /// </summary>
        /// <param name="bodyBytes"></param>
        /// <param name="header"></param>
        public ActiveTestReq(byte[] bodyBytes, MsgHeaderEnt header):
            base(header)
        {
           
        }

        /// <summary>
        ///   主动发送给运营商
        /// </summary>
        public ActiveTestReq() : base(CommandType.ActiveTest)
        {

        }

        /// <summary>
        ///   
        /// </summary>
        /// <returns></returns>
        public override byte[] ToBytes()
        {
            MsgHeader.TotalLength = MsgHeaderEnt.HeaderLength;
            return MsgHeader.ToHeaderBytes();
        }
    }


    public class ActiveTestResp : BaseMsg
    {
        public ActiveTestResp(uint seqId) : base(CommandType.ActiveTestResp)
        {
            MsgHeader.SequenceId = seqId;
        }

        public ActiveTestResp(byte[] bodyBytes, MsgHeaderEnt header)
            : base(bodyBytes, header)
        {

        }

  
        public override byte[] ToBytes()
        {
            MsgHeader.TotalLength = MsgHeaderEnt.HeaderLength + 1;

            int i = 0;
            byte[] bytes = new byte[MsgHeader.TotalLength];


            byte[] buffer = MsgHeader.ToHeaderBytes();
            Buffer.BlockCopy(buffer, 0, bytes, 0, buffer.Length);
            i += MsgHeaderEnt.HeaderLength;

            //Reserved
            bytes[i] = 0;

            return bytes;
        }

     
    }
}
