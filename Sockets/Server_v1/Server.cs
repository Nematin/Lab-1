using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Server_v1
{
    class Server
    {


        static void Main(string[] args)
        {
            Console.Write("Введите IP: ");
            var localIP = IPAddress.Parse(Console.ReadLine());
            
            int serverPort = 5000;

            IPEndPoint localEndPoint = new IPEndPoint(localIP, serverPort);
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.EnableBroadcast = true;

            try
            {
                StringBuilder builder = new StringBuilder();

                socket.Bind(localEndPoint);

                EndPoint remoteEndPoint = new IPEndPoint(IPAddress.Any, 0);
                EndPoint remoteEndPointClient = new IPEndPoint(IPAddress.Broadcast, 6000);

                string hostname = "Server: " + Dns.GetHostName();
                byte[] _data = Encoding.Unicode.GetBytes(hostname);

                Console.WriteLine("Ready to receive!");
                Console.WriteLine("IP: " + localEndPoint);
                byte[] data = new byte[1024];
                int i = 0;

                while (true)
                {

                    do
                    {
                        int recv = socket.ReceiveFrom(data, ref remoteEndPoint);
                        string stringData = Encoding.Unicode.GetString(data, 0, recv);
                        Console.WriteLine("[" + i + "]" + "Received: {0} from: {1}", stringData, remoteEndPoint.ToString());
                        i++;
                    } while (socket.Available > 0);

                    socket.SendTo(_data, remoteEndPointClient);

                }

            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            finally
            {
                Console.ReadKey();
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            

           

        }
    }
}
