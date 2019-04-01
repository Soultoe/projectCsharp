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

        static TcpClient myClient = new TcpClient(server, port);

        static NetworkStream stream;

        static string username = "bob";



        public static void Main()
        {
            Connect("127.0.0.1","hello! I'm a client!",username);
            SendMsg("HELLO THERE");
            Disconnect();
            //Connect("127.0.0.1", "hello! I'm a client 2!");
            //Connect("127.0.0.1", "hello! I'm a client 3!");

	        //New thread countdown
            //Thread KA = new Thread(client.KeepAlive);
            //KA.Start();
        }

        static void Connect(String server, String message, String username)
        {
            try
            {
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
            stream = myClient.GetStream();
            Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);

            stream.Write(data, 0, data.Length);

            data = new Byte[256];
            
            // String to store the response ASCII representation.
            String responseData = String.Empty;

            // Read the first batch of the TcpServer response bytes.
            Int32 bytes = stream.Read(data, 0, data.Length);
            responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
            //Console.WriteLine("Received: {0}", responseData);
            Console.WriteLine("{0}", responseData);

        }

        static void Disconnect()
        {
            // Close everything.
            stream.Close();
            myClient.Close();
            Console.WriteLine("Disconnected From server!");
        }

        static void KeepAlive() {
            Thread.Sleep(5000);
            Console.WriteLine("you're AFK, bye!");
            Disconnect();
        }
    }
}
