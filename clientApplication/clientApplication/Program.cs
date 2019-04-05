using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace clientApplication
{
    class client
    {
        static Int32 port = 13000;
        static String server = "127.0.0.1";

        static TcpClient myClient;

        static NetworkStream stream;

        static string username = "bob";



        public static void Main()
        {
            String message = null;

            Console.WriteLine("enter server IP: (you should put 127.0.0.1)");
            server = Console.ReadLine();
            Console.WriteLine("enter your name: (this one you can choose)");
            username = Console.ReadLine();

            Connect(server,username + " is now connected",username);

            //new thread to listen to incoming messages --> ReceiveMsg
            Thread receive = new Thread(ReceiveMsg);
            receive.Start();

            //New thread countdown
            Thread KA = new Thread(client.KeepAlive);
            KA.Start();

            do
            {
                message = Console.ReadLine();
                if (!message.Equals("exit"))
                {
                    SendMsg(username + ":" + " " + message);
                    //restart afk thread
                    KA.Abort();
                    KA = new Thread(client.KeepAlive);
                    KA.Start();
                }
            } while (!message.Equals("exit"));

            Disconnect();
        }

        static void Connect(String server, String message, String username)
        {
            try
            {
                Console.WriteLine(server);
                myClient = new TcpClient(server, port);

                Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

                stream = myClient.GetStream();

                // Send the message to the connected TcpServer. 
                stream.Write(data, 0, data.Length);

                Console.WriteLine("{0}: {1}", username, message);

                // Receive the TcpServer.response.

                // Buffer to store the response bytes.
                data = new Byte[256];

                // String to store the response ASCII representation.
                String responseData = String.Empty;

                // Read the first batch of the TcpServer response bytes.
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);

                //Console.WriteLine("Received: {0}", responseData);
                Console.WriteLine("{0}", responseData);
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
        }

        static void SendMsg(string message)
        {
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);
            stream.Write(data, 0, data.Length);
            data = new Byte[256];
        }

        static void ReceiveMsg()
        {
            // Buffer for reading data
            Byte[] bytes = new Byte[256];
            String data = String.Empty;
            int i;

            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                // Translate data bytes to a ASCII string.
                data = System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                if(!data.Contains(username))
                    Console.WriteLine("{0}", data);
                data = String.Empty;
            }
        }

        static void Disconnect()
        {
            // Close everything.
            SendMsg(username + " is now disconnected");
            stream.Close();
            myClient.Close();
            Console.WriteLine("Disconnected From server!");
        }

        static void KeepAlive() {
            Thread.Sleep(60000);
            Console.WriteLine("you're AFK, bye!");
            Disconnect();
            server = Console.ReadLine();

        }
    }
}
