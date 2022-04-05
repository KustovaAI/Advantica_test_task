using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace OffSystemDataEntry
{
	class Program
	{
		/* This class performs off-system data entry. A random delay is generated (from 5 to 15 seconds)
		   - through this interval, operations are performed with the database. Each operation is randomly generated.
		   Also, worker data is randomly generated when adding or updating, and worker id is randomly generated when deleting. */

		static String host = "127.0.0.1";
		static int port = 8005;
		static Socket socket;
		static void Main(string[] args)
		{
			int delay = 0;
			Random rand = new Random((int)DateTime.Now.Ticks);
			delay = rand.Next(5, 15);	// Generate random delay at the first time
			try
			{
				IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(host), port);   // Getting address
				socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);   // Creating socket
				socket.Connect(ipPoint);    // Connection to server	
				byte[] out_data;	// Output stream
				while (true)
				{
					Thread.Sleep(delay * 1000);
					String operation = DoRandomOperation();
					if (operation != "")
					{
						out_data = Encoding.Unicode.GetBytes(DoRandomOperation());  // Chose the operation: add new worker, modify worker, delete worker
						socket.Send(out_data);  // Send request
					}
					delay = rand.Next(5, 15);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		static public String DoRandomOperation()
		{
			String res;
			Random rand = new Random((int)DateTime.Now.Ticks);
			int c1 = rand.Next(1, 100);
			if (c1 % 3 == 0)
			{
				//Modify worker
				String id = TookIndexesFromServer();
				if (id == "")
					return "";
				else
					res = "update_line|" + id + "," + GetGeneratedRequest();				
			} else if (c1 % 3 == 1)
			{
				//Delete worker
				String id = TookIndexesFromServer();
				if (id == "")
					return "";
				else
					res = "delete_line|" + id;
			} else
			{
				//Add new worker
				res = "set_line|" + GetGeneratedRequest();
			}
			return res;
		}

		static public String TookIndexesFromServer()	// Getting array of indexes
		{
			String id = "";
			byte[] in_data = Encoding.Unicode.GetBytes("get_indexes");	// Input stream
			socket.Send(in_data);
			byte[] os = new byte[256]; // Output stream
			int bytes = 0;	// Length of output stream
			while (true)
			{
				StringBuilder builder = new StringBuilder();
				do
				{
					bytes = socket.Receive(os);
					builder.Append(Encoding.Unicode.GetString(os, 0, bytes));
				}
				while (socket.Available > 0);
				id = GetRandomIndex(builder.ToString());
				break;
			}
			return id;
		}

		static public String GetGeneratedRequest()
		{
			Random rand = new Random((int)DateTime.Now.Ticks);
			int c1 = rand.Next(1, 100);
			string surname = GenerateString(rand);
			string name = GenerateString(rand);
			string patronymic = GenerateString(rand);
			string day_of_birth = GenerateDate(rand);
			int c2 = rand.Next(100, 200);
			string sex = "";
			if (c1 % 2 == 0)
				sex = "м";
			else
				sex = "ж";
			string having_children = "";
			if (c2 % 2 == 0)
				having_children = "да";
			else
				having_children = "нет";
			String s = surname + "," + name + "," + patronymic + "," + day_of_birth + "," + sex + "," + having_children;
			return s;
		}

		static public String GenerateString(Random rand)
		{
			int n = rand.Next(5, 10);
			string s = "";
			for (int i = 0; i < n; i++)
			{
				s += (char)(rand.Next(1040, 1104));
			}
			return s;
		}
		static public String GenerateDate(Random rand)
		{
			int day = rand.Next(1, 28);
			int month = rand.Next(1, 12);
			int year = rand.Next(1950, 2010);
			string s = day.ToString() + "." + month.ToString() + "." + year.ToString();
			return s;
		}

		static public string GetRandomIndex(String str) {
			String[] tmp = str.Split('|');
			if (tmp[0] == "array_id")
			{
				String[] indexes = tmp[1].Split(',');
				Random rand = new Random((int)DateTime.Now.Ticks);
				int n = rand.Next(0, indexes.Count() - 1);
				return indexes[n];
			}
			else if (tmp[0] == "error")
			{
				Console.WriteLine(tmp[1]);
				return "";
			}
			else
			{
				Console.WriteLine("Received an unexpected response from the server!");
				return "";
			}
		}

	}
}
