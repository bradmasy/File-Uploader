using System;
using System.ComponentModel;
using System.Diagnostics.Tracing;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using server;


class ServerThread : BaseThread
{

    private Socket _client;

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
        Console.WriteLine("Thread running...");

        // implement new read method

        byte[] buffer      = new byte[_client.Available];
        int bytes_recieved = _client.Receive(buffer);

        String path             =  Directory.GetCurrentDirectory() + $"\\upload\\temp.txt";
        File.WriteAllBytes(path, buffer);

        // keep looping until all data is recieved

        
        if (bytes_recieved != 0) // block the thread if no bytes
        {
            String data       = Encoding.ASCII.GetString(buffer, 0, buffer.Length);
            Console.WriteLine(data);
            Request request   = new Request(data);
            Console.WriteLine(request);
            Response response = new Response(_client);

            Servlet servlet;
            
            if (request.GetRequestMap()["User-Agent"].Equals("Browser"))
            {
                ConstructorInfo[] info = Type.GetType("server.UploadServlet").GetConstructors();
                //   Console.WriteLine($" Constructor name: {info[0].Name}");
                // servlet = info[0].Invoke(_client);
                servlet = (UploadServlet)Activator.CreateInstance(typeof(UploadServlet), _client);
            }
            else
            {
                Console.WriteLine("initiating client servlet...");
                servlet = (ClientServlet)Activator.CreateInstance(typeof(ClientServlet), _client);
            }

            servlet.SetClient(_client);

            HandleRequest(request, response, servlet);
        }

        _client.Close(); // close the client after the request has been processed.
    }
}
