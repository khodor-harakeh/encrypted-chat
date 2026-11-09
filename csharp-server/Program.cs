using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Server
{
    static void Main()
    {
        // 1. Listen on 0.0.0.0:5000 (all network interfaces)
        IPAddress ipAddress = IPAddress.Any; // 0.0.0.0
        int port = 5000;
        TcpListener listener = new TcpListener(ipAddress, port);

        // Allows address reuse (like SO_REUSEADDR)
        listener.Server.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

        listener.Start();
        Console.WriteLine($"[C# Server] Listening on 0.0.0.0:{port} ...");

        // 2. Accept incoming client connection (blocking)
        TcpClient client = listener.AcceptTcpClient();
        IPEndPoint clientEndPoint = (IPEndPoint)client.Client.RemoteEndPoint!;
        Console.WriteLine($"[C# Server] Client connected from {clientEndPoint.Address}:{clientEndPoint.Port}");

        // 3. Get network stream for reading/writing
        NetworkStream stream = client.GetStream();
        byte[] buffer = new byte[1024];

        while (true)
        {
            // 4. Read bytes from client (recv)
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            
            // bytesRead == 0 means client disconnected cleanly (same as b"" in Python)
            if (bytesRead == 0)
            {
                Console.WriteLine("[C# Server] Client disconnected.");
                break;
            }

            string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            if (message == "/quit")
            {
                Console.WriteLine("[C# Server] Client ended the chat.");
                break;
            }

            Console.WriteLine($"Client: {message}");

            // 5. Send server reply
            Console.Write("You (C# Server): ");
            string? reply = Console.ReadLine();
            
            if (string.IsNullOrEmpty(reply)) reply = " ";

            byte[] replyBytes = Encoding.UTF8.GetBytes(reply);
            stream.Write(replyBytes, 0, replyBytes.Length);

            if (reply == "/quit")
            {
                Console.WriteLine("[C# Server] Closing chat.");
                break;
            }
        }

        // 6. Clean up
        client.Close();
        listener.Stop();
        Console.WriteLine("[C# Server] Closed.");
    }
}﻿
