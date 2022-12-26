

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;


class Program
{


    private const string HOST = "localhost";
    private const int PORT    = 8000;
    

    private static void Welcome_Message()
    {
        Console.WriteLine("Welcome To the Program, here are your options:");
        Console.WriteLine("1. Upload Photo");
        Console.WriteLine("2. Upload File");
        Console.WriteLine("3. Messenger Application");
        Console.WriteLine("4. Exit");
        Console.WriteLine("\n");
    }


    private static bool Upload_Photo(TcpClient client)
    {
        Console.WriteLine("Please input the path to the image you wish to upload:\n");

        string image_path = Console.ReadLine();

        String req = Build_Post_Request();

        byte[] message = Encoding.ASCII.GetBytes(req);

        client.GetStream().Write(message, 0, message.Length); // test message.

        return true;

    }

    private String BuildGetRequest()
    {
        StringBuilder builder = new StringBuilder();
        builder.Append("GET \\ 1.1 HTTP");
        return builder.ToString();
    }


    private static bool Launch_Application(TcpClient client)
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
                    continue_program = Upload_Photo(client);
                    break;
                case 2:
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

    private static String Build_Post_Request()
    {
        StringBuilder builder = new StringBuilder();    
        builder.Append("POST / HTTP/1.1\n");
        builder.Append("Host: localhost:8000\n");
        builder.Append("Connection: keep-alive\n");
        builder.Append("User-Agent: command line interface\n\r");

        /**
        * GET / HTTP/1.1
        * Host: localhost:8000
        * Connection: keep-alive
        * sec-ch-ua: "Not?A_Brand";v="8", "Chromium";v="108", "Google Chrome";v="108"
        * sec-ch-ua-mobile: ?0
        * sec-ch-ua-platform: "Windows"
        * Upgrade-Insecure-Requests: 1
        * User-Agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/108.0.0.0 Safari/537.36
        * Accept: text/html,application/xhtml+xml,application/xml;q=0.9,image/avif,image/webp,image/apng,*; q = 0.8,application / signed - exchange; v = b3; q = 0.9
        * Sec - Fetch - Site: none
        * Sec - Fetch - Mode: navigate
        * Sec - Fetch - User: ?1
        * Sec - Fetch - Dest: document
        * Accept - Encoding: gzip, deflate, br
        * Accept - Language: en - US,en; q = 0.9
        * */
        return builder.ToString();
    }

    static void Main(string[] args)
    {
        TcpClient client = new TcpClient(HOST, 8000);
      //  byte[] message   = Encoding.ASCII.GetBytes("Connection Established...");

   //     client.GetStream().Write(message, 0, message.Length); // test message.


        bool continue_program = true;

        Welcome_Message();

        while (continue_program)
        {
            continue_program = Launch_Application(client);
        }

        Console.WriteLine("Good Bye!");

    }
}
