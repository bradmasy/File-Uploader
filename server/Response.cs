using System.Net;
using System.Text;
using System.Net.Sockets;

namespace server;

public class Response
{
    private Socket _stream;
    public Response(Socket stream)
    {
        _stream = stream;
    }

    public void WriteToStream(Byte[] response)
    {
        //_stream.Write(response, 0, response.Length);
        _stream.Send(response);
    }



    private void CloseStream()
    {
        _stream.Close();
    }

}