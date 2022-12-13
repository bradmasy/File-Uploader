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
        Console.WriteLine("Server Running...");
        TcpClient client = server.AcceptTcpClient();
        Console.WriteLine("accepted client");
        ServerThread thread = new ServerThread(client); 
        thread.Run();
        // IPHostEntry iPHost  = Dns.GetHostEntry(Dns.GetHostName());
        // IPAddress iPAddress = iPHost.AddressList[0];
        // IPAddress ip =  IPAddress.Parse("127.0.0.1");
        // IPEndPoint endpoint = new IPEndPoint(ip, 8999);
        // Socket socket       = new Socket(iPAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        //
        // try {
        //     socket.Bind(endpoint);
        //     socket.Listen(10);
        //
        //     while (true)
        //     {
        //         Console.WriteLine("waiting...");
        //
        //         Socket clientSocket = socket.Accept();  
        //         Console.WriteLine("one...");
        //
        //         ServerThread thread = new ServerThread(clientSocket);
        //         thread.Run();
        //         Console.WriteLine("two...");
        //
        //         
        //         
        //     }
        // }
        // catch (Exception e)
        // {
        //     Console.WriteLine(e.Message);
        // }
    }
}
