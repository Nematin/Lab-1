using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class Client
    {
        static Socket sendingSocket;
        static Socket listeningSocket;
        

        static void Main(string[] args)
        {
            
            try
            {
                Console.WriteLine("Client: " + Dns.GetHostName());

                sendingSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                Task sendTask = new Task(Send);
                sendTask.Start();

                Task recieveTask = new Task(Recieve);
                recieveTask.Start();

            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadLine();

        }

        private static void Send()
        {
            try
            {
                while (true)
                {

                    string hostname = Dns.GetHostName();
                    string message = "OK: " + hostname + "\n";

                    byte[] data = Encoding.Unicode.GetBytes(message);

                    EndPoint remotePoint = new IPEndPoint(IPAddress.Broadcast, 5000);

                    sendingSocket.EnableBroadcast = true;
                    sendingSocket.SendTo(data, remotePoint);
                    Console.WriteLine("Отправил!");
                    Thread.Sleep(4000);
                }

            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            finally
            {
                CloseSend();
            }

        }

        private static void Recieve() //Вся запара тут
        {
            IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 6000);

            try
            {
                //Прослушиваем по адресу
                listeningSocket.Bind(localEndPoint);

                while (true)
                {
                    // получаем сообщение
                    StringBuilder builder = new StringBuilder();
                    int bytes = 0; // количество полученных байтов
                    byte[] data = new byte[256]; // буфер для получаемых данных

                    //адрес, с которого пришли данные
                    EndPoint remoteIp = new IPEndPoint(IPAddress.Any, 0);
                    //EndPoint remoteIp = new IPEndPoint(IPAddress.Loopback, 0);

                    do
                    {
                        bytes = listeningSocket.ReceiveFrom(data, ref remoteIp);
                        builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
                    }
                    while (listeningSocket.Available > 0);
                    // получаем данные о подключении
                    IPEndPoint remoteFullIp = remoteIp as IPEndPoint;

                    // выводим сообщение
                    Console.WriteLine("В сети {0}:{1} - {2}", remoteFullIp.Address.ToString(), remoteFullIp.Port, builder.ToString());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                CloseRecieve();
            }
        }

        // закрытие сокета
        private static void CloseSend()
        {
            if (sendingSocket != null)
            {
                sendingSocket.Shutdown(SocketShutdown.Both);
                sendingSocket.Close();
                sendingSocket = null;
            }
        }

        private static void CloseRecieve()
        {
            if (listeningSocket != null)
            {
                listeningSocket.Shutdown(SocketShutdown.Both);
                listeningSocket.Close();
                listeningSocket = null;
            }
        }
    }
}

