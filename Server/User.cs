using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NetworkCommsDotNet.Connections;

namespace Server
{
    class User
    {
        public bool                                 _receivedCard = false;

        private DateTime                            _PingStart;
        public int                                  _Ping { get; set; }
        public string                               _Username { get; set; }
        internal myNetwork_IP                       _Ip { get; set; }
        public Connection                           _Connection { get; set; }
        public bool                                 _ReadyToPlay { get; set; }
        public Card.Card                            _Card { get; set; }
        public int                                  _Pts { get; set; }
        public User(string username, NetworkCommsDotNet.Connections.Connection connection)
        {
            _Card = null;
            _Ping = 0;
            _ReadyToPlay = false;
            _Username = username;
            _Connection = connection;
            _Ip = new myNetwork_IP(connection.ConnectionInfo.RemoteEndPoint.ToString().Split(':').First(), int.Parse(connection.ConnectionInfo.RemoteEndPoint.ToString().Split(':').Last()));
        }

        public DateTime                             getPingStart() => (_PingStart);
        public void                                 setPingStart(DateTime time) { _PingStart = time; }
        public void                                 toggleReadyToPlay() { _ReadyToPlay = !_ReadyToPlay; }
        public int                                  GetPing() => (_Ping);
        public void                                 GetPing(int ping) { _Ping = ping; }
        public string                               GetFullIp() => (_Ip.GetFullIp());
        public string                               GetIp() => (_Ip.GetIp());
        public int                                  GetPort() => (_Ip.GetPort());
    }
}
