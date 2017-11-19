using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    class myNetwork_IP
    {
        private string                          _Ip;
        private int                             _Port;
        private string                          _FullIp;

        public                                  myNetwork_IP(string ip, int port)
        {
            _Ip = ip;
            _Port = port;
            _FullIp = ip + ":" + port;
        }

        public string                           GetFullIp() => (_FullIp);

        public string                           GetIp() => (_Ip);

        public int                              GetPort() => (_Port);
    }
}
