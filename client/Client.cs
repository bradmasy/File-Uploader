using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

namespace client
{
    public class Client
    {

        private int port;
        private IPAddress host;
        public IPEndPoint ip_endpoint;
        public Socket client_socket;

        public Client(int port, string host)
        {
            this.host = IPAddress.Parse(host);
            this.port = port;
            this.ip_endpoint = new IPEndPoint(this.host, this.port);
            this.client_socket =  new Socket(ip_endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            client_socket.Connect(ip_endpoint);
            Console.WriteLine("New Client");
        }
    }
}

