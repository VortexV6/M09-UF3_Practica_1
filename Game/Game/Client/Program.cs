using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class Program
    {
        private static IPAddress serverIp;
        private static int clientPort;
        private static int playerId;

        static void Main(string[] args)
        {

            clientPort = 60000;
            serverIp = IPAddress.Parse("127.0.0.1");

            TcpClient client = new TcpClient();
            client.Connect(serverIp, clientPort);

            if (client.Connected){

            }
        }
    }
}

