using System;
using System.ComponentModel;
using System.Diagnostics.Tracing;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using server;


class ServerThread : BaseThread
{

    //private TcpClient _client;
    private Socket _client;

    //  public ServerThread(TcpClient client) : base()
    // {
    //    _client = client;
    //   _thread.Start();
    // }


    public ServerThread(Socket client) : base()
    {
        _client = client;
        _thread.Start(); // start the thread.
    }


    void HandleRequest(Request request, Response response, Servlet servlet)
    {
        switch (request.GetRequestMap()["Request"])
        {
            case "Get":
                servlet.DoGet(response, request);
                break;
            case "Post":
                servlet.DoPost(response, request);
                break;
            default:
                Console.WriteLine("Error");
                break;
        }
    }

    public override void Run()
    {
        Console.WriteLine("running...");

        // NetworkStream stream = _client.GetStream();

        Console.WriteLine($"amount of data: {_client.Available}");
        byte[] buffer = new byte[_client.Available];
        int bytes_recieved = _client.Receive(buffer);
        Console.WriteLine("buffer size: " + buffer.Length);
        String data = Encoding.ASCII.GetString(buffer, 0, buffer.Length);
       
        Console.WriteLine(data);
        Request request   = new Request(data);
        Response response = new Response(_client);

        Servlet servlet;

        if (request.GetRequestMap()["User-Agent"].Equals("Browser"))
       {
            ConstructorInfo[] info = Type.GetType("server.UploadServlet").GetConstructors();
            Console.WriteLine($" Constructor name: {info[0].Name}");
           // servlet = info[0].Invoke(_client);
            servlet = (UploadServlet) Activator.CreateInstance(typeof(UploadServlet),_client);
            servlet.SetClient(_client);
        }
        else
        {
            Console.WriteLine("initiating client servlet...");
            servlet = Activator.CreateInstance<ClientServlet>();
        }

         HandleRequest(request, response, servlet);
        _client.Close(); // close the client after the request has been processed.
    }

    //  public override void Run()
    // {
    //    Console.WriteLine("running...");

    //     NetworkStream stream = _client.GetStream();

    //     while(_client.Available < 3)
    //    {
    // wait for enough bytes to be available
    //   }


    //  Byte[] bytes      = new Byte[_client.Available]; // creates a Byte array the size of whats available.

    //   stream.Read(bytes, 0, bytes.Length); // writes to the byte array.

    //    String data       = Encoding.UTF8.GetString(bytes); // the data to string.
    //    Console.WriteLine(data);
    //    Request request   = new Request(data);
    //   Response response = new Response(stream);

    //  Servlet servlet;

    //        if (request.GetRequestMap()["User-Agent"].Equals("Browser"))
    //      {
    //        servlet = Activator.CreateInstance<UploadServlet>();
    //      servlet.SetClient(_client);
    //  }
    // else {
    //       Console.WriteLine("initiating client servlet...");
    //     servlet = Activator.CreateInstance<ClientServlet>();
    //  }

    //  HandleRequest(request,response,servlet);
    //  _client.Close(); // close the client after the request has been processed.
    // }
}