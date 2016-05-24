using System;

namespace OS.Cmpp.CMPP2.Entitis
{
    //public class DeliverReq //: CMPP_Response
    //{
    //    private MsgHeaderEnt _Header;
    //    private ulong _Msg_Id;
    //    private uint _Result;
    //    public const int Bodylength = 8 + 4;
    //    public DeliverResp(ulong Msg_Id, uint Result)
    //    {
    //        _Msg_Id = Msg_Id;
    //        _Result = Result;
    //        _Header = new MsgHeaderEnt(MsgHeaderEnt.Length + Bodylength, CommandType.DeliverResp, 0 );
    //    }
    //    public byte[] ToBytes()
    //    {
    //        int i = 0;
    //        byte[] bytes = new byte[MsgHeaderEnt.Length + Bodylength];
    //        byte[] buffer = new byte[MsgHeaderEnt.Length];
    //        //header
    //        buffer = _Header.ToHeaderBytes();
    //        Buffer.BlockCopy(buffer, 0, bytes, 0, buffer.Length);
    //        i += MsgHeaderEnt.Length;
    //        //msg_id 8
    //        buffer = BitConverter.GetBytes(_Msg_Id);
    //        Array.Reverse(buffer);
    //        buffer.CopyTo(bytes, i);
    //        //result 4
    //        i += 8;
    //        buffer = BitConverter.GetBytes(_Result);
    //        Array.Reverse(buffer);
    //        buffer.CopyTo(bytes, i);
    //        return bytes;
    //    }
    //    public override string ToString()
    //    {
    //        return _Header.ToString() + "\r\n"
    //                + string.Format
    //                            (
    //                                "[\r\nMessageBody:"
    //                                + "\r\n\tMsg_Id: {0}"
    //                                + "\r\n\tResult: [{1}]"
    //                                + "\r\n]"
    //                                , _Msg_Id
    //                                , _Result
    //                            );
    //    }
    //}
    public class DeliverResp : BaseMsg //: CMPP_Request
    {

        /// <summary>
        /// 8 Unsigned Integer 信息标识。
        /// 生成算法如下：
        ///   采用64位（8字节）的整数：
        ///   （1）时间（格式为MMDDHHMMSS，即月日时分秒）：bit64~bit39，其中
        ///     bit64~bit61：月份的二进制表示；
        ///     bit60~bit56：日的二进制表示；
        ///     bit55~bit51：小时的二进制表示；
        ///     bit50~bit45：分的二进制表示；
        ///     bit44~bit39：秒的二进制表示；
        ///   （2）短信网关代码：bit38~bit17，把短信网关的代码转换为整数填写到该字段中。
        ///   （3）序列号：bit16~bit1，顺序增加，步长为1，循环使用。
        ///     各部分如不能填满，左补零，右对齐。
        ///   各部分如不能填满,左补零,右对齐。
        /// </summary>
        public ulong MsgId { get; private set; }


        /// <summary>
        /// 21 Octet String 目的号码。
        ///  SP的服务代码，一般4--6位，或者是前缀为服务代码的长号码；该号码是手机用户短消息的被叫号码。
        /// </summary>
        public string DestId { get; private set; }

        //   SP的服务代码,一般4--6位,或者是前缀为服务代码的长号码;该号码是手机用户短消息的被叫号码。


        /// <summary>
        /// 10 Octet String 业务标识,是数字、字母和符号的组合。
        /// </summary>
        public string ServiceId { get; private set; }


        /// <summary>
        /// 1 Unsigned Integer GSM协议类型。详细解释请参考GSM03.40中的9.2.3.9。
        /// </summary>
        public uint _TP_pid { get; private set; }


        /// <summary>
        /// 1 Unsigned Integer GSM协议类型。详细解释请参考GSM03.40中的9.2.3.23,仅使用1位,右对齐。
        /// </summary>
        public uint _TP_udhi { get; private set; }

        /// <summary>
        /// 1 Unsigned Integer 信息格式:
        ///   0:ASCII串;
        ///   3:短信写卡操作;
        ///   4:二进制信息;
        ///   8:UCS2编码;
        ///   15:含GB汉字。
        /// </summary>
        public uint _Msg_Fmt { get; private set; }


        /// <summary>
        /// 21 Octet String 源终端MSISDN号码(状态报告时填为CMPP_SUBMIT消息的目的终端号码)。
        /// </summary>
        public string _Src_terminal_Id { get; private set; }



        /// <summary>
        /// 1 Unsigned Integer 是否为状态报告:
        ///   0:非状态报告;
        ///   1:状态报告。
        /// </summary>
        public uint _Registered_Delivery { get; private set; }


        /// <summary>
        /// 1 Unsigned Integer 消息长度,取值大于或等于0。
        /// </summary>
        public uint _Msg_Length { get; private set; }

        /// <summary>
        /// Msg_length Octet String 消息内容。
        /// </summary>
        public string _Msg_Content { get; private set; }


        /// <summary>
        /// 8 Octet String 保留项。
        /// </summary>
        public string Reserved { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bodyBytes"></param>
        public DeliverResp(byte[] bodyBytes, MsgHeaderEnt header)
            : base(bodyBytes, header)
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="allBytes"></param>
        protected override void FormatContentBytes(byte[] allBytes)
        {
            throw new NotImplementedException();
        }


        //public DeliverReq(byte[] bytes)
        //{
        //    int i = 0;
        //    byte[] buffer = new byte[MsgHeaderEnt.Length];
        //    Buffer.BlockCopy(bytes, 0, buffer, 0, MsgHeaderEnt.Length);
        //    _Header = new MsgHeaderEnt(buffer);
        //    //Msg_Id 8
        //    i += MsgHeaderEnt.Length;
        //    buffer = new byte[8];
        //    Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
        //    Array.Reverse(buffer);
        //    MsgId = BitConverter.ToUInt64(buffer, 0);
        //    string s = null;
        //    //Dest_Id 21
        //    i += 8;
        //    buffer = new byte[21];
        //    Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
        //    s = Encoding.ASCII.GetString(buffer).Trim('\0');
        //    //s = s.Substring(0, s.IndexOf('\0'));
        //    DestId = s;
        //    //Service_Id 20
        //    i += 21;
        //    buffer = new byte[10];
        //    Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
        //    s = Encoding.ASCII.GetString(buffer).Trim('\0');
        //    //s = s.Substring(0, s.IndexOf('\0'));
        //    ServiceId = s;
        //    //TP_pid 1
        //    i += 10;
        //    _TP_pid = (uint)bytes[i++];
        //    _TP_udhi = (uint)bytes[i++];
        //    _Msg_Fmt = (uint)bytes[i++];
        //    //Src_terminal_Id 32
        //    buffer = new byte[32];
        //    Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
        //    s = Encoding.ASCII.GetString(buffer).Trim('\0');
        //    //s = s.Substring(0, s.IndexOf('\0'));
        //    _Src_terminal_Id = s;
        //    //Src_terminal_type 1
        //    i += 32;
        //    _Src_terminal_type = (uint)bytes[i++];
        //    _Registered_Delivery = (uint)bytes[i++];
        //    _Msg_Length = (uint)bytes[i++];
        //    //Msg_Content
        //    buffer = new byte[_Msg_Length];
        //    Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
        //    switch (_Msg_Fmt)
        //    {
        //        case 8:
        //            _Msg_Content = Encoding.BigEndianUnicode.GetString(buffer).Trim('\0');
        //            break;
        //        case 15: //gb2312
        //            _Msg_Content = Encoding.GetEncoding("gb2312").GetString(buffer).Trim('\0');
        //            break;
        //        case 0: //ascii
        //        case 3: //短信写卡操作
        //        case 4: //二进制信息
        //        default:
        //            _Msg_Content = Encoding.ASCII.GetString(buffer).Trim('\0');
        //            break;
        //    }
        //    //Linkid 20
        //    i += (int)_Msg_Length;
        //    buffer = new byte[20];
        //    Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
        //    s = Encoding.ASCII.GetString(buffer).Trim('\0');
        //    //s = s.Substring(0, s.IndexOf('\0'));
        //    _LinkID = s;
        //}
        //public byte[] ToBytes()
        //{
        //    //Msg_Length Msg_Content
        //    byte[] buf;
        //    switch (_Msg_Fmt)
        //    {
        //        case 8:
        //            buf = Encoding.BigEndianUnicode.GetBytes(_Msg_Content);
        //            break;
        //        case 15: //gb2312
        //            buf = Encoding.GetEncoding("gb2312").GetBytes(_Msg_Content);
        //            break;
        //        case 0: //ascii
        //        case 3: //短信写卡操作
        //        case 4: //二进制信息
        //        default:
        //            buf = Encoding.ASCII.GetBytes(_Msg_Content);
        //            break;
        //    }
        //    _Msg_Length = (uint)buf.Length;
        //    _BodyLength = FixedBodyLength + (int)_Msg_Length;
        //    byte[] bytes = new byte[MsgHeaderEnt.Length + _BodyLength];
        //    int i = 0;
        //    byte[] buffer = null;
        //    //header 12
        //    _Header = new MsgHeaderEnt
        //                        (
        //                            (uint)(MsgHeaderEnt.Length + _BodyLength)
        //                            , CommandType.CMPP_DELIVER
        //                            , 0
        //                        );
        //    //Msg_Id 8
        //    i += MsgHeaderEnt.Length;
        //    buffer = new byte[8];
        //    buffer = BitConverter.GetBytes(MsgId);
        //    Array.Reverse(buffer);
        //    Buffer.BlockCopy(buffer, 0, bytes, i, buffer.Length);
        //    //Dest_Id 21
        //    i += 8;
        //    buffer = new byte[21];
        //    buffer = Encoding.ASCII.GetBytes(DestId);
        //    Buffer.BlockCopy(buffer, 0, bytes, i, buffer.Length);
        //    //Service_Id 10
        //    i += 21;
        //    buffer = new byte[10];
        //    buffer = Encoding.ASCII.GetBytes(ServiceId);
        //    Buffer.BlockCopy(buffer, 0, bytes, i, buffer.Length);
        //    //TP_pid 1
        //    i += 10;
        //    bytes[i++] = (byte)_TP_pid;
        //    bytes[i++] = (byte)_TP_udhi;
        //    bytes[i++] = (byte)_Msg_Fmt;
        //    //Src_terminal_Id 32
        //    buffer = new byte[32];
        //    buffer = Encoding.ASCII.GetBytes(_Src_terminal_Id);
        //    Buffer.BlockCopy(buffer, 0, bytes, i, buffer.Length);
        //    //Src_terminal_type 1
        //    i += 32;
        //    bytes[i++] = (byte)_Src_terminal_type;
        //    bytes[i++] = (byte)_Registered_Delivery;
        //    bytes[i++] = (byte)_Msg_Length;
        //    //Msg_Content
        //    Buffer.BlockCopy(buf, 0, bytes, i, buf.Length);
        //    //LinkID
        //    i += (int)_Msg_Length;
        //    return bytes;
        //}



    }
}
