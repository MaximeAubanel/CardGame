 using NetworkCommsDotNet.Tools;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server
{
    enum GameState
    {
        Lobby,
        Game,
        Done
    }
    class Game
    {
        // ATTRIBUTE
        private List<User> _Users = new List<User>();
        public GameState _GameState { get; set; }
        public Deck _Deck { get; set; }




        // CONSTRUCTOR/DESTRUCTOR
        public Game(List<User> Users)
        {
            _Users = Users;
            _GameState = GameState.Lobby;
            _Deck = new Deck(System.IO.Path.Combine(Environment.CurrentDirectory) + "\\Cards.txt"); // path of the file witch contain all the cards of the game
        }
        ~Game() { }




        // LOBBY
        public int Lobby()
        {
            while (_GameState == GameState.Lobby)
            {
                updateLobbyStatus();
            }
            Console.WriteLine("Lobby Ready");
            return 0;
        }
        public int updateLobbyStatus()
        {
            if (_Users.Count > 1)
            {
                foreach (User currentUser in _Users.Reverse<User>())
                {
                    if (currentUser._ReadyToPlay == false)
                        return 0;
                }
                _GameState = GameState.Game;
            }
            return 0;
        }




        // GAME
        public int GameLoop()
        {
            DistribCard(_Deck);
            while (WaitUserReceiveCard() == -1) ;
            while (_GameState == GameState.Game)
            {
                Tick();
            }
            sendUserWinner();
            return 0;
        }
        public void Tick()
        {
            int x;
            askUserToPlay();
            while ((x = WaitUserPlay()) == -1) ;
            if (x == -2)
                return ;
            ProceedTurn();
            restartTurn();
        }
        public int ProceedTurn()
        {
            int i = 0;
            string winners = null;
            foreach (User currentUser in _Users)
            {
                if (currentUser._Card.Val > i)
                    i = currentUser._Card.Val;
            }
            foreach (User currentUser in _Users)
            {
                if (currentUser._Card.Val == i)
                {
                    currentUser._Pts += 1;
                    winners = winners + currentUser._Username + "|";
                }
            }
            foreach (User currentUser in _Users)
            {
                SendPacket("GameState", currentUser, winners);
            }
            return 0;
        }
        public int WaitUserPlay()
        {
            if (_GameState == GameState.Done)
                return -2;
            foreach (User currentUser in _Users)
            {
                if (currentUser._Card == null)
                    return -1;
            }
            return 0;
        }
        public int WaitUserReceiveCard()
        {
            foreach (User currentUser in _Users)
            {
                if (currentUser._receivedCard == false)
                    return -1;
            }
            return 0;
        }




        // TOOLS FUNCTION
        public void DistribCard(Deck deck)
        {
            List<List<Card.Card>> NewDecks = new List<List<Card.Card>>();

            foreach (User user in _Users)
            {
                NewDecks.Add(new List<Card.Card>());
            }
            while (deck.Packet.Any())
            {
                foreach (List<Card.Card> tmp in NewDecks)
                {
                    Card.Card temp = deck.PickACard();
                    if (temp != null)
                        tmp.Add(temp);
                }
            }
            int index = 0;
            foreach (User currentUser in _Users)
            {
                foreach (Card.Card card in NewDecks[index])
                {
                    Console.WriteLine(card.Color + " " + card.Name);
                }
                SendPacket("Card", currentUser, JsonConvert.SerializeObject(NewDecks[index]));
                index += 1;
            }
        }   
        public void askUserToPlay()
        {
            foreach (User currentUser in _Users)
            {
                SendPacket("GameLoop", currentUser, "null");
            }
        }
        public void sendUserWinner()
        {
            int x = 0;
            string tmp = null;

            foreach (User currentUser in _Users)
            {
                if (currentUser._Pts > x)
                   x = currentUser._Pts;
            }

            foreach (User currentUser in _Users)
            {
                if (currentUser._Pts == x)
                    tmp = tmp + currentUser._Username + " | ";
            }
            foreach (User currentUser in _Users)
            {
                SendPacket("GameLoop", currentUser, tmp + " WON");
            }

        }




        // NETWORK FUNCTION
        public int SendPacket<T>(string header, User user, T packet)
        {
            try
            {
               user._Connection.SendObject<T>(header, packet);
            }
            catch
            {
                removeUser(user);
                return -1;
            }
            return 0;
        }




        // USERS FUNCTION
        public void removeUser(User user)
        {
            user._Connection.CloseConnection(true);
            _Users.Remove(user);
            Console.WriteLine("User " + user._Username + " offline");
        }
        public void restartTurn()
        {
            foreach (User currentUser in _Users)
            {
                currentUser._Card = null;
            }
        }
    }
}