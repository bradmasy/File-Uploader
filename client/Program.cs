

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
    private const int PORT = 8000;
    private const int TEXT_FILE = 1;
    private const int IMAGE_FILE = 2;



    private static bool Upload_Photo()
    {

        bool continue_program = true;

        Dictionary<String, String> data_to_send = Get_User_Input("image");

        Console.WriteLine($"photo bytes: [{data_to_send["Content"]}]");
        String req = Build_Post_Request(IMAGE_FILE, data_to_send);

        Console.WriteLine(req);

        byte[] message = Encoding.ASCII.GetBytes(req);
        Client cl = new Client(PORT, HOST_IP);

        cl.client_socket.Send(message);
        cl.client_socket.Close();

        return continue_program;

    }

    private String BuildGetRequest()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("GET \\ 1.1 HTTP");
        return builder.ToString();
    }


  private static string Read_Incoming_Transmission(Client c,string end_pattern)
    {
        byte[] incoming_bytes = new byte[1]; // single byte
        string incoming_data = "";

        while (true)
        {
            c.client_socket.Receive(incoming_bytes, 1, 0);

            incoming_data += Encoding.ASCII.GetString(incoming_bytes);

            if (Regex.IsMatch(incoming_data, end_pattern))
            {
                break;
            }
        }

        return incoming_data;
    }
   
    private static bool Upload_File()
    {
        bool continue_program = true;
        Dictionary<String, String> data_to_send = Get_User_Input("file");

        String req = Build_Post_Request(TEXT_FILE, data_to_send);
        byte[] message = Encoding.ASCII.GetBytes(req);
        Client cl = new Client(PORT, HOST_IP);

        cl.client_socket.Send(message);


        string incoming_data = Read_Incoming_Transmission(cl, "\r\n");
        Console.WriteLine($"Current Files Uploaded on Server...\n{incoming_data}");
        

        cl.client_socket.Close();

        return continue_program;
    }


    private static Dictionary<String, String> Get_User_Input(string inputType)
    {
        Dictionary<String, String> data_to_send = new Dictionary<String, String>();
        Console.WriteLine($"Please input the path to the {inputType} you wish to upload:\n");
        string file_path = Console.ReadLine();
        Console.WriteLine($"Please input the caption of the {inputType}:\n");
        string caption = Console.ReadLine();
        Console.WriteLine("Please input the date (please use format: mm\\dd\\yyyy):\n");
        string date = Console.ReadLine();
        string data = File.ReadAllText(file_path);
        String[] file_path_split = file_path.Split("\\");
        string file_name = file_path_split[file_path_split.Length - 1];

        data_to_send.Add("Caption", caption);
        data_to_send.Add("Date", date);
        data_to_send.Add("File_Path", file_path);
        data_to_send.Add("Filename", file_name);
        data_to_send.Add("Content", data);
        data_to_send.Add("Type", "Submit");

        return data_to_send;
    }


  
    private static bool Launch_Application()
    {
        bool continue_program = true;
        string input = Console.ReadLine();
        string input_regex = @"^[1-4]$"; // match for only 1 number from 1 - 4
        bool m = Regex.IsMatch(input, input_regex);

        if (m)
        {
            switch (int.Parse(input))
            {
                case 1:
                    continue_program = Upload_Photo();
                    Console.WriteLine("\nImage uploaded successfully.\n");
                    break;
                case 2:
                    continue_program = Upload_File();
                    Console.WriteLine("\n\tFile uploaded successfully.\n");
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
        builder.Append("syTrWiNhk1fONdsm");
        return builder.ToString();
    }

    private static String Build_Get_Request()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("GET / HTTP/1.1\n");
        builder.Append("Host: localhost:8000\n");
        builder.Append("Connection: keep-alive\n");
        builder.Append("User-Agent: CLI\n");
        return builder.ToString();
    }




    private static string Build_File(string boundary, Dictionary<String, String> data_dictionary, string contentType)
    {
        StringBuilder builder = new StringBuilder();
        builder.Append($"------WebKitFormBoundary{boundary}\r\n");
        builder.Append($"Content-Disposition: form-data; name=\"fileName\"; filename=\"{data_dictionary["Filename"]}\"\r\n");
        builder.Append($"Content-Type: {contentType}\r\n\r\n");
        builder.Append(data_dictionary["Content"]);

        // the text file caption data.
        builder.Append($"\r\n------WebKitFormBoundary{boundary}\r\n");
        builder.Append("Content-Disposition: form-data; name=\"caption\"\r\n\r\n");
        builder.Append($"{data_dictionary["Caption"]}");

        // the text file date.
        builder.Append($"\r\n------WebKitFormBoundary{boundary}\r\n");
        builder.Append("Content-Disposition: form-data; name=\"date\"\r\n\r\n");
        builder.Append($"{data_dictionary["Date"]}");

        // the text file submit type.
        builder.Append($"\r\n------WebKitFormBoundary{boundary}\r\n");
        builder.Append("Content-Disposition: form-data; name=\"submit\"\r\n\r\n");
        builder.Append($"{data_dictionary["Type"]}");

        builder.Append($"\r\n------WebKitFormBoundary{boundary}--\r\n");

        return builder.ToString();
    }
    private static String Build_Post_Request(int data_type, Dictionary<String, String> data_dictionary)
    {
        StringBuilder builder = new StringBuilder();
        String boundary = Create_Boundary();

        builder.Append("POST / HTTP/1.1\r\n");
        builder.Append("Host: localhost:8000\r\n");
        builder.Append("User-Agent: CLI\r\n");
        builder.Append("Connection: keep-alive\r\n");
        builder.Append($"Content-Length: {data_dictionary["Content"].Length}\r\n");
        builder.Append("Cache-Control: max-age=0\r\n");
        builder.Append("sec-ch-ua: \"Not?A_Brand\";v=\"8\", \"Chromium\";v=\"108\", \"Google Chrome\";v=\"108\"\r\n");
        builder.Append("sec-ch-ua-mobile: ?0\r\n");
        builder.Append("sec-ch-ua-platform: \"Windows\"\r\n");
        builder.Append("Upgrade-Insecure-Requests: 1\r\n");
        builder.Append("Origin: http://localhost:8000\r\n");
        builder.Append($"Content-Type: multipart/form-data; boundary=----WebKitFormBoundary{boundary}\r\n");
        builder.Append("Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9\r\n");
        builder.Append("Sec-Fetch-Site: same-origin\r\n");
        builder.Append("Sec-Fetch-Mode: navigate\r\n");
        builder.Append("Sec-Fetch-User: ?1\r\n");
        builder.Append("Sec-Fetch-Dest: document\r\n");
        builder.Append("Referer: local\r\n");
        builder.Append("Accept-Encoding: gzip, deflate, br\r\n");
        builder.Append("Accept-Language: en-US,en;q=0.9\r\n\r\n");

        // this is where the multipart data is written.
        switch (data_type)
        {
            case TEXT_FILE:
                // the text file content.
                builder.Append(Build_File(boundary, data_dictionary, "text/plain"));
                break;
            case IMAGE_FILE:
                // the image content.
                builder.Append(Build_File(boundary, data_dictionary, "image/png"));
                break;
            default:
                break;
        }
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
        Socket client = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
        client.Connect(endpoint);
        return client;
    }

    private static String Welcome_Message()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("Welcome To File Uploader, here are your options:\n");

        return builder.ToString();
    }

    private static String Menu_Options()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("1. Upload Photo\n");
        builder.Append("2. Upload File\n");
        builder.Append("3. Messenger Application\n");
        builder.Append("4. Exit\n");
        builder.Append("\n\0");

        return builder.ToString();
    }

    public static void Main(string[] args)
    {

        bool continue_program = true;
        Console.WriteLine(Welcome_Message());

        while (continue_program)
        {
            Console.WriteLine(Menu_Options());
            continue_program = Launch_Application();
        }

    }
}


