using System;
using System.Net;
using System.Net.Sockets;

namespace Server
{
	/*Server side of the application. Accepts clients.*/

	class Program
	{
		static int port = 8005;
		static int count = 0;
		static void Main(string[] args)
		{
			IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);    // Getting address
			Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);	// Creating socket
			try
			{
				listenSocket.Bind(ipPoint);		// Binding socket
				listenSocket.Listen(50);		// Starting listening
				Console.WriteLine("Сервер запущен. Ожидание подключений...");
				ServerDataBase server_db = new ServerDataBase();
				while (true)
				{
					Socket socket = listenSocket.Accept();
					count++;
					Console.WriteLine("Client " + count + " connect");
					ClientThread cc = new ClientThread(socket, server_db);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

	}
}
