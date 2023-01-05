

using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using client;

class Program
{


    private const string HOST = "localhost";
    private const string HOST_IP = "127.0.0.1";
    private const int PORT    = 8000;
    private const int TEXT_FILE = 1;



    private static bool Upload_Photo(Socket client)
    {
        Console.WriteLine("Please input the path to the image you wish to upload:\n");

        string image_path = Console.ReadLine();

       // String req = Build_Post_Request();

       // byte[] message = Encoding.ASCII.GetBytes(req);

//        client.GetStream().Write(message, 0, message.Length); // test message.

        return true;

    }

    private String BuildGetRequest()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("GET \\ 1.1 HTTP");
        return builder.ToString();
    }

    private static bool Upload_File()
    {
      
        bool continue_program = true;
        Console.WriteLine("Please input the path to the file you wish to upload:\n");

        string file_path = Console.ReadLine();
        String data = "";
        using(FileStream file =  File.OpenRead(file_path)){
            byte[] b = new byte[1024];
            UTF8Encoding temp = new UTF8Encoding(true);
            while(file.Read(b,0, b.Length) > 0){
                data += temp.GetString(b);
            }
        };
        
        Console.WriteLine($"file data: {data}");


        String req = Build_Post_Request(TEXT_FILE, data);
        Console.WriteLine(req);
        byte[] message = Encoding.ASCII.GetBytes(req);
                Client cl = new Client(PORT, HOST_IP);

        cl.client_socket.Send(message);


        return continue_program;
    }

    private static bool Launch_Application()
    {
        bool continue_program = true;
        string input          = Console.ReadLine();
        string input_regex    = @"^[1-4]$"; // match for only 1 number from 1 - 4
        bool m                = Regex.IsMatch(input, input_regex);

        if (m)
        {
            switch (int.Parse(input))
            {
                case 1:
                     // continue_program = Upload_Photo(cl);
                    break;
                case 2:
                    continue_program = Upload_File();
                    break;
                case 3:
                    break;
                case 4:
                    continue_program = false;
                    break;
            }
        }

        return continue_program;
    }

    private static String Create_Boundary()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("aB29sh23wl109456");
        return builder.ToString() ;
    }

    private static String Build_Get_Request()
    {
        StringBuilder builder = new StringBuilder() ;
        builder.Append("GET / HTTP/1.1\n");
        builder.Append("Host: localhost:8000\n");
        builder.Append("Connection: keep-alive\n");
        builder.Append("User-Agent: CLI\n");       
        return builder.ToString();
    }
    private static String Build_Post_Request(int data_type, String data )
    {
        StringBuilder builder = new StringBuilder();
        String charset        = "UTF-8";
        String boundary       = Create_Boundary();

        builder.Append("POST / HTTP/1.1\r\n");
        builder.Append("Host: localhost:8000\n");
        builder.Append("Connection: keep-alive\r\n");
        builder.Append("User-Agent: CLI\r\n");
        builder.Append($"Content-Type: multipart/form-data; boundary=----WebKitFormBoundary{boundary}\r\n");

        switch(data_type){
            case TEXT_FILE:
                builder.Append($"------WebKitFormBoundary{boundary}\r\n");
                builder.Append($"Content-Disposition: form-data; name=\"fileName\"; filename={"test.txt"}\r\n");
                builder.Append($"Content-Type: text/plain\r\n");
                builder.Append("\r\n");
                builder.Append(data);
                break;
            
        }
       
//  C:\Users\bradl\Pictures\mytext.txt
// POST / HTTP/1.1
// Host: localhost:8000
// Connection: keep-alive
// Content-Length: 1079
// Cache-Control: max-age=0
// sec-ch-ua: "Not?A_Brand";v="8", "Chromium";v="108", "Google Chrome";v="108"
// sec-ch-ua-mobile: ?0
// sec-ch-ua-platform: "Windows"
// Upgrade-Insecure-Requests: 1
// Origin: http://localhost:8000
// Content-Type: multipart/form-data; boundary=----WebKitFormBoundaryOuHfNBQciXtH6o6Z
// User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/108.0.0.0 Safari/537.36
// Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9
// Sec-Fetch-Site: same-origin
// Sec-Fetch-Mode: navigate
// Sec-Fetch-User: ?1
// Sec-Fetch-Dest: document
// Referer: http://localhost:8000/
// Accept-Encoding: gzip, deflate, br
// Accept-Language: en-US,en;q=0.9

// ------WebKitFormBoundaryOuHfNBQciXtH6o6Z
// Content-Disposition: form-data; name="fileName"; filename="mytext.txt"
// Content-Type: text/plain

// text file testLorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and scrambled it to make a type specimen book. It has survived not only five centuries, but also the leap into electronic typesetting, remaining essentially unchanged. It was popularised in the 1960s with the release of Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions of Lorem Ipsum.
// ------WebKitFormBoundaryOuHfNBQciXtH6o6Z
// Content-Disposition: form-data; name="caption"

// test file
// ------WebKitFormBoundaryOuHfNBQciXtH6o6Z
// Content-Disposition: form-data; name="date"

// 2023-01-04
// ------WebKitFormBoundaryOuHfNBQciXtH6o6Z
// Content-Disposition: form-data; name="submit"

// Submit
// ------WebKitFormBoundaryOuHfNBQciXtH6o6Z--














        return builder.ToString();
    }

    private static void Send_Initial_Request(Client cl)
    {
        String req = Build_Get_Request();
        cl.client_socket.Send(Encoding.ASCII.GetBytes(req));
    }

    private static Socket Establish_Connection()
    {
        IPEndPoint endpoint = new IPEndPoint(IPAddress.Parse(HOST_IP), PORT);
        Socket client       = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        client.Connect(endpoint);
        return client;
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

    public static void Main(string[] args)
    {
        
       // Socket client = Establish_Connection();

        //Send_Initial_Request(cl);
        
        bool continue_program = true;
   
      //  Console.WriteLine(client.Available);
        //Console.WriteLine(Welcome_Message());
        
        // if (cl.client_socket.Connected)
        // {
        //     String message   = "";
        //     byte[] bytes_rec = new byte[1];

        //     while (true)
        //    {
        //        if (cl.client_socket.Receive(bytes_rec,1,0) == 0  || Encoding.ASCII.GetString(bytes_rec,0,1) == "\0")
        //         {
        //             break;
        //         }
        //         message += Encoding.ASCII.GetString(bytes_rec,0,1);
        //    }

          //  Console.WriteLine(message);

            Console.WriteLine(Welcome_Message());

            while (continue_program)
            {
               continue_program = Launch_Application();
            }

        

    }
}


