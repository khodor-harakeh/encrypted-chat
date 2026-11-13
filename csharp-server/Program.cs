// IMPORTS (always on the absolute top)
// Core Language Tools
using System;
// IP NETWORK TYPES
using System.Net;
// LOW-LEVEL NETWORKING TOOLS
using System.Net.Sockets;
// CHARACTER ENCODINGS
using System.Text;

// CLASS FOR SERVER -ENCAPSULATES THE CODE (TRADITIONAL C# ACT)
class Server
{
    // WHERE OS STARTS EXECUTING AFTER "dotnet run" 
    static void Main()
    {
        // 1. Listen on 0.0.0.0:5000 (all network interfaces)
	// defining ip address to be capable to connect to other Devices not just same device home 127.0.0.1
        IPAddress ipAddress = IPAddress.Any; // MEANS 0.0.0.0
        int port = 5000; // use port 5000
        TcpListener listener = new TcpListener(ipAddress, port); // new socket for server to listen on 0.0.0.0:5000

        // Allows address reuse (like SO_REUSE)
	// SocketOptionLevel.Socket -> applied to socket layer
	// SocketOptionName.ReuseAddress maps to  SO_REUSEADDR (Linux)
	// true -> NO TIME_WAIT
        listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
	
	// POSIX: here mirrors BSD listen() system call
        listener.Start();
	// print
        Console.WriteLine($"[C# Server] Listening on 0.0.0.0:{port} ...");
        
        // 2. Accept incoming client connection (blocking)
	// POSIX: here mirrors BSD accept() system call
        TcpClient client = listener.AcceptTcpClient();
	// return IPEndpoint object by explicit type casting in order to access .Address and .Port
	// ! null-forgiving operator tells the compiler don't throw a warning, trust me not null ;)
        IPEndPoint clientEndPoint = (IPEndPoint)client.Client.RemoteEndPoint!;
	// print
        Console.WriteLine($"[C# Server] Client connected from {clientEndPoint.Address}:{clientEndPoint.Port}");

        // 3. Get network stream for reading/writing
	// what is a network stream ?
	// an abstraction for a continuous sequence of bytes flowing rom source to destination over time
	// wraps the socket inside a Stream interface -> to be capable to read/write data same as on disk
	// good practice for encryption later
        NetworkStream stream = client.GetStream();
	// pre-allocates a 1 KB bucket in memory
	// helps later to prevent memory fragmentation and garbage collection lag
        byte[] buffer = new byte[1024];

	// the loop where all the main work is done
        while (true)
        {  
            // 4. Read bytes from client (mirrors POSIX BSD recv() system call)
	    // stream.Read() dumps raw bytes into the existing bucket
	    // arguments: (analogy)
	    // buffer -> whiteboard to write on
	    // 0 -> start writing on the beginning (position zero)
	    // buffer.Length -> stop if you reach edge (1024 bytes)
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            
            // bytesRead == 0 means client disconnected cleanly (same as b"" in Python)
            if (bytesRead == 0)
            {
                Console.WriteLine("[C# Server] Client disconnected.");
                break;
            }

	    // buffer bytes(UTF-8) -> string
	    // UTF-8 -> universal standard to represent characters from 1 to 4 bytes per character
            string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
	    //  if "/quit" is recieved -> end chat from client side
            if (message == "/quit")
            {
                Console.WriteLine("[C# Server] Client ended the chat.");
                break;
            }
	    
	    // prints client's message
            Console.WriteLine($"Client: {message}");

            // 5. Send server reply
            Console.Write("You (C# Server): ");
	    // read string input 
	    // ? means it can have null value
            string? reply = Console.ReadLine();
            
	    // either reply == null (pressed ctrl+D/ctrl+Z) -> EOF
	    // or reply == "" (empty string with zero characters)
	    // assigns reply = " " (a single whitespace) in both cases 
            if (string.IsNullOrEmpty(reply)) reply = " ";

	    // string -> bytes (UTF-8)
            byte[] replyBytes = Encoding.UTF8.GetBytes(reply);
	    // mirrors POSIX BSD send() system call
            stream.Write(replyBytes, 0, replyBytes.Length);
	
	    // if server sent "/quit" -> exit chat from server side
            if (reply == "/quit")
            {
                Console.WriteLine("[C# Server] Closing chat.");
                break;
            }
        }

        // 6. Clean up
	// Why need it?
	// Prevents: 
	// 1- Port Exhaustion
	// 2- Memory Leaks
	// 3- Firewall/Connection Stalls
        client.Close();
        listener.Stop();
	// Informs that session is closed
        Console.WriteLine("[C# Server] Closed.");
    }
}
