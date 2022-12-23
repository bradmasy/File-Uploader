using System;
using System.Diagnostics.Tracing;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using server;


class ServerThread : BaseThread
{

    private TcpClient _client;

    public ServerThread(TcpClient client) : base()
    {
        _client = client;
        _thread.Start();
    }

    void HandleRequest(Request request, Response response, Servlet servlet)
    {
        switch (request.GetRequestMap()["Request"])
        {
            case "Get":
                servlet.DoGet(response, request);
                break;
            case "Post":
                Console.WriteLine("HERE");
                Console.WriteLine("BOUNDARY: " + request.GetBoundary());
                servlet.DoPost(response,request);
             //   servlet.DoGet(response,request);
                break;
            default:
                Console.WriteLine("Error");
                break;
        }
        
    }
    public override void Run()
    {
        Console.WriteLine("running...");
        NetworkStream stream = _client.GetStream();
        
        while(_client.Available < 3)
        {
            // wait for enough bytes to be available
        }
        
        Byte[] bytes      = new Byte[_client.Available]; // creates a Byte array the size of whats available.
        
        stream.Read(bytes, 0, bytes.Length); // writes to the byte array.
        
        String data       = Encoding.UTF8.GetString(bytes); // the data to string.
        Request request   = new Request(data);
        Response response = new Response(stream);
        
        Servlet servlet;

        if (request.GetRequestMap()["User-Agent"].Equals("Browser"))
        {
            servlet = Activator.CreateInstance<UploadServlet>();
            servlet.SetClient(_client);
        }
        else {
            servlet = Activator.CreateInstance<ClientServlet>();
        }
        
        HandleRequest(request,response,servlet);
        _client.Close(); // close the client after the request has been processed.
    }
    
    
    
    
    
}