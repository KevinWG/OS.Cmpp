using System;
using System.Text;
using OS.Common.ComModels;
using OS.Common.ComModels.Enums;
using OS.Common.Encrypt;
using OS.Common.Extention;

namespace OS.Cmpp.CMPP2.Entitis
{
    internal class ConnectReq : BaseMsg
    {
        /// <summary>
        /// 6 Octet String 源地址,此处为SP_Id,即SP的企业代码。
        /// </summary>
        public string SourceAddr { get; private set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; private set; }

        /// <summary>
        /// 16 Octet String 用于鉴别源地址。其值通过单向MD5 hash计算得出,表示如下:
        ///   MD5(Source_Addr+9 字节的0 +shared secret+timestamp)
        //    Shared secret 由中国移动与源地址实体事先商定,timestamp格式为:MMDDHHMMSS,即月日时分秒,10位。
        /// </summary>
        public byte[] AuthenticatorSource { get; private set; }

        /// <summary>
        ///  1 Unsigned Integer 双方协商的版本号(高位4bit表示主版本号,低位4bit表示次版本号),对于3.0的版本,高4bit为3,低4位为0
        /// </summary>
        public uint Version { get; private set; }

        /// <summary>
        ///  4 Unsigned Integer 时间戳的明文,由客户端产生,格式为MMDDHHMMSS,
        ///  即月日时分秒,10位数字的整型,右对齐 。
        /// </summary>
        public uint Timestamp { get; private set; }


        /// <summary>
        ///   创建连接请求数据实体
        /// </summary>
        /// <param name="sourceAddr">企业代码</param>
        /// <param name="password">密码</param>
        /// <param name="version">版本号</param>
        public ConnectReq(string sourceAddr, string password, uint version = 1)
            : base(CommandType.Connect)
        {
            SourceAddr = sourceAddr;
            Password = password;
            Version = version;

            string strTimestamp = DateTime.Now.ToString("MMddHHmmss");
            Timestamp = strTimestamp.ToUInt32();

            byte[] buffer = new byte[6 + 9 + password.Length + 10];

            Encoding.ASCII.GetBytes(sourceAddr).CopyTo(buffer, 0);
            Encoding.ASCII.GetBytes(password).CopyTo(buffer, 6 + 9);
            Encoding.ASCII.GetBytes(strTimestamp).CopyTo(buffer, 6 + 9 + password.Length);

            AuthenticatorSource = Md5.Encrypt(buffer);
        }



        /// <summary>
        ///  获取请求的字节流
        /// </summary>
        /// <returns></returns>
        public override byte[] ToBytes()
        {
            MsgHeader.TotalLength = MsgHeaderEnt.HeaderLength + 6 + 16 + 1 + 4;

            int i = 0;
            byte[] bytes = new byte[MsgHeader.TotalLength];

            //header 12
            byte[] buffer = MsgHeader.ToHeaderBytes();
            Buffer.BlockCopy(buffer, 0, bytes, i, MsgHeaderEnt.HeaderLength);


            //Source_Addr 6
            i += MsgHeaderEnt.HeaderLength;
            AddAsciiByteToBytes(SourceAddr, bytes, i);

            //AuthenticatorSource 16
            i += 6;
            buffer = AuthenticatorSource;
            Buffer.BlockCopy(buffer, 0, bytes, i, 16); //16

            //version 1
            i += 16;
            bytes[i++] = (byte) Version; //版本

            //Timestamp
            AddUInt4ByteToBytes(Timestamp, bytes, i);

            return bytes;
        }
    }

    public class ConnectResp : BaseMsg //: CMPP_Response
    {

        /// <summary>
        ///   16 Octet String ISMG认证码,用于鉴别ISMG。
        ///   其值通过单向MD5 hash计算得出,表示如下:
        ///   AuthenticatorISMG =MD5(Status+AuthenticatorSource+shared secret),Shared secret 由中国移动与源地址实体事先商定,AuthenticatorSource为源地址实体发送给ISMG的对应消息CMPP_Connect中的值。
        /// 认证出错时,此项为空。
        /// </summary>
        public byte[] AuthenticatorISMG { get; private set; }

        /// <summary>
        ///   返回状态
        ///    0:正确   1:消息结构错   2:非法源地址   3:认证错   4:版本太高   5~:其他错误
        /// </summary>
        public uint Status { get; private set; }

        /// <summary>
        ///   1 Unsigned Integer 服务器支持的最高版本号,对于3.0的版本,高4bit为3,低4位为0
        /// </summary>
        public uint Version { get; private set; }

        public ConnectResp(byte[] bodyBytes, MsgHeaderEnt header)
            : base(bodyBytes, header)
        {
        }

        /// <summary>
        ///   获取返回属性
        /// </summary>
        /// <param name="bodyBytes"></param>
        protected override void FormatContentBytes(byte[] bodyBytes)
        {
            //header 12
            int i = 0;

            //status 1
            Status = bodyBytes[i];

            //AuthenticatorISMG 16
            i += 1;
            AuthenticatorISMG = new byte[16];
            Buffer.BlockCopy(bodyBytes, i, AuthenticatorISMG, 0, AuthenticatorISMG.Length);

            //version
            i += 16;
            Version = bodyBytes[i];
        }


        public override ResultModel GetResult()
        {
            if (Status == 0)
                return new ResultModel();
            else
            {
                string errorMsg = string.Empty;
                switch (Status)
                {
                    case 1:
                        errorMsg = "消息结构错";
                        break;
                    case 2:
                        errorMsg = "非法源地址";
                        break;
                    case 3:
                        errorMsg = "认证错";
                        break;
                    case 4:
                        errorMsg = "版本太高";
                        break;
                    default:
                        errorMsg = "其他错误";
                        break;
                }

                return new ResultModel(ResultTypes.ObjectStateError, errorMsg);
            }
        }
    }
}
