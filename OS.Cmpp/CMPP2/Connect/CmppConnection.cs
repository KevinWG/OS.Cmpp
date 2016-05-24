using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading;
using OS.Cmpp.CMPP2.Entitis;
using OS.Common.ComModels;
using OS.Common.ComModels.Enums;
using OS.Common.Modules.AsynModule;
using OS.Common.Modules.LogModule;

namespace OS.Cmpp.CMPP2.Connect
{
    /// <summary>
    ///   CMPP链接类
    /// </summary>
    public class CmppConnection
    {

        #region 私有内部属性

        /// <summary>
        ///   选项
        /// </summary>
        public CmppConnectionOption Option { get;private set; }

        /// <summary>
        /// socket 对象
        /// </summary>
        private Socket _socket;

        /// <summary>
        ///  是否能够正常使用
        /// </summary>
        private bool _canUsed = true;
        #endregion

        #region   序列ID相关

        /// <summary>
        /// 当前序列ID
        /// </summary>
        private static int _sequenceId;

        /// <summary>
        ///   自增长序列ID
        ///      每次调用都会自增长+1
        /// </summary>
        private static uint GetNextSequenceId()
        {
            if (_sequenceId < CmppConnectionOption.DefaultSequenceId)
            {
                _sequenceId = CmppConnectionOption.DefaultSequenceId;
            }
            if (_sequenceId >= int.MaxValue)
            {
                _sequenceId = CmppConnectionOption.DefaultSequenceId = 0;
            }
            Interlocked.Increment(ref _sequenceId);
            return (uint) _sequenceId;
        }

        #endregion

        /// <summary>
        ///    所有当期没有返回的执行命令数量
        /// </summary>
        public int AllWaitingCount
        {
            get { return Option.CommandQueue.Count + Option.WaitingCommand.Count; }
        }


        /// <summary> 
        ///  构造函数
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="port"></param>
        /// <param name="spCode">企业代码</param>
        /// <param name="pwd">密码</param>
        /// <param name="spId">企业号码（手机号显示主号）</param>
        /// <param name="opt"> 链接其他设置信息 </param>
        public CmppConnection(string ipAddress, int port,string spCode,string pwd,string spId,CmppConnectionOption opt=null)
        {
            Option = opt ?? new CmppConnectionOption();

            Option.IpAddress = ipAddress;
            Option.Port = port;
            Option.SpCode = spCode;
            Option.Pwd = pwd;
            Option.SpId = spId;

            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        }

        /// <summary>
        ///  发送命令 
        /// </summary>
        /// <param name="req"></param>
        public bool Send(BaseMsg req)
        {
            if (req.MsgHeader.CommandType == CommandType.Connect)
                return false;

            req.MsgHeader.SequenceId = GetNextSequenceId();

            var command = new CmppCommand() {Request = req};
            Option.CommandQueue.Enqueue(command);

            return true;
        }

        /// <summary>
        ///   运行监测
        /// </summary>
        public void Run()
        {
            _canUsed = true;
            ThreadPool.QueueUserWorkItem(obj =>
            {
                var tupleObj = obj as Tuple<CmppConnectionOption, Socket>;

                if (tupleObj != null)
                {
                    var cmConOpt = tupleObj.Item1;
                    var socket = tupleObj.Item2;

                    //  停止的时候如果数据没有读完，等待读完
                    while (_canUsed|| _socket.Available>0 )
                    {
                        if ((cmConOpt.ConnectedStatu == ConnectionStatu.Default
                            ||cmConOpt.ConnectedStatu == ConnectionStatu.Cloase)
                            && OpenSocketConnect(cmConOpt).Ret != ResultTypes.Success)
                            return;

                        if (cmConOpt.ConnectedStatu == ConnectionStatu.Failed)
                            _canUsed=false;


                        bool isRuning = false;
                        if (CheckCanWrite(cmConOpt))
                        {
                            isRuning = true;
                            WriteSocketReq(cmConOpt, socket);
                        }

                        if (socket.Available > 0)
                        {
                            isRuning = true;
                            ReadSocketResp(socket, cmConOpt);
                        }

                        RemoveExpireCommand(cmConOpt);

                        cmConOpt.LastScanTime = DateTime.Now;

                        if (!isRuning)
                        {
                            Thread.Sleep(500);
                        }
                    }

                    //  通道监听关闭后 关闭socket
                    if (_socket.Connected)
                    {
                        _socket.Close();
                    }
                }
            }, Tuple.Create(Option,_socket));
        }

        /// <summary>
        ///  关闭当前通道
        /// </summary>
        public void Stop()
        {
            Option.ConnectedStatu = ConnectionStatu.Cloase;//  立即关闭  防止继续有数据进入

            //  todo  发送关闭命令，在发送命令返回之后修改 _canUsed 值
            _canUsed = false;
        }

        /// <summary>
        ///  清理过期请求
        /// </summary>
        /// <param name="cmConOpt"></param>
        /// <returns></returns>
        private static void RemoveExpireCommand(CmppConnectionOption cmConOpt)
        {
            if ((DateTime.Now - cmConOpt.LastExcuteTime).TotalSeconds > 20
                && cmConOpt.WaitingCommand.Count > 0)
            {
                List<uint> removeSeqIds = null;
                foreach (var wcom in cmConOpt.WaitingCommand)
                {
                    if ((DateTime.Now - wcom.Value.LastSendTime).TotalSeconds > 30)
                    {
                        //  放在这里避免多次创建无效变量
                        if (removeSeqIds==null)
                        {
                            removeSeqIds=new List<uint>();
                        }
                        removeSeqIds.Add(wcom.Key);
                    }
                }
                if (removeSeqIds != null 
                    && removeSeqIds.Count > 0)
                {
                    removeSeqIds.ForEach(cmSeqId => { cmConOpt.WaitingCommand.Remove(cmSeqId); });
                }
            }
        }

        /// <summary>
        ///   打开socket链接
        /// </summary>
        private ResultModel OpenSocketConnect(CmppConnectionOption opt)
        {
            try
            {
                if (!_socket.Connected)
                    _socket.Connect(opt.IpAddress, opt.Port);

                var connectReq = new ConnectReq(opt.SpCode, opt.Pwd, Option.Version);
                connectReq.MsgHeader.SequenceId = GetNextSequenceId();
                _socket.Send(connectReq.ToBytes());

                opt.ConnectedStatu = ConnectionStatu.Waiting;

                LogUtil.Info("发送打开连接请求", "connect_req");
            }
            catch (Exception ex)
            {
                var keyCode = LogUtil.Error(string.Concat("连接短信服务方异常失败,详情：", ex.Message), "connect_failed");

                opt.ConnectedStatu = ConnectionStatu.Failed;
                return new ResultModel(ResultTypes.InnerError, string.Concat("连接短信服务方失败，错误码：", keyCode));
            }
            return new ResultModel();
        }


        #region  写socket操作

        /// <summary>
        /// 写socket字节流部分
        /// </summary>
        /// <param name="cmConOpt"></param>
        /// <param name="socket"></param>
        private static void WriteSocketReq(CmppConnectionOption cmConOpt, Socket socket)
        {
            try
            {
                // 1.  发送请求
                var command = cmConOpt.CommandQueue.Dequeue();
                socket.Send(command.Request.ToBytes());

                // 2.  记录当前请求时间
                command.LastSendTime = cmConOpt.LastExcuteTime = DateTime.Now;

                // 3.  添加等待响应请求
                if (command.Request.MsgHeader.CommandType == CommandType.ActiveTestResp
                    || command.Request.MsgHeader.CommandType == CommandType.DeliverResp)
                {
                    cmConOpt.WaitingCommand.Add(command.Request.MsgHeader.SequenceId, command);
                }
            }
            catch (Exception ex)
            {
                LogUtil.Info("发送短信运营方信息时出错");
#if DEBUG
                throw ex;
#endif
            }
        }

        #endregion

        #region  读socket字节流结果

        /// <summary>
        ///   读处理
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="cmConOpt"></param>
        private static void ReadSocketResp(Socket socket, CmppConnectionOption cmConOpt)
        {
            try
            {
                var resp = GetSocketResp(socket);

                // 1.  获取上次发送请求command
                CmppCommand command = null;
                if (cmConOpt.WaitingCommand.ContainsKey(resp.MsgHeader.SequenceId))
                {
                    command = cmConOpt.WaitingCommand[resp.MsgHeader.SequenceId];
                    cmConOpt.WaitingCommand.Remove(resp.MsgHeader.SequenceId);
                }
                else
                {
                    command = new CmppCommand();
                }
                command.Response = resp;

                //  2. 激活测试
                if (resp.MsgHeader.CommandType == CommandType.ActiveTest)
                {
                    //  对方发送activetest   给出响应
                    cmConOpt.CommandQueue.Enqueue(new CmppCommand()
                    {
                        Request = new ActiveTestResp(resp.MsgHeader.SequenceId)
                    });
                }
                //  3. 连接结果响应
                else if (resp.MsgHeader.CommandType == CommandType.ConnectResp)
                {
                    var conResp = resp as ConnectResp;
                    cmConOpt.ConnectedStatu = conResp.Status == 0 ? ConnectionStatu.Success : ConnectionStatu.Failed;
                }

                // 4.  对响应设置回调处理
                RespCallBack(cmConOpt, command);
            }
            catch (Exception ex)
            {
                LogUtil.Info("接收运营商响应信息时出错", "sendreq_error");
#if DEBUG
                throw ex;
#endif
            }
           
        }


        /// <summary>
        /// 针对响应数据回调处理
        /// </summary>
        /// <param name="cmConOpt"></param>
        /// <param name="command"></param>
        private static void RespCallBack(CmppConnectionOption cmConOpt, CmppCommand command)
        {
            //LogUtil.Info("接收运营商响应信息", command, CompModuleLogs.SmsExchangeRespLog);
            if (command.Response.MsgHeader.CommandType == CommandType.ConnectResp)
            {
                var resp = command.Response as ConnectResp;
                if (resp != null)
                {
                    var result = resp.GetResult();
                    if (result.Ret==ResultTypes.Success)
                        LogUtil.Info("建立连接成功!", "connect_success");
                    else
                        LogUtil.Info(result.Message, "connect_failed");
                }
            }
            else if (command.Response.MsgHeader.CommandType != CommandType.ActiveTestResp
                && command.Response.MsgHeader.CommandType != CommandType.ActiveTest)
            {
                if (cmConOpt.CallBack != null)
                {
                    AsynUtil.Asyn(obj =>
                    {
                        var com = obj.Item2;
                        var opt = obj.Item1;
                        opt.CallBack(com);
                    }, Tuple.Create(cmConOpt, command));
                }
            }

        }

        /// <summary>
        ///    获取响应实体
        /// </summary>
        /// <param name="socket"></param>
        /// <returns></returns>
        private static BaseMsg GetSocketResp(Socket socket)
        {
            byte[] headerBytes = GetBytesFromSocket(socket, MsgHeaderEnt.HeaderLength);

            MsgHeaderEnt header = new MsgHeaderEnt(headerBytes);

            if (header.TotalLength > 65536 || header.TotalLength == 0)
                throw new ArgumentOutOfRangeException("socket", "当前socket包不正确");

            int bodyLength = (int) header.TotalLength - MsgHeaderEnt.HeaderLength;
            byte[] bodyBytes = GetBytesFromSocket(socket, bodyLength);

            return GetSocketRespByHeader(header, bodyBytes);
        }


        /// <summary>
        /// 根据命令类型返回相关响应类
        /// </summary>
        /// <param name="header"></param>
        /// <param name="bodyBytes"></param>
        /// <returns></returns>
        private static BaseMsg GetSocketRespByHeader(MsgHeaderEnt header, byte[] bodyBytes)
        {
            //  todo 完善信息
            BaseMsg resp = null;
            switch (header.CommandType)
            {
                case CommandType.ConnectResp:
                    resp = new ConnectResp(bodyBytes, header);
                    break;
                //case CommandType.DeliverResp:
                //    resp = new DeliverResp(bodyBytes, header);
                //    break;
                case CommandType.SubmitResp:
                    resp = new SubmitResp(bodyBytes, header);
                    break;
                    case CommandType.ActiveTest:
                    resp = new ActiveTestReq(bodyBytes, header);
                    break;
                case  CommandType.ActiveTestResp:
                    resp = new ActiveTestResp(bodyBytes, header);
                    break;
                default:
                    resp = new UnKnowResp(bodyBytes, header);
                    break;
            }
            return resp;
        }

        /// <summary>
        /// 从socket中获取对应的字节
        /// </summary>
        /// <param name="socket"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        private static byte[] GetBytesFromSocket(Socket socket, int length)
        {
            int p = 0;
            byte[] data = new byte[length];
            while (p < length)
            {
                int r = socket.Receive(data, p, length - p, SocketFlags.None);
                p += r;
            }
            return data;
        }

        #endregion

        /// <summary>
        ///    判断当前是否具备可写
        ///     是否已经大于滑动窗口数量，是否超过时间限制
        /// </summary>
        /// <param name="opt"></param>
        /// <returns></returns>
        private bool CheckCanWrite(CmppConnectionOption opt)
        {
            bool isOkay = false;

            if (opt.ConnectedStatu==ConnectionStatu.Success
                && opt.MaxSecondSpeed > 0)
            {

                isOkay = opt.CommandQueue.Count > 0 && opt.WaitingCommand.Count < opt.SlidCount;

                if (isOkay)
                {
                    int seprateMilliSecond = 1000 / opt.MaxSecondSpeed;

                    isOkay =  (DateTime.Now - opt.LastExcuteTime).Milliseconds > seprateMilliSecond;
                }
            }
            return isOkay;
        }
    }

    /// <summary>
    ///    链接的选项信息
    /// </summary>
    public class CmppConnectionOption
    {
        public CmppConnectionOption()
        {
            CommandQueue = new Queue<CmppCommand>();
            WaitingCommand=new Dictionary<uint, CmppCommand>();
            LastExcuteTime = DateTime.Now;

            SlidCount = 1;
            Version = 1;
            ConnectedStatu = ConnectionStatu.Default;
        }

        /// <summary>
        /// 初始序列ID
        /// </summary>
        public static int DefaultSequenceId { get; set; }

        #region  连接的账号密码等信息，不直接赋值

        /// <summary>
        ///   请求地址
        /// </summary>
        public string IpAddress { get; internal set; }

        /// <summary>
        ///  端口
        /// </summary>
        public int Port { get; internal set; }

        /// <summary>
        ///  企业编号
        /// </summary>
        public string SpCode { get; internal set; }

        /// <summary>
        ///  密码
        /// </summary>
        public string Pwd { get; internal set; }


        /// <summary>
        ///  企业号码（显示手机号主号）
        /// </summary>
        public string SpId { get; internal set; }
        #endregion

        /// <summary>
        /// 版本
        /// </summary>
        public uint Version { get; set; }

        /// <summary>
        ///   连接状态
        ///      -1  打开失败   
        ///       0   初始化   
        ///       1  发送等待中    
        ///       2   打开成功
        /// </summary>
        public ConnectionStatu ConnectedStatu { get; set; }

        /// <summary>
        ///    最大单秒速度
        /// </summary>
        public int MaxSecondSpeed { get; set; }

        private int _slidCount=1;

        /// <summary>
        ///     发送窗口数量
        ///     ==  已发送出去但是在等待结果的命令数量
        /// </summary>
        public int SlidCount
        {
            get { return _slidCount; }
            set
            {
                if (value>0)
                {
                    _slidCount = value; 
                }
            }
        }

        /// <summary>
        /// 上次命令执行时间
        /// </summary>
        public DateTime LastExcuteTime { get; internal set; }

        /// <summary>
        /// 上次线程扫描时间
        /// </summary>
        public DateTime LastScanTime { get;internal set; }

        /// <summary>
        ///   当前等待发送命令
        /// </summary>
        internal Queue<CmppCommand> CommandQueue { get; set; }

        /// <summary>
        ///   等待结果的命令对象
        /// </summary>
        internal IDictionary<uint, CmppCommand> WaitingCommand { get; set; }

        /// <summary>
        /// 回调方法
        /// </summary>
        public Action<CmppCommand> CallBack { get; set; }
    }



    /// <summary>
    ///   链接状态
    /// </summary>
    public enum ConnectionStatu
    {
        /// <summary>
        ///  初始化
        /// </summary>
        Default=0,

        /// <summary>
        /// 等待答复
        /// </summary>
        Waiting=1,

        /// <summary>
        ///   连接成功
        /// </summary>
        Success=2,

        /// <summary>
        /// 连接失败
        /// </summary>
        Failed=-1,

        /// <summary>
        /// 关闭
        /// </summary>
        Cloase = 3,

    }
}
