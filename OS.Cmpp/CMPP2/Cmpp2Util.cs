using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks.Dataflow;
using OS.Cmpp.CMPP2.Connect;
using OS.Cmpp.CMPP2.Entitis;
using OS.Common.ComModels;
using OS.Common.ComModels.Enums;
using OS.Common.Modules.LogModule;

namespace OS.Cmpp.CMPP2
{
    /// <summary>
    ///    todo   如果连接不上通知等特殊处理，以及速度不同时，连接自动关闭等
    ///    清理没能打开连接
    /// </summary>
    public static class Cmpp2Util
    {
        private static bool _canUse = true;



        #region   渠道注册

        private static Dictionary<string, List<CmppConnection>> _smsChannels =
            new Dictionary<string, List<CmppConnection>>();

        /// <summary>
        /// 注册当前通道的Cmpp通道
        /// </summary>
        /// <param name="smsChannel"></param>
        /// <param name="config"></param>
        public static void Register(string smsChannel, Cmpp2ChannelConfig config)
        {
            List<CmppConnection> conList;
            //  todo   设置default sequeueId
            if (!_smsChannels.ContainsKey(smsChannel))
            {
                conList=new List<CmppConnection>(config.MaxConnectionCount);
                for (int i = 0; i < config.MaxConnectionCount; i++)
                {
                    var con = new CmppConnection(config.IpAddress, config.Port,
                        config.SpCode, config.PassWord, config.SpId, new CmppConnectionOption()
                        {
                            MaxSecondSpeed = config.MaxConnetionSpeed,
                            SlidCount = config.SlidCount,
                            CallBack = config.CallBack
                        });
                    conList.Add(con);
                }
                _smsChannels.Add(smsChannel, conList);
            }
            else
            {
                conList = _smsChannels[smsChannel];
            }
            for (int i = 0; i < conList.Count; i++)
            {
                conList[i].Run();
                LogUtil.Info(string.Format("启动渠道：{0}  第 {1} 个连接", smsChannel, i), "start_connection");
            }
        }

        /// <summary>
        /// 停止CMPP服务
        /// </summary>
        public static void Stop()
        {
            foreach (var channel in _smsChannels)
            {
                var list = channel.Value;
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].Stop();
                    LogUtil.Info(string.Format("关闭渠道：{0}  第 {1} 个连接", channel.Key, i), "close_connection");
                }
                _smsChannels.Remove(channel.Key);
            }
        }


        //  异步给指定连接分配短信 
        private static ActionBlock<Tuple<string, SubmitReq>> _cmppCommands = new ActionBlock
            <Tuple<string, SubmitReq>>(
            tu =>
            {
                var list = _smsChannels[tu.Item1];

                var connect = list.OrderBy(con => con.AllWaitingCount).FirstOrDefault();

                if (connect != null)
                {
                    var subReq = tu.Item2;

                    subReq.MsgSrc = connect.Option.SpId;
                    subReq.SrcId = string.Concat(connect.Option.SpId, subReq.SrcId);

                    connect.Send(subReq);
                }
            });

        /// <summary>
        /// 发送短信
        ///    立马返回结果，后台线程处理具体消息发送
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="msg"></param>
        public static ResultModel SendMsg(string channel, Cmpp2MsgEnt msg)
        {
            if (_smsChannels.Keys.Contains(channel))
            {
                //  todo  处理长短信
                var subReq = GetSubmitReq(msg);
                bool isOk = _cmppCommands.Post(Tuple.Create(channel, subReq));
                return isOk ? new ResultModel() : new ResultModel(ResultTypes.AddFail, "添加消息缓冲池失败！");
            }
            return new ResultModel(ResultTypes.ObjectNull, "未找到对应消息通道");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        private static SubmitReq GetSubmitReq(Cmpp2MsgEnt msg)
        {
            //这里的字段根据需要设定
            SubmitReq submit = new SubmitReq();

            submit.PkTotal = 1; //    uint _Pk_total; 
            submit.PkNumber = 1; //    uint _Pk_number;     // 1 Unsigned Integer 相同Msg_Id的信息序号,从1开始。
            submit.RegisteredDelivery = 1; //   返回错误报告
            //    //   0:不需要;
            //    //   1:需要。
            submit.MsgLevel = 1;
            submit.ServiceId = "1"; //    string _Service_Id;     // 10 Octet String 业务标识,是数字、字母和符号的组合。

            submit.FeeUserType = 1; //    uint _Fee_UserType;     // 1 Unsigned Integer 计费用户类型字段:
            //    //   0:对目的终端MSISDN计费;
            //    //   1:对源终端MSISDN计费;
            //    //   2:对SP计费;
            //    //   3:表示本字段无效,对谁计费参见Fee_terminal_Id字段。

            submit.FeeTerminalId = string.Empty;
            //    string _Fee_terminal_Id;   // 32 Octet String 被计费用户的号码,当Fee_UserType为3时该值有效,当Fee_UserType为0、1、2时该值无意义。
            submit.TpPId = 0; //    uint _TP_pId;    // 1 Unsigned Integer GSM协议类型。详细是解释请参考GSM03.40中的9.2.3.9。
            submit.TpUdhi = 0;
            //    uint _TP_udhi;    // 1 Unsigned Integer GSM协议类型。详细是解释请参考GSM03.40中的9.2.3.23,仅使用1位,右对齐。
            submit.MsgFmt = 8; //    uint _Msg_Fmt;    // 1 Unsigned Integer 信息格式:
            //    //   0:ASCII串;
            //    //   3:短信写卡操作;
            //    //   4:二进制信息;
            //    //   8:UCS2编码;
            //    //   15:含GB汉字......

            submit.FeeType = "01"; //    string _FeeType;     // 2 Octet String 资费类别:
            //    //   01:对"计费用户号码"免费;
            //    //   02:对"计费用户号码"按条计信息费;
            //    //   03:对"计费用户号码"按包月收取信息费。

            submit.FeeCode = "0000"; //    string _FeeCode;     // 6 Octet String 资费代码(以分为单位)。
            //submit.ValIdTime = DateTimeHelper.Get_MMddHHmmss_String(DateTime.Now.AddHours(2))
            //                    + "032+";                                                    //    string _ValId_Time;     // 17 Octet String 存活有效期,格式遵循SMPP3.3协议。
            //submit.At_Time = DateTimeHelper.Get_MMddHHmmss_String(DateTime.Now) + "032+";                        //    string _At_Time;     // 17 Octet String 定时发送时间,格式遵循SMPP3.3协议。

            submit.SrcId = msg.ChanelExt;
            //    string _Src_Id;    // 21 Octet String 源号码。SP的服务代码或前缀为服务代码的长号码, 网关将该号码完整的填到SMPP协议Submit_SM消息相应的source_addr字段,该号码最终在用户手机上显示为短消息的主叫号码。
            submit.DestId = new[] {msg.Mobile};
            //new string[] {"1391xxx1138", "1391xxx1137"}; //    string[] _Dest_terminal_Id;   // 32*DestUsr_tl Octet String 接收短信的MSISDN号码。

            submit.MsgContent = msg.Content;
            //    uint _Dest_terminal_type;   // 1 Unsigned Integer 接收短信的用户的号码类型,0:真实号码;1:伪码。

            return submit;
        }

        #endregion

    }

    public class Cmpp2ChannelConfig
    {
        /// <summary>
        ///   企业码
        /// </summary>
        public string SpCode { get; set; }

        /// <summary>
        ///   企业码
        /// </summary>
        public string PassWord { get; set; }

        /// <summary>
        ///   企业码
        /// </summary>
        public string IpAddress { get; set; }

        /// <summary>
        ///   企业码
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        ///  企业号码（显示手机号主号）
        /// </summary>
        public string SpId { get; set; }

        /// <summary>
        /// 最大连接数
        /// </summary>
        public int MaxConnectionCount { get; set; }

        /// <summary>
        ///   单个连接最大速度  个/秒
        /// </summary>
        public int MaxConnetionSpeed { get; set; }

        /// <summary>
        ///     同时发送窗口数量
        ///     ==  已发送出去但是在等待结果的命令数量
        /// </summary>
        public int SlidCount { get; set; }

        /// <summary>
        /// 回调处理方法
        /// </summary>
        public Action<CmppCommand> CallBack { get; set; }
    }

}
