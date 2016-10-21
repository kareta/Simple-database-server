using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DatabaseServer
{
    public class DatabaseServer
    {
        public static string Data { get; private set; }
        public static QueryParser parser = new QueryParser();

        public static void StartListening() {
            var ipHostInfo = Dns.Resolve(Dns.GetHostName());
            var ipAddress = ipHostInfo.AddressList[0];
            Console.WriteLine(ipAddress);
            var localEndPoint = new IPEndPoint(ipAddress, 11000);

            var listener = new Socket(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp );

            try {
                listener.Bind(localEndPoint);
                listener.Listen(10);

                while (true) {
                    Console.WriteLine("Waiting for a connection...");

                    var handler = listener.Accept();
                    Data = null;


                    while (true) {
                        var bytes = new byte[10];
                        var bytesRec = handler.Receive(bytes);
                        Data += Encoding.UTF8.GetString(bytes,0,bytesRec);
                        if (Data.IndexOf("\r\n\r\n") > -1) {
                            break;
                        }
                    }
                    var clientPacket = new ClientPacket(Data.Trim());
                    var query = clientPacket.Query;

                    var resultData = parser.ParseQuery(query);
                    var serverPacket = new ServerPacket(resultData);
                    Console.WriteLine( "Text received : {0}", Data);
                    Console.WriteLine("Result data : {0}", serverPacket);

                    byte[] packetBytes = Encoding.ASCII.GetBytes(serverPacket.ToString());

                    handler.Send(packetBytes);
                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();
                }

            } catch (Exception e) {
                Console.WriteLine(e.ToString());
            }

            Console.WriteLine("\nPress ENTER to continue...");
            Console.Read();

        }

        public static int Main() {
            StartListening();

            return 0;
        }
    }
}
