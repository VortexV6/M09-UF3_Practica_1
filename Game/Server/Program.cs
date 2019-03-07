using PositionLibrary;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server
{
    // Creamos la clase Player donde le ponemos como parámetros un Id, un NetworkStream y una posición asociada
    class Player
    {
        public int IdJugador { get; set; }
        public NetworkStream NetworkStream { get; set; }
        public Position Position { get; set; }

        public Player(int IdJugador, NetworkStream NetworkStream)
        {
            this.IdJugador = IdJugador;
            this.NetworkStream = NetworkStream;
        }
    }

    class Program
    {
        private static IPAddress ServerIp;
        private static int CurrentNumOfPlayers = 0;
        private static readonly object locker = new object();
        private static List<Player> players = new List<Player>();

        static void Main(string[] args)
        {
            // Iniciamos el servidor con una Ip y un puerto
            int serverPort = 50000;
            ServerIp = IPAddress.Parse("127.0.0.1");

            TcpListener server = new TcpListener(ServerIp, serverPort);
            Console.WriteLine("El servidor ha sido creado");

            server.Start();
            Console.WriteLine("Servidor iniciado");

            // Creamos un bucle infinito como listener para aceptar conexiones entrantes de clientes (Game)
            while (true)
            {
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("Cliente conectado");

                // Iniciamos un hilo que se encargará de enviar los datos correspondientes a cada jugador que esté conectado mediante el método ServerResponse
                Thread clientThread = new Thread(ServerResponse);
                clientThread.Start(client);
            }
        }

        static void ServerResponse(Object o)
        {
            NetworkStream serverNs = null;
            TcpClient client = (TcpClient)o;

            try
            {
                // Primeramente generamos un objeto jugador asociado al cliente que se conecte y le enviamos los datos generados, en este caso, su Id asociada
                serverNs = client.GetStream();
                string generatedIdString = GeneratePlayer(serverNs).ToString();

                byte[] idBytes = Encoding.UTF8.GetBytes(generatedIdString);

                serverNs.Write(idBytes, 0, idBytes.Length);
            }
            catch (Exception e)
            {
                serverNs.Close();
                client.Close();
                //Manejamos el error si la conexion no ha sido posible
                Console.WriteLine("Ha habido un error con la conexion");
            }

            if (serverNs != null)
            {
                while (true)
                {
                    // Si el NetworkStream del server no es null, creamos un bucle infinito que se encarga de ir escuchando los datos de los jugadores conectados y reenviarlo a todos los demás
                    byte[] localBuffer = new byte[256];
                    int receivedBytes = serverNs.Read(localBuffer, 0, localBuffer.Length);

                    // Los datos que envía un Jugador será la posición de su bola, si lo queremos mostrar por la consola del server, necesitamos Deserialitzar el objeto recibido y acceder a sus atributos
                    Position receivedPosition;
                    receivedPosition = (Position)Position.Deserialize(localBuffer);

                    Console.WriteLine(receivedPosition.posX);
                    Console.WriteLine(receivedPosition.posY);

                    // Para reenviar la posición a los demás jugadores excepto el cliente origen, hacemos un recorrido de la lista de Jugadores y enviamos la posición a todos los jugadores que tengan un
                    // NetworkStream diferente al del origen
                    for (int i = 0; i < players.Count; i++)
                    {
                        Player player = players[i];

                        if (player.NetworkStream != serverNs)
                        {
                            player.NetworkStream.Write(localBuffer, 0, receivedBytes);
                        }
                    }
                }
            }
        }

        static int GeneratePlayer(NetworkStream clientNs)
        {
            Player player;

            lock (locker)
            {
                // La id del jugador dependera de la cantidad de jugadores que haya
                // actualmente en la lista de Jugadores, esto lo controlamos con una variable que se encarga de incrementar un valor cuando un Jugador se ha añadido a la lista
                player = new Player(CurrentNumOfPlayers, clientNs);
                players.Add(player);
                CurrentNumOfPlayers++;
            }

            return player.IdJugador;
        }
    }
}