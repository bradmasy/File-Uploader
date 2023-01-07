using System;
using System.ComponentModel;
using System.Diagnostics.Tracing;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
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
            case "GET":
                servlet.DoGet(response, request);
                break;
            case "POST":
                servlet.DoPost(response, request);
                break;
            default:
                Console.WriteLine("Error");
                break;
        }
    }

    private string Read_Incoming_Transmission(string end_pattern)
    {
        byte[] incoming_bytes = new byte[1]; // single byte
        string incoming_data = "";

        while (true)
        {
            _client.Receive(incoming_bytes, 1, 0);

            incoming_data += Encoding.ASCII.GetString(incoming_bytes);

            if (Regex.IsMatch(incoming_data, end_pattern))
            {
                Console.WriteLine("PATTERN FOUND");
                break;
            }
        }

        return incoming_data;
    }

    public override void Run()
    {
        Console.WriteLine("Thread running...");

        string end_pattern   = "\r\n\r\n";
        string incoming_data = Read_Incoming_Transmission(end_pattern);
      
        //Console.WriteLine($"[{incoming_data}]");

        Request request   = new Request(incoming_data);
        Response response = new Response(_client);

        if (request.GetRequestMap()["Request"] == "POST")
        {
            string boundary  = request._requestData["Boundary"] + "--";//@"\w*------WebKitFormBoundary\w*[a-zA-Z0-9]{16}[-]{2}"; // regex for the boundary, specifically the last boundary of the incoming data.
            string multipart = Read_Incoming_Transmission(boundary);
           
            request.Parse_Multipart_Data(multipart); // call to process the multipart data.
        }






        Servlet servlet;

        if (request.GetRequestMap()["User-Agent"].Equals("Browser"))
        {
            servlet = (UploadServlet)Activator.CreateInstance(typeof(UploadServlet), _client);
        }
        else
        {
            Console.WriteLine("initiating client servlet...");
            servlet = (ClientServlet)Activator.CreateInstance(typeof(ClientServlet), _client);
        }

        servlet.SetClient(_client);

        HandleRequest(request, response, servlet);
        Console.WriteLine("Ending of thread");
        _client.Close(); // close the client after the request has been processed.
    }
}
