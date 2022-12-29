// See https://aka.ms/new-console-template for more information

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Program
{
    private const int PORT = 8000;
    private const String HOST = "127.0.0.1";
    static void Main(string[] args)
    {

        IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(HOST), PORT);
        Socket server       = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        
        server.Bind(endpoint); // socket is bound to the endpoint local host 8000 on IP 127.0.0.1
        server.Listen(10);
        

        while (true) // ensures server does not stop
        {
            Console.WriteLine("Server Running...");
            Socket client = server.Accept();
            Console.WriteLine("accepted client");
            ServerThread thread = new ServerThread(client);
        }
        server.Close();
    }
}
