using System;
using System.Net.Sockets;

class ServerThread : BaseThread
{
    private TcpClient _client;

    public ServerThread(Socket sock):base(sock){
    }
    
    public ServerThread(TcpClient client):base()
    {
        _client = client;
    }
    public override void Run() {
        Console.WriteLine("running..." + _client);
        
    }
}