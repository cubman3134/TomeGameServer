﻿using Microsoft.VisualBasic;
using System.Net;
using Tome2DServer.Server;


class Program
{
    static void Main(string[] args)
    {
        // TCP server port
        int port = 11420;
        if (args.Length > 0)
            port = int.Parse(args[0]);

        Console.WriteLine($"TCP server port: {port}");

        Console.WriteLine();

        // Create a new TCP chat server
        var server = new GameServer(IPAddress.Any, port);

        // Start the server
        Console.Write("Server starting...");
        server.Start();
        Console.WriteLine("Done!");

        Console.WriteLine("Press Enter to stop the server or '!' to restart the server...");

        // Perform text input
        for (; ; )
        {
            string? line = Console.ReadLine();
            if (string.IsNullOrEmpty(line))
                break;

            // Restart the server
            if (line == "!")
            {
                Console.Write("Server restarting...");
                server.Restart();
                Console.WriteLine("Done!");
                continue;
            }

            // Multicast admin message to all sessions
            line = "(admin) " + line;
            server.Multicast(line);
        }

        // Stop the server
        Console.Write("Server stopping...");
        server.Stop();
        Console.WriteLine("Done!");
    }
}