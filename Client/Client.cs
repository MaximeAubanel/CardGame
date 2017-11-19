using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Client
{
    class Client
    {
        public static int ServerPort { get; set; }
        public static string ServerIP { get; set; }
        public static string Name { get; set; }
        public List<Card.Card> Cards { get; set; }
        bool _End = false;

        public Client()
        {
            NetworkCommsDotNet.Connections.Connection.StartListening(NetworkCommsDotNet.Connections.ConnectionType.TCP, new System.Net.IPEndPoint(System.Net.IPAddress.Any, 0));

            NetworkCommsDotNet.NetworkComms.AppendGlobalIncomingPacketHandler<string>("Card", CardPacketHandler);
            NetworkCommsDotNet.NetworkComms.AppendGlobalIncomingPacketHandler<string>("GameLoop", GameLoopHandler);
            NetworkCommsDotNet.NetworkComms.AppendGlobalIncomingPacketHandler<string>("GameState", GameStatePacketHandler);
        }
        ~Client()
        {
            Stop();
        }


        public int Start()
        {
            NetworkCommsDotNet.Connections.Connection.StartListening(NetworkCommsDotNet.Connections.ConnectionType.TCP, new System.Net.IPEndPoint(System.Net.IPAddress.Any, 0));

            GetInfoServer();
            GetInfoUser();
            Run();
            return (0);
        }

        public int Stop()
        {
            NetworkCommsDotNet.NetworkComms.Shutdown();
            return 0;
        }

        public int Lobby()
        {
            while (true)
            {
                Console.Write("Tape 'ready' when you are ready or 'q' if you want to quit : ");
                string line = Console.ReadLine();
                if (line.Equals("ready"))
                {
                    SendPacket("Lobby", "ready");
                    return 0;
                }
                else if (line.ToLower().Equals("quit"))
                    break;
                else
                    SendPacket("Lobby", "notReady");
            }
            return (-1);
        }

        public int Run()
        {
            ConnectToServer(ServerIP, ServerPort);
            if (Lobby() == -1)
                return (-1);
            while (_End == false) ;
            return (0);
        }

        public int GetInfoServer()
        {

            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex("([0-9]{1,3}\\.){3}[0-9]{1,3}");

            ServerIP = "127.0.0.1";
            ServerPort = 4242;

            /*System.Text.RegularExpressions.Match match = null;

            while (ServerIP == null ||!match.Success)
            {
                Console.Write("Server's IP Adress : ");
                ServerIP = Console.ReadLine();
                match = regex.Match(ServerIP);
            }

            System.Text.RegularExpressions.Regex regexPort = new System.Text.RegularExpressions.Regex("[0-9]{4,5}");

            System.Text.RegularExpressions.Match matchPort = null;

            while (ServerPort == null || !matchPort.Success)
            {
                Console.Write("Server's Port : ");
                ServerPort = Console.ReadLine();
                matchPort = regexPort.Match(ServerPort);
            }*/
            return (0);
        }

        public int GetInfoUser()
        {
            Console.Write("Please enter your player name : ");
            Name = Console.ReadLine();
            while (Name == "" || Name.IndexOf("'") != -1 || Name.IndexOf("\"") != -1 
                    || Name.IndexOf("(") != -1 || Name.IndexOf(")") != -1 
                    || Name.IndexOf(".") != -1 || Name.IndexOf(":") != -1 
                    || Name.IndexOf(",") != -1)
            {
                Console.WriteLine("Please enter your name without special caracter : { ' \" . : , ( ) }...\nEnter your player name :");
                Name = Console.ReadLine();
            }   
            return (0);
        }

        public int ConnectToServer(string ip, int port)
        {
            SendPacket<string>("Connection", Name);

            return (0);
        }

        private void GameLoopHandler(NetworkCommsDotNet.PacketHeader header, NetworkCommsDotNet.Connections.Connection connection, string message)
        {
            if (message == "null")
            {
                AffHand();
                if (Cards.Count > 0)
                {
                    int x = ReadLine();
                    Card.Card a = Cards[x];
                    if (SendPacket<string>("Game", JsonConvert.SerializeObject(a)) == 1)
                        System.Environment.Exit(1);
                    Cards.RemoveAt(x);
                }
                else
                {
                    if (SendPacket<string>("Game", "end") == 1)
                        System.Environment.Exit(1);
                }
            }
            else
            {
                Console.WriteLine(message);
                _End = true;
            }

        }
        private static void GameStatePacketHandler(NetworkCommsDotNet.PacketHeader header, NetworkCommsDotNet.Connections.Connection connection, string message)
        {
            Console.WriteLine(message + " WON the round");
        }

        private void CardPacketHandler(NetworkCommsDotNet.PacketHeader header, NetworkCommsDotNet.Connections.Connection connection, string message)
        {
            Cards = JsonConvert.DeserializeObject<List<Card.Card>>(message);
            if (SendPacket<string>("Start", "") == -1)
                System.Environment.Exit(1);
        }

        static public int SendPacket<T>(string header, T packet)
        {
            try
            {
                NetworkCommsDotNet.NetworkComms.SendObject(header, ServerIP, ServerPort, packet);
            }
            catch
            {
                return -1;
            }
            return 0;
        }

        public void AddCard(Card.Card newCard)
        {
            if (newCard != null)
                Cards.Add(newCard);
        }

        /**
         * Aff Card List
         */
        public void AffHand()
        {
            int x = 0;
            foreach (Card.Card tmp in Cards)
            {
                Console.WriteLine(x + " ------> " + tmp.Name + " of " + tmp.Color);
                x = x + 1;
            }

        }

        public int ReadLine()
        {
            Console.Write("Which card do you want to play ? (number) ");
            string tmp = Console.ReadLine();
            int x;
            while (!int.TryParse(tmp, out x))
            {
                Console.Write("Enter an interger in size list please : ");
                tmp = Console.ReadLine();
            }
            while (x > Cards.Count - 1)
            {
                tmp = "";
                while (!int.TryParse(tmp, out x))
                {
                    Console.Write("Enter an interger in size list please : ");
                    tmp = Console.ReadLine();
                }
            }
            return (x);
        }

    }
}
