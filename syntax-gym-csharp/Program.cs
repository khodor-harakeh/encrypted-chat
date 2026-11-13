// Imports
using System;
using System.Text;


// Program Class
class Program
{
    // Displays prompt
    static string Prompt(string text)
    {
        Console.Write(text);
        return Console.ReadLine().Trim();
    }

    // Convert string to UTF-8 byte array
    static byte[] ToBytes(string text)
    {
        return Encoding.UTF8.GetBytes(text);
    }

    // Convert UTF-8 byte array to string
    static string FromBytes(byte[] data)
    {
        return Encoding.UTF8.GetString(data);
    }

    // Echo message
    static void EchoRound(string message)
    {
        byte[] byte_message = ToBytes(message);
        int count = byte_message.Length;
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string echoed_message = FromBytes(byte_message);
        Console.WriteLine($"[{timestamp}] byte count = {count} bytes | Echo: {echoed_message}");
    }

    // main entry point
    static void Main(string[] args)
    {
        // Print app banner
        Console.WriteLine($"App: C# Syntax Gym | Port 5000");
        
        // Run an infinite loop
        while(true)
        {
            // Prompt for input
            string message = Prompt("> Input Your message here: ");

            // if user types `/quit` -> print "Closing..." and `break`
            if (message == "/quit")
            {
                Console.WriteLine("Closing...");
                break;
            }

            // if user submits an empty string -> print "Input a valid message!" and `continue`
            if (string.IsNullOrEmpty(message)) Console.WriteLine("Input a valid message!");

            // returns echoed message
            EchoRound(message);
        }
    }
}
