using System;
using OS.Cmpp.CMPP2.Entitis;

namespace OS.Cmpp.CMPP2.Connect
{
    public class CmppCommand
    {
    
        /// <summary>
        /// command请求
        /// 返回时可能为空
        /// </summary>
        public BaseMsg Request { get; set; }

        /// <summary>
        /// command 返回
        /// </summary>
        public BaseMsg Response { get; set; }

        /// <summary>
        ///  上次发送时间
        /// </summary>
        public DateTime LastSendTime { get; set; }

    }
}
