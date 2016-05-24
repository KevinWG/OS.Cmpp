using System;
using System.Text;

namespace OS.Cmpp.CMPP2.Entitis
{
    public class SubmitReq : BaseMsg
    {
        #region   长度

        public const int FixedBodyLength = 8+ 1+ 1+ 1+ 1+ 10+ 1+ 21+ 1+ 1+ 1+ 6+ 2+ 6+ 17+ 17+ 21+ 1
            //+ 21*DestUsr_tl
                                           + 1
            //+ Msg_length
                                           + 8;

        #endregion

        #region  属性

        /// <summary>
        /// 8 Unsigned Integer 信息标识。
        /// </summary>
        public ulong MsgId { get; set; }

        /// <summary>
        /// 1 Unsigned Integer 相同Msg_Id的信息总条数,从1开始。
        /// </summary>
        public uint PkTotal { get; set; }

        /// <summary>
        /// 1 Unsigned Integer 相同Msg_Id的信息序号,从1开始。
        /// </summary>
        public uint PkNumber { get; set; }

        /// <summary>
        /// 1 Unsigned Integer 是否要求返回状态确认报告:
        ///   0:不需要;
        ///   1:需要。
        /// </summary>
        public uint RegisteredDelivery { get; set; }


        /// <summary>
        /// 1 Unsigned Integer 信息级别。
        /// </summary>
        public uint MsgLevel { get; set; }

        /// <summary>
        /// 10 Octet String 业务标识,是数字、字母和符号的组合。
        /// </summary>
        public string ServiceId { get; set; }

        /// <summary>
        /// 1 Unsigned Integer 计费用户类型字段:
        ///   0:对目的终端MSISDN计费;
        ///   1:对源终端MSISDN计费;
        ///   2:对SP计费;
        ///   3:表示本字段无效,对谁计费参见Fee_terminal_Id字段。
        /// </summary>
        public uint FeeUserType { get; set; }

        /// <summary>
        ///  21 Octet String 被计费用户的号码,当Fee_UserType为3时该值有效,当Fee_UserType为0、1、2时该值无意义。
        /// </summary>
        public string FeeTerminalId { get; set; }
                  
        /// <summary>
        ///  1 Unsigned Integer GSM协议类型。详细是解释请参考GSM03.40中的9.2.3.9。
        /// </summary>
        public uint TpPId { get; set; }

        /// <summary>
        ///  1 Unsigned Integer GSM协议类型。详细是解释请参考GSM03.40中的9.2.3.23,仅使用1位,右对齐。
        /// </summary>
        public uint TpUdhi { get; set; }

        /// <summary>
        ///  1 Unsigned Integer 信息格式:
        ///   0:ASCII串;
        ///   3:短信写卡操作;
        ///   4:二进制信息;
        ///   8:UCS2编码;
        ///   15:含GB汉字......
        /// </summary>
        public uint MsgFmt { get; set; }


        /// <summary>
        /// 6 Octet String 信息内容来源(SP_Id)。
        /// </summary>
        public string MsgSrc { get; set; }

        /// <summary>
        /// 2 Octet String 资费类别:
        ///   01:对"计费用户号码"免费;
        ///   02:对"计费用户号码"按条计信息费;
        ///   03:对"计费用户号码"按包月收取信息费。
        /// </summary>
        public string FeeType { get; set; }

        /// <summary>
        /// 6 Octet String 资费代码(以分为单位)。
        /// </summary>
        public string FeeCode { get; set; }

        /// <summary>
        /// 17 Octet String 存活有效期,格式遵循SMPP3.3协议。
        /// </summary>
        public string ValIdTime { get; set; }

        /// <summary>
        /// 17 Octet String 定时发送时间,格式遵循SMPP3.3协议。
        /// </summary>
        public string AtTime { get; set; }

        /// <summary>
        /// 21 Octet String 源号码。SP的服务代码或前缀为服务代码的长号码,
        ///  网关将该号码完整的填到SMPP协议Submit_SM消息相应的source_addr字段,
        ///  该号码最终在用户手机上显示为短消息的主叫号码。
        /// </summary>
        public string SrcId { get; set; }

        /// <summary>
        /// 1 Unsigned Integer 接收信息的用户数量(小于100个用户)。
        /// </summary>
        public uint DestUsrTl { get; set; }

        /// <summary>
        /// 21*DestUsr_tl Octet String 接收短信的MSISDN号码。
        /// </summary>
        public string[] DestId { get; set; }


        /// <summary>
        /// Msg_length Octet String 信息内容。
        /// </summary>
        public string MsgContent { get; set; }

        /// <summary>
        /// 8 保留
        /// </summary>
        public string Reserve { get; set; }

        #endregion

        public SubmitReq() : base(CommandType.Submit)
        {

        }

        public override byte[] ToBytes()
        {
            //Msg_Length Msg_Content
            uint destTerminalIdLength = (uint) DestId.Length*21;
            var msgContentBytes = GetContentByte(MsgFmt);

            MsgHeader.TotalLength = (uint)(MsgHeaderEnt.HeaderLength + FixedBodyLength + destTerminalIdLength + msgContentBytes.Length);

            int i = 0;
            byte[] bytes = new byte[MsgHeader.TotalLength];

            byte[] buffer = MsgHeader.ToHeaderBytes();
            Buffer.BlockCopy(buffer, 0, bytes, 0, buffer.Length);
            i += MsgHeaderEnt.HeaderLength;

            //Msg_Id //8 [12,19]   //  发送时不需要填写
            //buffer = BitConverter.GetBytes(MsgId);
            //Array.Reverse(buffer);
            //Buffer.BlockCopy(buffer, 0, bytes, i, buffer.Length); 

            //_Pk_total
            i += 8;
            bytes[i++] = (byte) PkTotal; //[20,20]
            bytes[i++] = (byte) PkNumber; //[21,21]
            bytes[i++] = (byte) RegisteredDelivery; //[22,22]
            bytes[i++] = (byte) MsgLevel; //[23,23]

            //Service_Id
            AddAsciiByteToBytes(ServiceId, bytes, i);
  
            //Fee_UserType
            i += 10;
            bytes[i++] = (byte) FeeUserType; //[34,34]
             
            //Fee_terminal_Id   21
            AddAsciiByteToBytes(FeeTerminalId, bytes, i);

            //
            i += 21;
            bytes[i++] = (byte)TpPId; //[56,56]
            bytes[i++] = (byte)TpUdhi; //[57,57]
            bytes[i++] = (byte)MsgFmt; //[58,58]

            //Msg_src
            AddAsciiByteToBytes(MsgSrc, bytes, i);

            //FeeType
            i += 6;
            AddAsciiByteToBytes(FeeType, bytes, i);

            //FeeCode
            i += 2;
            AddAsciiByteToBytes(FeeCode, bytes, i);
     

            //ValId_Time
            i += 6;
            AddAsciiByteToBytes(ValIdTime, bytes, i);

            //At_Time
            i += 17;
            AddAsciiByteToBytes(AtTime, bytes, i);
         
            //Src_Id
            i += 17;
            AddAsciiByteToBytes(SrcId, bytes, i);
         
            //DestUsr_tl
            i += 21;
            DestUsrTl = (uint) DestId.Length;
            bytes[i++] = (byte) DestUsrTl; //[128,128]

            //Dest_terminal_Id
            foreach (string s in DestId)
            {
                if (string.IsNullOrEmpty(s))
                    continue;

                AddAsciiByteToBytes(s, bytes, i);
                i += 21;
            }

            //Msg_Length
            bytes[i++] = (byte) msgContentBytes.Length;

            //Msg_Content
            Buffer.BlockCopy(msgContentBytes, 0, bytes, i, msgContentBytes.Length);

            //Reserve
            i += msgContentBytes.Length;
            AddAsciiByteToBytes(Reserve, bytes, i);

            return bytes;
        }

        /// <summary>
        /// 获取内容字节流
        /// </summary>
        /// <param name="msgFmt"></param>
        /// <returns></returns>
        private byte[] GetContentByte(uint msgFmt)
        {
            //byte[] buf;
            switch (msgFmt)
            {
                case 8:
                    return Encoding.BigEndianUnicode.GetBytes(MsgContent);
                    break;
                case 15: //gb2312
                    return Encoding.GetEncoding("gbk").GetBytes(MsgContent);
                    break;
                default:
                    return Encoding.ASCII.GetBytes(MsgContent);
                    break;
            }
        }

    }



    public class SubmitResp : BaseMsg //: CMPP_Response
    {
        /// <summary>
        ///   消息ID
        /// </summary>
        public ulong MsgId { get; set; }

        /// <summary>
        ///    结果
        /// </summary>
        public uint Result { get; set; }

        //public const int BodyLength = 8 + 4;


        public SubmitResp(byte[] bodyBytes, MsgHeaderEnt header)
            : base(bodyBytes, header)
        {

        }

        protected override void FormatContentBytes(byte[] bodyBytes)
        {
            int i = 0;

            //Msg_Id
            byte[] buffer = new byte[8];
            Buffer.BlockCopy(bodyBytes, i, buffer, 0, buffer.Length);
            MsgId = Byte8ToULong(buffer);

            //Result
            i += 8;
            Result = (uint)bodyBytes[i];
        }
    }
}
