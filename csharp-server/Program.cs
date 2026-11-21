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
        // mirrors POSIX BSD socket() + bind() system calls
        TcpListener listener = new TcpListener(ipAddress, port); // Creates a socket and reserves port 5000

        // Allows address reuse (like SO_REUSE)
	    // SocketOptionLevel.Socket -> applied to socket layer
	    // SocketOptionName.ReuseAddress maps to  SO_REUSEADDR (Linux)
	    // true -> NO TIME_WAIT
        listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
	
	    // POSIX: here mirrors BSD listen() system call
        listener.Start(); // Marks the socket as passive and ready to accept incoming connections.
	    // print
        Console.WriteLine($"[C# Server] Listening on 0.0.0.0:{port} ...");
        
        // 2. Accept incoming client connection (blocking)
	    // POSIX: here mirrors BSD accept() system call
        // Pauses until a client completes the 3-way handshake, then returns a new socket for that client.
        TcpClient client = listener.AcceptTcpClient();
	    // return IPEndpoint object by explicit type casting in order to access .Address and .Port
	    // ! null-forgiving operator tells the compiler don't throw a warning, trust me not null ;)
        IPEndPoint clientEndPoint = (IPEndPoint)client.Client.RemoteEndPoint!;
	    // print
        Console.WriteLine($"[C# Server] Client connected from {clientEndPoint.Address}:{clientEndPoint.Port}");

        // 3. Get network stream for reading/writing
	    // what is a network stream ?
	    // an abstraction for a continuous sequence of bytes flowing from source to destination over time
	    // wraps the socket inside a Stream interface -> to be capable to read/write data same as on disk
	    // good practice for encryption later
        NetworkStream stream = client.GetStream();

	    // the loop where all the main work is done
        while (true)
        { 
            // 4. Protocol.ReceiveEncryptedMessage handles reading the stream & detecting disconnects!
            string? message = Protocol.ReceiveEncryptedMessage(stream, Protocol.SharedKey);
            if (message == null)
            {
                Console.WriteLine("[C# Server] Client disconnected.");
                break;
            }
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

	        Protocol.SendEncryptedMessage(stream, reply, Protocol.SharedKey);
	
	        // if server sent "/quit" -> exit chat from server side
            if (reply == "/quit")
            {
                Console.WriteLine("[C# Server] Closing chat.");
                break;
            }
        }

        // 6. Clean up
        // mirrors POSIX BSD close() system call
        // Sends a TCP FIN packet to end the connection and frees the OS file descriptor.
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
