cmpp短信网关协议
===========================
  暂时实现了cmpp2.0  网关通讯协议，通过TPL DataFlow实现的短信消息异步接收，多连接处理。
 

```csharp

Cmpp2Util.Register("BaiWuInternational", new Cmpp2ChannelConfig()
{
    IpAddress = "210.121.134.79",
    Port = 8855,

    MaxConnectionCount = 1,
    MaxConnetionSpeed = 16,

    SpCode = "we76512",
    PassWord = "*******",

    SpId = "7366748495857"

    //CallBack = com =>
    //{

    //}
});

```
