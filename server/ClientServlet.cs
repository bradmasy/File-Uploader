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
        Console.WriteLine("Launching Client Servlet");
        String welcome = Welcome_Message();
        byte[] message = Encoding.ASCII.GetBytes(welcome);
        response.WriteToStream(message); 
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

        string content = request.GetMultiData()["Content"]; // gets all the data we need to write
        string fileName = request.GetMultiData()["fileName"];
        string path = Directory.GetCurrentDirectory() + "\\upload\\" + fileName; // file being created
        File.WriteAllText(path, content); // data written to file

        // write a file here from the request 
    }

    public void SetClient(Socket client)
    {
        _client = client;
    }
}