namespace OS.Cmpp.CMPP2.Entitis
{
    public enum CommandType : uint
    {
        /// <summary>
        /// 请求连接
        /// </summary>
        Connect = 0x00000001,

        /// <summary>
        /// 请求连接应答
        /// </summary>
        ConnectResp = 0x80000001,

        /// <summary>
        /// 终止连接
        /// </summary>
        Terminate = 0x00000002,

        /// <summary>
        /// 终止连接应答
        /// </summary>
        TerminateResp = 0x80000002,

        /// <summary>
        /// 提交短信
        /// </summary>
        Submit = 0x00000004,

        /// <summary>
        /// 提交短信应答
        /// </summary>
        SubmitResp = 0x80000004,

        /// <summary>
        /// 短信下发
        /// </summary>
        Deliver = 0x00000005,

        /// <summary>
        /// 下发短信应答
        /// </summary>
        DeliverResp = 0x80000005,

        /// <summary>
        /// 发送短信状态查询
        /// </summary>
        Query = 0x00000006,

        /// <summary>
        /// 发送短信状态查询应答
        /// </summary>
        QueryResp = 0x80000006,

        /// <summary>
        /// 删除短信
        /// </summary>
        Cancel = 0x00000007,

        /// <summary>
        /// 删除短信应答
        /// </summary>
        CancelResp = 0x80000007,

        /// <summary>
        /// 激活测试
        /// </summary>
        ActiveTest = 0x00000008,

        /// <summary>
        /// 激活测试应答
        /// </summary>
        ActiveTestResp = 0x80000008,

        ///// <summary>
        ///// 消息前转
        ///// </summary>
        //Fwd = 0x00000009,

        ///// <summary>
        ///// 消息前转应答
        ///// </summary>
        //FwdResp = 0x80000009,

        ///// <summary>
        ///// MT路由请求
        ///// </summary>
        //MtRoute = 0x00000010,

        ///// <summary>
        ///// MT路由请求应答
        ///// </summary>
        //MtRouteResp = 0x80000010,

        ///// <summary>
        ///// MO路由请求
        ///// </summary>
        //MoRoute = 0x00000011,

        ///// <summary>
        ///// MO路由请求应答
        ///// </summary>
        //MoRouteResp = 0x80000011,

        ///// <summary>
        ///// 获取MT路由请求
        ///// </summary>
        //GetMtRoute = 0x00000012,

        ///// <summary>
        ///// 获取MT路由请求应答
        ///// </summary>
        //GetMtRouteResp = 0x80000012,

        ///// <summary>
        ///// MT路由更新
        ///// </summary>
        //MtRouteUpdate = 0x00000013,

        ///// <summary>
        ///// MT路由更新应答
        ///// </summary>
        //MtRouteUpdateResp = 0x80000013,

        ///// <summary>
        ///// MO路由更新
        ///// </summary>
        //MoRouteUpdate = 0x00000014,

        ///// <summary>
        ///// MO路由更新应答
        ///// </summary>
        //MoRouteUpdateResp = 0x80000014,

        ///// <summary>
        ///// MT路由更新
        ///// </summary>
        //PushMtRouteUpdate = 0x00000015,

        ///// <summary>
        ///// MT路由更新应答
        ///// </summary>
        //PushMtRouteUpdateResp = 0x80000015,

        ///// <summary>
        ///// MO路由更新
        ///// </summary>
        //PushMoRouteUpdate = 0x00000016,

        ///// <summary>
        ///// MO路由更新应答
        ///// </summary>
        //PushMoRouteUpdateResp = 0x80000016,

        ///// <summary>
        ///// 获取MO路由请求
        ///// </summary>
        //GetMoRoute = 0x00000017,

        ///// <summary>
        ///// 获取MO路由请求应答
        ///// </summary>
        //GetMoRouteResp = 0x80000017
    }
}
