using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Server
{
    // Creem la classe Player on li possem com a paràmetres un Id, un NetworkStream i una Posició associada
    class Player
    {
        public int idJugador { get; set; }
        public NetworkStream networkStream { get; set; }

        public Player(int idJugador, NetworkStream networkStream)
        {
            this.idJugador = idJugador;
            this.networkStream = networkStream;
        }
    }

    class Program
    {
        private static IPAddress serverIp;
        private static int currentNumOfPlayers = 0;
        private static readonly object locker = new object();
        private static List<Player> players = new List<Player>();

        static void Main(string[] args)
        {
            // Iniciem el servidor amb una Ip i un Port
            int serverPort = 50000;
            serverIp = IPAddress.Parse("127.0.0.1");

            TcpListener server = new TcpListener(serverIp, serverPort);
            Console.WriteLine("Server has been created");

            server.Start();
            Console.WriteLine("Server started");

            // Creem un bucle infinit com a listener per acceptar connexions entrants de clients (Game)
            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Client connected");

                Thread clientThread = new Thread(serverResponse);
                clientThread.Start(client);
            }
        }

        static void serverResponse(Object o)
        {
            NetworkStream serverNs = null;
            TcpClient client = (TcpClient)o;

            try
            {
                // Primerament generem un objecte jugador associat al client que es connecti i li enviem les dades generades, en aquest cas, la seva Id associada
                serverNs = client.GetStream();
                string generatedIdString = generatePlayer(serverNs).ToString();

                byte[] idBytes = Encoding.UTF8.GetBytes(generatedIdString);

                serverNs.Write(idBytes, 0, idBytes.Length);
            }
            catch (Exception e)
            {
                serverNs.Close();
                client.Close();

                Console.WriteLine("Hi ha hagut un error amb la connexió");
            }

            if (serverNs != null)
            {
                while (true)
                {
                    byte[] localBuffer = new byte[256];
                    int receivedBytes = serverNs.Read(localBuffer, 0, localBuffer.Length);

                    String receivedFrasePl = "";
                    receivedFrasePl = Encoding.UTF8.GetString(localBuffer, 0, receivedBytes);

                    Console.WriteLine(receivedFrasePl);

                    for (int i = 0; i < players.Count; i++)
                    {
                        Player player = players[i];

                        if (player.networkStream != serverNs)
                        {
                            player.networkStream.Write(localBuffer, 0, receivedBytes);
                        }
                    }
                }
            }
        }

        static int generatePlayer(NetworkStream clientNs)
        {
            Player player;

            lock (locker)
            {
                // La id del jugador dependrà de la quantitat de jugadors que hi han 
                // actualment a la llista de Jugadors, això ho controlem amb una variable que s'encarrega de incrementar un valor quan un Jugador es afegit a la llista
                player = new Player(currentNumOfPlayers, clientNs);
                players.Add(player);
                currentNumOfPlayers++;
            }

            return player.idJugador;
        }
    }
}
