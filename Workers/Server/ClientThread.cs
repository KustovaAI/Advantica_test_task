using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace Server
{
	class ClientThread : MyIObserver
	{
		/* For each connected client, a separate thread and an instance of this class are created.
		  This class accepts requests from clients, searches for relevant information, and sends responses to the client.*/

		Socket cs;
		ServerDataBase s_db;
		byte[] ins; // Input stream
		int ins_bytes; // Length of input stream
		StringBuilder builder;
		byte[] outs = new byte[256]; // Output stream

		Thread thread;

		public ClientThread(Socket _cs, ServerDataBase _server_db)
		{
			this.cs = _cs;
			this.s_db = _server_db;
			s_db.AddO(this);
			thread = new Thread(Run);
			thread.Start();
		}

		public void Run()
		{
			ins = new byte[256];
			ins_bytes = 0;
			// Listening client
			while (true)
			{
				builder = new StringBuilder();
				do
				{
					ins_bytes = cs.Receive(ins);
					builder.Append(Encoding.Unicode.GetString(ins, 0, ins_bytes));
				}
				while (cs.Available > 0);
				ParseRequests(builder.ToString());
			}
		}

		public void ParseRequests(String request)
		{
			// The request comes in the following format:
			// request name|[request body]
			Console.WriteLine(DateTime.Now.ToShortTimeString() + ": " + request);
			String[] str = request.Split('|');
			switch (str[0])
			{ 
				case "get_table":			
					String table = s_db.GetTable();
					String res;
					if (table == "error")
						res = "error|Error getting table from database!";
					else
						res = "table|" + table;
					SendRequest(res);
					break;
				case "set_line":
					String res2 = s_db.AddNewWorker(str[1]);
					if (res2 != "ok") {
						SendRequest("error|" + res2);
					}
					break;
				case "update_line":
					String res3 = s_db.UpdateWorker(str[1]);
					if (res3 != "ok")
					{
						SendRequest("error|" + res3);
					}
					break;
				case "delete_line":
					String res4 = s_db.DeleteWorker(str[1]);
					if (res4 != "ok")
					{
						SendRequest("error|" + res4);
					}
					break;
				case "get_indexes":
					String indexes = s_db.GetIndexes();
					if (indexes == "error")
					{
						SendRequest("error|Error getting indexes from database!");
					} else
					{
						String res5 = "array_id|" + indexes;
						SendRequest(res5);
					}
					break;
				default:
					Console.WriteLine("Received an unexpected response from the client!");
					break;
			}
		}

		public void SendRequest(String str)
		{
			outs = Encoding.Unicode.GetBytes(str);
			cs.Send(outs);
		}

		public void UpdateTable()
		{
			String table = s_db.GetTable();
			String res = "table|" + table;
			// The response comes out the following format:
			// table|[Row1Col1,Row1Col2...Row1Coln;Row2Col1,Row2Col2,...,Row2Coln,...,RownColn]
			SendRequest(res);
		}

	}
}
