

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


    private static bool Launch_Application()
    {
        bool continue_program = true;
        string input = Console.ReadLine();
        Console.WriteLine($"input: {input}");
        string input_regex = @"^[1-4]$";
        Match m = Regex.


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
