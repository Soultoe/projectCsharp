﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;


class MyTcpListener
{
    /**
     * GIT
     * https://github.com/Soultoe/projectCsharp
     */

    public static List<TcpClient> clients = new List<TcpClient>();

    public static void Main()
    {
        TcpListener server = null;
        try
        {
            // Set the TcpListener on port 13000.
            Int32 port = 13000;
            IPAddress localAddr = IPAddress.Parse("127.0.0.1");

            // TcpListener server = new TcpListener(port);
            server = new TcpListener(localAddr, port);

            // Start listening for client requests.
            server.Start();
        

            // Enter the listening loop.
            while (true)
            {
                // Console.Write("Waiting for a connection... ");

                // Perform a blocking call to accept requests.
                // You could also user server.AcceptSocket() here.
                TcpClient client = server.AcceptTcpClient();
                clients.Add(client);

                // Console.WriteLine("Connected!");

                ThreadPool.QueueUserWorkItem(ThreadProc, client);

            }
        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException: {0}", e);
        }
        finally
        {
            // Stop listening for new clients.
            server.Stop();
        }


        Console.WriteLine("\nHit enter to continue...");
        Console.Read();
    }

    // Accept incoming connections
    //https://stackoverflow.com/questions/5339782/how-do-i-get-tcplistener-to-accept-multiple-connections-and-work-with-each-one-i


    private static void ThreadProc(object obj){
        var client = (TcpClient)obj;

        while(client.Connected)
        {
            String msg = receiveData(client);

            ThreadPool.QueueUserWorkItem(SendToAll, msg);
        }

        // Shutdown and end connection
        client.Close();
    }

    private static void SendToAll(object obj)
    {
        var data = (String)obj;

        for (int i = 0; i < clients.Count; i++)
        {
            var client = clients[i];

            NetworkStream stream = client.GetStream();

            byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);
            stream.Write(msg, 0, msg.Length);
        }
    }


    private static String receiveData(TcpClient client){
        // Buffer for reading data
        Byte[] bytes = new Byte[256];
        String data = null;

        data = null;

        // Get a stream object for reading and writing
        NetworkStream stream = client.GetStream();

        int i;

        // Loop to receive all the data sent by the client.
        while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
        {
            // Translate data bytes to a ASCII string.
            data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);

            Console.WriteLine("{0}", data);

            return data;

            /*
            // Process the data sent by the client.
            //data = data.ToUpper();
            data = "message was received by server";

            byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

            // Send back a response.
            stream.Write(msg, 0, msg.Length);
            Console.WriteLine("{0}", data);

            */


        }

        return null;
    }
}