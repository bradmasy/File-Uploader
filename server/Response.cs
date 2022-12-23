using System.Net;
using System.Text;

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

    private void ProcessPayload(String data)
    {
        //Request request = new Request(data);
        //Console.WriteLine(request);
       // PostRequest postRequest = new PostRequest(data);
    }

    private void CloseStream()
    {
        _stream.Close();
    }

   // public void ReadStream(TcpClient client)
    //{
     //   Byte[] bytes = new Byte[client.Available]; // creates a Byte array the size of whats available.
      //  Console.WriteLine("amount of bytes: " + client.Available);
       // _stream.Read(bytes, 0, bytes.Length);
      //  String data = Encoding.UTF8.GetString(bytes);
      //  Console.WriteLine(data);
       // ProcessPayload(data);
       // CloseStream();
        
   // }
}