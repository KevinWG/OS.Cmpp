using System;
using System.Text;
using OS.Common.ComModels;

namespace OS.Cmpp.CMPP2.Entitis
{
    /// <summary>
    ///   基础类
    /// </summary>
    public abstract class BaseMsg
    {

        /// <summary>
        /// 头部信息
        /// </summary>
        public MsgHeaderEnt MsgHeader { get; private set; }

        /// <summary>
        ///   
        /// </summary>
        /// <param name="commandType"></param>
        protected BaseMsg(CommandType commandType)
        {
            MsgHeader = new MsgHeaderEnt(commandType);
        }


        /// <summary>
        ///  基类构造函数，响应返回时使用
        /// </summary>
        /// <param name="ent"></param>
        protected BaseMsg(MsgHeaderEnt ent)
        {
            MsgHeader = ent;
        }


        /// <summary>
        ///   基类构造函数
        /// </summary>
        /// <param name="bodyBytes"></param>
        /// <param name="header"></param>
        protected BaseMsg(byte[] bodyBytes, MsgHeaderEnt header)
            : this(header)
        {
            FormatContentBytes(bodyBytes);
        }



        /// <summary>
        ///   获取当前请求的字节
        /// </summary>
        /// <returns></returns>
        public virtual Byte[] ToBytes()
        {
            return null;
        }





        /// <summary>
        ///   获取内容的字节
        /// </summary>
        /// <returns></returns>
        protected virtual void FormatContentBytes(byte[] bodyBytes)
        {

        }


        /// <summary>
        ///   将字符串转化成Ascii码，并添加到目标bytes的指定位置中
        /// </summary>
        /// <param name="sourceText"></param>
        /// <param name="targetBytes"></param>
        /// <param name="startIndex"></param>
        protected void AddAsciiByteToBytes(string sourceText, byte[] targetBytes, int startIndex)
        {
            if (string.IsNullOrEmpty(sourceText))
                return;

            var buffer = Encoding.ASCII.GetBytes(sourceText);
            Buffer.BlockCopy(buffer, 0, targetBytes, startIndex, buffer.Length);
        }


        /// <summary>
        ///   将字符串转化成Ascii码，并添加到目标bytes的指定位置中
        /// </summary>
        /// <param name="u"></param>
        /// <param name="targetBytes"></param>
        /// <param name="startIndex"></param>
        protected void AddUInt4ByteToBytes(uint u, byte[] targetBytes, int startIndex)
        {
            if (u == 0)
                return;

            var buffer = BitConverter.GetBytes(u);

            Array.Reverse(buffer);
            Buffer.BlockCopy(buffer, 0, targetBytes, startIndex, buffer.Length);
        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="targetBytes"></param>
        protected uint Byte4ToUInt(byte[] targetBytes)
        {
            Array.Reverse(targetBytes);
            return BitConverter.ToUInt32(targetBytes, 0);
        }

        /// <summary>
        ///  
        /// </summary>
        /// <param name="targetBytes"></param>
        protected ulong Byte8ToULong(byte[] targetBytes)
        {
            Array.Reverse(targetBytes);
            return BitConverter.ToUInt64(targetBytes, 0);
        }


        /// <summary>
        ///  获取结果，在Resp 类中重写，处理显示结果
        /// </summary>
        /// <returns></returns>
        public virtual ResultModel GetResult()
        {
            return new ResultModel();
        }
    }



    public class UnKnowResp:BaseMsg
    {
        public UnKnowResp(byte[] bodyBytes, MsgHeaderEnt header)
            : base(bodyBytes, header)
        {

        }

        protected override void FormatContentBytes(byte[] bodyBytes)
        {
            
        }
    }

    /// <summary>
    ///   消息头部
    /// </summary>
    public sealed class MsgHeaderEnt //消息头
    {

        #region  头部属性定义

        /// <summary>
        /// 头部长度
        /// </summary>
        public const int HeaderLength = 4 + 4 + 4;


        /// <summary>
        ///   消息指令
        ///   4 Unsigned Integer 命令或响应类型
        /// </summary>
        public CommandType CommandType { get; private set; }

        /// <summary>
        ///  当前消息序列ID
        ///  4 Unsigned Integer 消息流水号,顺序累加,步长为1,循环使用(一对请求和应答消息的流水号必须相同)
        /// </summary>
        public uint SequenceId { get;internal set; }

        /// <summary>
        ///   此次消息总长度  
        ///   4 Unsigned Integer 消息总长度(含消息头及消息体)
        /// </summary>
        public uint TotalLength { get;  set; }

        #endregion

        #region   头部消息处理


        /// <summary>
        ///    初始化头部属性信息
        /// </summary>
        /// <param name="commandType"></param>
        public MsgHeaderEnt(CommandType commandType)
        {
            //TotalLength = (uint)(bodyLength + HeaderLength);
            CommandType = commandType;
        }


        /// <summary>
        ///    初始化头部属性信息
        /// </summary>
        public MsgHeaderEnt(byte[] bytes)
        {
            byte[] buffer = new byte[4];
            Buffer.BlockCopy(bytes, 0, buffer, 0, buffer.Length);
            Array.Reverse(buffer);
            TotalLength = BitConverter.ToUInt32(buffer, 0);

            Buffer.BlockCopy(bytes, 4, buffer, 0, buffer.Length);
            Array.Reverse(buffer);
            CommandType = (CommandType) BitConverter.ToUInt32(buffer, 0);

            Buffer.BlockCopy(bytes, 8, buffer, 0, buffer.Length);
            Array.Reverse(buffer);
            SequenceId = BitConverter.ToUInt32(buffer, 0);
        }


        /// <summary>
        ///   将头部的属性信息转化为字符串
        /// </summary>
        /// <returns></returns>
        public byte[] ToHeaderBytes()
        {
            byte[] bytes = new byte[HeaderLength];

            byte[] buffer = BitConverter.GetBytes(TotalLength);
            Array.Reverse(buffer);
            Buffer.BlockCopy(buffer, 0, bytes, 0, 4);

            buffer = BitConverter.GetBytes((uint) CommandType);
            Array.Reverse(buffer);
            Buffer.BlockCopy(buffer, 0, bytes, 4, 4);

            buffer = BitConverter.GetBytes(SequenceId);
            Array.Reverse(buffer);
            Buffer.BlockCopy(buffer, 0, bytes, 8, 4);

            return bytes;
        }

        #endregion

    }

}
