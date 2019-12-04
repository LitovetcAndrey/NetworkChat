using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleServer
{
    class Program
    {
        static List<TcpClient> cliens = new List<TcpClient>();
        const int PORT = 55_555;


        static void Main(string[] args)
        {
            TcpClient client;
            TcpListener server = null;

            Console.Title = "Server";
            try
            {
                server = new TcpListener(IPAddress.Any, PORT);
                server.Start();
                Console.WriteLine("Сервер запущен");
                while (true)
                {
                    //прослушивание сокета
                    client = server.AcceptTcpClient();
                    Console.WriteLine("Подключился client  " + client.Client.RemoteEndPoint.ToString());
                    cliens.Add(client);

                    Listen(client);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                if (server.Server.Connected)
                {
                    cliens.ForEach(s =>
                    {
                        s.Client.Shutdown(SocketShutdown.Both);
                        s.Close();
                    });

                    server.Server.Close();
                    server.Stop();
                }
                Console.WriteLine("Server close!");
            }

            Console.Read();
        }


        private async static void Listen(TcpClient client)
        {
            await Task.Run(() =>
            {
                while (true)
                {
                    try
                    {
                        byte[] buff = new byte[256];
                        int count = client.Client.Receive(buff);
                        string sender = client.Client.RemoteEndPoint.ToString();
                        Console.WriteLine("Получено сообщение от " + sender);


                        //отправка сообщений
                        cliens.ForEach(c =>
                    {
                        string recerver = c.Client.RemoteEndPoint.ToString();

                        if (!sender.Equals(recerver))
                        {
                            c.Client.Send(buff, 0, count, SocketFlags.None);
                        }

                    });

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                    }
                }

            });
        }

    }
}

