using NetworkCommsDotNet.Tools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    class Server
    {
        // ATTRIBUTE
        private bool                            _Toggle = false;
        private List<User>                      _Users = new List<User>();
        private Game                            _Game;




        // CONSTRUCTOR/DESTRUCTOR
        public Server()
        {
            NetworkCommsDotNet.NetworkComms.AppendGlobalIncomingPacketHandler<string>("Lobby", LobbyPacketHandler);
            NetworkCommsDotNet.NetworkComms.AppendGlobalIncomingPacketHandler<string>("Game", GamePacketHandler);
            NetworkCommsDotNet.NetworkComms.AppendGlobalIncomingPacketHandler<string>("Connection", ConnectionPacketHandler);
            NetworkCommsDotNet.NetworkComms.AppendGlobalIncomingPacketHandler<string>("Start", StartPacketHandler);

            _Game = new Game(_Users);
        }
        ~Server()
        {
            Stop();
        }




        // START/STOP FUNCTION
        public int Start()
        {
            NetworkCommsDotNet.Connections.Connection.StartListening(NetworkCommsDotNet.Connections.ConnectionType.TCP, new System.Net.IPEndPoint(System.Net.IPAddress.Any, 4242));
            foreach (System.Net.IPEndPoint localEndPoint in NetworkCommsDotNet.Connections.Connection.ExistingLocalListenEndPoints(NetworkCommsDotNet.Connections.ConnectionType.TCP))
                Console.WriteLine("{0}:{1}", localEndPoint.Address, localEndPoint.Port);

            Run();
            return 0;
        }
        public int Stop()
        {
            NetworkCommsDotNet.NetworkComms.Shutdown();

            return 0;
        }




        // START GAME
        public int Run()
        {
            _Game.Lobby();
            _Game.GameLoop();

            return 0;
        }




        // TOOLS FUNCTION
        public int                              AddNewUser(string username, NetworkCommsDotNet.Connections.Connection connection)
        {
            User                                newUser = new User(username, connection);

            _Users.Add(newUser);
            Console.WriteLine("User " + username + " " + "online");
            return 0;
        }
        public User                             getUser(string ip)
        {
            foreach (User currentUser in _Users)
            {
                if (currentUser.GetFullIp() == ip)
                    return currentUser;
            }
            return null;
        }




        // PacketHandler
        private void                            ConnectionPacketHandler(NetworkCommsDotNet.PacketHeader header, NetworkCommsDotNet.Connections.Connection connection, string username)
        {
            if (getUser(connection.ConnectionInfo.RemoteEndPoint.ToString()) != null)
                return;
            AddNewUser(username, connection);
            Console.WriteLine(connection.ConnectionInfo.RemoteEndPoint.ToString());
        }
        private void                            LobbyPacketHandler(NetworkCommsDotNet.PacketHeader header, NetworkCommsDotNet.Connections.Connection connection, string message)
        {
            User user;

            if ((user = getUser(connection.ConnectionInfo.RemoteEndPoint.ToString())) == null) return ;

            if (message == "ready")
                user._ReadyToPlay = true;
            else if (message == "notReady")
                user._ReadyToPlay = false;
        }
        private void                            GamePacketHandler(NetworkCommsDotNet.PacketHeader header, NetworkCommsDotNet.Connections.Connection connection, string message)
        {
            User user;

            Console.WriteLine(message);

            if ((user = getUser(connection.ConnectionInfo.RemoteEndPoint.ToString())) == null) return;

            if (message.Equals("end"))
            {
                _Game._GameState = GameState.Done;
                return;
            }

            Card.Card newCard = new Card.Card();
            newCard.Color = JsonConvert.DeserializeObject<Card.Card>(message).Color;
            newCard.Val = JsonConvert.DeserializeObject<Card.Card>(message).Val;
            newCard.Name = JsonConvert.DeserializeObject<Card.Card>(message).Name;

            user._Card = newCard;
        }
        private void                            StartPacketHandler(NetworkCommsDotNet.PacketHeader header, NetworkCommsDotNet.Connections.Connection connection, string message)
        {
            User user;

            if ((user = getUser(connection.ConnectionInfo.RemoteEndPoint.ToString())) == null) return;

            user._receivedCard = true;
        }
    }
}
