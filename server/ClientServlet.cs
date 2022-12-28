using System.Net.Sockets;

namespace server;

public class ClientServlet: Servlet
{
    private Socket _client;

    public ClientServlet(Socket client)
    {
        _client = client;
    }
    
    public void DoGet(Response response, Request request)
    {
        Console.WriteLine("Launching Client Servlet");
    }

    public void DoPost(Response response, Request request)
    {
        throw new NotImplementedException();
    }

    public void SetClient(Socket client)
    {
        _client = client;
    }
}