using System.Net;
using System.Text;
using System.Net.Sockets;

namespace server;

public class Response
{
    private NetworkStream _stream;
    public Response(NetworkStream stream)
    {
        _stream = stream;
    }

    public void WriteToStream(Byte[] response)
    {
        _stream.Write(response, 0, response.Length);
    }



    private void CloseStream()
    {
        _stream.Close();
    }

}