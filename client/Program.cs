

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;


class Program
{

    private static void Welcome_Message()
    {
        Console.WriteLine("Welcome To the Program, here are your options:");
        Console.WriteLine("1. Upload Photo");
        Console.WriteLine("2. Upload File");
        Console.WriteLine("3. Messenger Application");
        Console.WriteLine("4. Exit");
        Console.WriteLine("\n");

        
    }


    private static void Upload_Photo()
    {
        Console.WriteLine("Please input the path to the image you wish to upload:");

        string image_path = Console.ReadLine();



    }


    private static bool Launch_Application()
    {
        bool continue_program = true;
        string input          = Console.ReadLine();
        string input_regex    = @"^[1-4]$"; // match for only 1 number from 1 - 4
        bool m                = Regex.IsMatch(input, input_regex);

        if (m)
        {
            Console.WriteLine("Here");
            Console.WriteLine(int.Parse(input));
            switch (int.Parse(input))
            {
                case 1:
                    Upload_Photo();
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
            }
        }

        Console.WriteLine($"match: {m}");


        return continue_program;
    }


    static void Main(string[] args)
    {

        Welcome_Message();

        bool continue_program = true;
        while (continue_program)
        {
            continue_program = Launch_Application();
        }

    }
}
