using System.Net.Sockets;
using System.Text;

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

    }

    private static String Welcome_Message()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("Welcome To the Program, here are your options:\n");
        builder.Append("1. Upload Photo\n");
        builder.Append("2. Upload File\n");
        builder.Append("3. Messenger Application\n");
        builder.Append("4. Exit\n");
        builder.Append("\n\0");
       
        return builder.ToString();
    }

    public void DoPost(Response response, Request request)
    {
        Console.WriteLine("---------------------REQUEST----------------------- \n" + request.ToString()); 
    }

    public void SetClient(Socket client)
    {
        _client = client;
    }
}