// See https://aka.ms/new-console-template for more information

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Program
{
    static void Main(string[] args)
    {

        TcpListener server = new TcpListener(IPAddress.Parse("127.0.0.1"), 8000);
        
        server.Start();
        
        while (true) // ensures server does not stop
        {
            Console.WriteLine("Server Running...");
            TcpClient client = server.AcceptTcpClient();
            Console.WriteLine("accepted client");
            ServerThread thread = new ServerThread(client); 
        }
        
    }
}
