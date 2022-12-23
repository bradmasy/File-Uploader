using System.Net.Sockets;

namespace server;

public class ClientServlet: Servlet
{
    private TcpClient _client;

    public ClientServlet()
    {
        _client = new TcpClient();
    }
    
    public void DoGet(Response response, Request request)
    {
        throw new NotImplementedException();
    }

    public void DoPost(Response response, Request request)
    {
        throw new NotImplementedException();
    }

    public void SetClient(TcpClient client)
    {
        _client = client;
    }
}