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

    // public void DoPost(Response response, Request request)
    // {
    //     Console.WriteLine("---------------------REQUEST----------------------- \n" + request.ToString()); 

    //     string content = request.GetMultiData()["Content"]; // gets all the data we need to write
    //     string fileName = request.GetMultiData()["fileName"];
    //    

    //     // write a file here from the request 
    // }



     private void ProcessPostRequest(Response response, Request request)
        {
            Console.WriteLine("Status: " + request._status);
           if (request._status == 200)
           {
                string directory_contents = Print_Directory_Contents();
                Console.WriteLine(directory_contents);
                Byte[] responseStr = Encoding.UTF8.GetBytes(directory_contents);
                response.WriteToStream(responseStr); // send back to client as response
                Console.Read(); // blocks the socket to read.
            }
        }
        

        
    public static string Print_Directory_Contents() {
        StringBuilder filesList = new StringBuilder();
        string uploadsDirectory = Directory.GetCurrentDirectory() + "\\upload";
        string[] uploadedFiles = Directory.GetFiles(uploadsDirectory);

        foreach (string fileName in uploadedFiles) {
            filesList.Append(Path.GetFileName(fileName) + "\n");
            // Console.WriteLine(Path.GetFileName(fileName));
        }

        filesList.Append("\r\n");

        return filesList.ToString();
    }

        
        public void DoPost(Response response, Request request)
        {
            Console.WriteLine($"Post request: {request}");
            if (request.GetRequestMap() != null && request.GetRequestMap().ContainsKey("Boundary"))
            {
                ProcessPostRequest(response, request);
            }
        }

    public void SetClient(Socket client)
    {
        _client = client;
    }
}