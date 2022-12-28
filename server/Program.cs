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
        
        server.Bind(endpoint);
        server.Listen(1);

        //TcpListener server = new TcpListener(IPAddress.Parse("127.0.0.1"), 8000);
        
        //server.Start();
        

        while (true) // ensures server does not stop
        {
            Console.WriteLine("Server Running...");
            //    TcpClient client = server.AcceptTcpClient();
            Socket client = server.Accept();
            Console.WriteLine("accepted client");
            ServerThread thread = new ServerThread(client);
        }
        server.Close();
    }
}
