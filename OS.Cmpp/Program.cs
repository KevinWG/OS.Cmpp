using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OS.Cmpp.CMPP2;

namespace OS.Cmpp
{
    class Program
    {
        private static void Main(string[] args)
        {


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

            Console.ReadLine();

        }
    }
}
