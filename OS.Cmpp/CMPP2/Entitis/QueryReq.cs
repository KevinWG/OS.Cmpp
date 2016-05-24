namespace OS.Cmpp.CMPP2.Entitis
{
    public class QueryReq //: CMPP_Request
    {
        //private MsgHeaderEnt _Header;
        //private string _Time;                                    // 8 Octet String 时间YYYYMMDD(精确至日)。
        //private uint _Query_Type;                                // 1 Unsigned Integer 查询类别:
        ////   0:总数查询;
        ////   1:按业务类型查询。
        //private string _Query_Code;                                // 10 Octet String 查询码。
        ////   当Query_Type为0时,此项无效;当Query_Type为1时,此项填写业务类型Service_Id.。
        //private string _Reserve;                                // 8 Octet String 保留。
        //public MsgHeaderEnt Header
        //{
        //    get
        //    {
        //        return _Header;
        //    }
        //}
        //public string Time
        //{
        //    get
        //    {
        //        return _Time;
        //    }
        //}
        //public uint Query_Type
        //{
        //    get
        //    {
        //        return _Query_Type;
        //    }
        //}
        //public string Query_Code
        //{
        //    get
        //    {
        //        return _Query_Code;
        //    }
        //}
        //public string Reserve
        //{
        //    get
        //    {
        //        return _Reserve;
        //    }
        //}
        //public const int BodyLength = 8 + 1 + 10 + 8;
        //public QueryReq
        //    (
        //            DateTime Time
        //            , uint Query_Type
        //            , string Query_Code
        //            , string Reserve
        //            , uint Sequence_Id
        //    )
        //{
        //    _Time = DateTimeHelper.Get_yyyyMMdd_String(Time);
        //    _Query_Type = Query_Type;
        //    _Query_Code = Query_Code;
        //    _Reserve = Reserve;
        //    _Header = new MsgHeaderEnt
        //                        (
        //                            (uint)(MsgHeaderEnt.Length + BodyLength)
        //                            , CommandType.CMPP_QUERY
        //                            , Sequence_Id
        //                        );
        //}
        //public byte[] ToBytes()
        //{
        //    int i = 0;
        //    byte[] bytes = new byte[MsgHeaderEnt.Length + BodyLength];
        //    //header
        //    byte[] buffer = new byte[MsgHeaderEnt.Length];
        //    buffer = _Header.ToHeaderBytes();
        //    buffer.CopyTo(bytes, 0);
        //    //Time 8
        //    i += MsgHeaderEnt.Length;
        //    buffer = new byte[10];
        //    buffer = Encoding.ASCII.GetBytes(_Time);
        //    buffer.CopyTo(bytes, i);
        //    //Query_Type 1
        //    i += 8;
        //    bytes[i++] = (byte)_Query_Type;
        //    //Query_Code 10
        //    buffer = new byte[10];
        //    buffer = Encoding.ASCII.GetBytes(_Query_Code);
        //    buffer.CopyTo(bytes, i);
        //    //Reserve 8
        //    i += 10;
        //    buffer = new byte[8];
        //    buffer = Encoding.ASCII.GetBytes(_Reserve);
        //    buffer.CopyTo(bytes, i);
        //    return bytes;
        //}
     
    }

    //public class QueryResp
    //{
    //    public MsgHeaderEnt Header
    //    {
    //        get
    //        {
    //            return _Header;
    //        }
    //    }
    //    public string Time
    //    {
    //        get
    //        {
    //            return _Time;
    //        }
    //    }
    //    public uint Query_Type
    //    {
    //        get
    //        {
    //            return _Query_Type;
    //        }
    //    }
    //    public string Query_Code
    //    {
    //        get
    //        {
    //            return _Query_Code;
    //        }
    //    }
    //    public uint Mt_TlMsg
    //    {
    //        get
    //        {
    //            return _MT_TLMsg;
    //        }
    //    }
    //    public uint Mt_Tlusr
    //    {
    //        get
    //        {
    //            return _MT_Tlusr;
    //        }
    //    }
    //    public uint Mt_Scs
    //    {
    //        get
    //        {
    //            return _MT_Scs;
    //        }
    //    }
    //    public uint MT_WT
    //    {
    //        get
    //        {
    //            return _MT_WT;
    //        }
    //    }
    //    public uint MT_FL
    //    {
    //        get
    //        {
    //            return _MT_FL;
    //        }
    //    }
    //    public uint MO_Scs
    //    {
    //        get
    //        {
    //            return _MO_Scs;
    //        }
    //    }
    //    public uint MO_WT
    //    {
    //        get
    //        {
    //            return _MO_WT;
    //        }
    //    }
    //    public uint MO_FL
    //    {
    //        get
    //        {
    //            return _MO_FL;
    //        }
    //    }
    //    private MsgHeaderEnt _Header;
    //    private string _Time;                                    // 8 Octet String 时间(精确至日)。
    //    private uint _Query_Type;                                // 1 Unsigned Integer 查询类别:
    //    //   0:总数查询;
    //    //   1:按业务类型查询。
    //    private string _Query_Code;                                // 10 Octet String 查询码。
    //    private uint _MT_TLMsg;                                    // 4 Unsigned Integer 从SP接收信息总数。
    //    private uint _MT_Tlusr;                                    // 4 Unsigned Integer 从SP接收用户总数。
    //    private uint _MT_Scs;                                    // 4 Unsigned Integer 成功转发数量。
    //    private uint _MT_WT;                                    // 4 Unsigned Integer 待转发数量。
    //    private uint _MT_FL;                                    // 4 Unsigned Integer 转发失败数量。
    //    private uint _MO_Scs;                                    // 4 Unsigned Integer 向SP成功送达数量。
    //    private uint _MO_WT;                                    // 4 Unsigned Integer 向SP待送达数量。
    //    private uint _MO_FL;                                    // 4 Unsigned Integer 向SP送达失败数量。
    //    public const int BodyLength = 8                            // Octet String 时间(精确至日)。
    //                                    + 1                        // Unsigned Integer 查询类别:
    //        //  0:总数查询;
    //        //  1:按业务类型查询。
    //                                    + 10                    // Octet String 查询码。
    //                                    + 4                        // Unsigned Integer 从SP接收信息总数。
    //                                    + 4                        // Unsigned Integer 从SP接收用户总数。
    //                                    + 4                        // Unsigned Integer 成功转发数量。
    //                                    + 4                        // Unsigned Integer 待转发数量。
    //                                    + 4                        // Unsigned Integer 转发失败数量。
    //                                    + 4                        // Unsigned Integer 向SP成功送达数量。
    //                                    + 4                        // Unsigned Integer 向SP待送达数量。
    //                                    + 4;                    // Unsigned Integer 向SP送达失败数量。
    //    public QueryResp(byte[] bytes)
    //    {
    //        int i = 0;
    //        //header 12
    //        byte[] buffer = new byte[MsgHeaderEnt.Length];
    //        Buffer.BlockCopy(bytes, 0, buffer, 0, buffer.Length);
    //        _Header = new MsgHeaderEnt(buffer);
    //        //Time 8
    //        i += MsgHeaderEnt.Length;
    //        buffer = new byte[8];
    //        Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
    //        _Time = Encoding.ASCII.GetString(buffer).Trim('\0');
    //        //Query_Type 1
    //        i += 8;
    //        _Query_Type = (uint)bytes[i++];
    //        //Query_Code 10
    //        buffer = new byte[10];
    //        Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
    //        _Query_Code = Encoding.ASCII.GetString(buffer).Trim('\0');
    //        //MT_TLMsg 4
    //        i += 10;
    //        buffer = new byte[4];
    //        Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
    //        Array.Reverse(buffer);
    //        _MT_TLMsg = BitConverter.ToUInt32(buffer, 0);
    //        //MT_Tlusr 4
    //        i += 4;
    //        buffer = new byte[4];
    //        Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
    //        Array.Reverse(buffer);
    //        _MT_Tlusr = BitConverter.ToUInt32(buffer, 0);
    //        //MT_Scs 4
    //        i += 4;
    //        buffer = new byte[4];
    //        Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
    //        Array.Reverse(buffer);
    //        _MT_Scs = BitConverter.ToUInt32(buffer, 0);
    //        //MT_WT 4
    //        i += 4;
    //        buffer = new byte[4];
    //        Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
    //        Array.Reverse(buffer);
    //        _MT_WT = BitConverter.ToUInt32(buffer, 0);
    //        //MT_FL 4
    //        i += 4;
    //        buffer = new byte[4];
    //        Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
    //        Array.Reverse(buffer);
    //        _MT_FL = BitConverter.ToUInt32(buffer, 0);
    //        //MO_Scs 4
    //        i += 4;
    //        buffer = new byte[4];
    //        Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
    //        Array.Reverse(buffer);
    //        _MO_Scs = BitConverter.ToUInt32(buffer, 0);
    //        //MO_WT 4
    //        i += 4;
    //        buffer = new byte[4];
    //        Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
    //        Array.Reverse(buffer);
    //        _MO_WT = BitConverter.ToUInt32(buffer, 0);
    //        //MO_FL 4
    //        i += 4;
    //        buffer = new byte[4];
    //        Buffer.BlockCopy(bytes, i, buffer, 0, buffer.Length);
    //        Array.Reverse(buffer);
    //        _MO_FL = BitConverter.ToUInt32(buffer, 0);
    //    }
    //    public override string ToString()
    //    {
    //        return "[\r\n"
    //                + _Header.ToString() + "\r\n"
    //                + "\t"
    //                + string.Format
    //                            (
    //                                "MessageBody:"
    //                                + "{0}BodyLength: [{1}]"
    //                                + "{0}MO_FL: {2}"
    //                                + "{0}MO_Scs: [{3}]"
    //                                + "{0}MO_WT: [{4}]"
    //                                + "{0}MT_FL: [{5}]"
    //                                + "{0}MT_Scs: [{6}]"
    //                                + "{0}MT_TLMsg: [{7}]"
    //                                + "{0}MT_Tlusr: [{8}]"
    //                                + "{0}MT_WT: [{9}]"
    //                                + "{0}Query_Code: [{10}]"
    //                                + "{0}Query_Type: [{11}]"
    //                                + "{0}Time: [{12}]"
    //                                , "\r\n\t\t"
    //                                , QueryResp.BodyLength
    //                                , _MO_FL
    //                                , _MO_Scs
    //                                , _MO_WT
    //                                , _MT_FL
    //                                , _MT_Scs
    //                                , _MT_TLMsg
    //                                , _MT_Tlusr
    //                                , _MT_WT
    //                                , _Query_Code
    //                                , _Query_Type
    //                                , _Time
    //                            )
    //                + "\r\n]";
    //    }
    //}
}
