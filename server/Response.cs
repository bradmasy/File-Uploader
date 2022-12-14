using System.Net;

namespace server;
using System.Net.Sockets;

public class Response
{
    private NetworkStream _stream;
    public Response(NetworkStream stream)
    {
        _stream = stream;
    }

    public void WriteToStream(Byte[] response)
    {
        _stream.Write(response,0,response.Length);
        
    }
}