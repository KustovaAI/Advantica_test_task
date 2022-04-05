using System;
using System.Collections.Generic;
using Npgsql;

namespace Server
{
	class ServerDataBase
	{
		/* This class directly accesses the database */

		NpgsqlConnection nc;
		String conn_param = "Server=127.0.0.1;Port=5433;User Id=postgres;Password=rootroot;Database=advantica";
		List<MyIObserver> all_o = new List<MyIObserver>();

		public ServerDataBase()
		{
			nc = new NpgsqlConnection(conn_param);
		}

		public void AddO (MyIObserver o)
		{
			all_o.Add(o);
		}

		public String GetTable()
		{
			String result = "";
			try
			{
				NpgsqlCommand com = new NpgsqlCommand("SELECT * FROM employees", nc);
				nc.Open();	//ОpenConnection
				NpgsqlDataReader reader;
				reader = com.ExecuteReader();
				while (reader.Read())
				{
					try
					{
						String id = reader.GetInt64(0).ToString();
						String surname = reader.GetString(1);
						String name = reader.GetString(2);
						String patronymic = reader.GetString(3);
						String day_of_birthday = reader.GetDate(4).ToString();
						String sex = reader.GetString(5);
						String having_children = reader.GetString(6);
						String line = id + "," + surname + "," + name + "," + patronymic + "," + day_of_birthday + "," + sex + "," + having_children + ";";
						result = result + line;
					}
					catch (Exception ex)
					{
						Console.WriteLine(ex.Message);
					}

				}
				nc.Close();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return "error";
			}
			result = result.Substring(0, result.Length - 1);
			return result;
		}

		public String RequestToDB(String sql1)
		{
			try
			{
				nc.Open();
				NpgsqlCommand dbcmd = nc.CreateCommand();
				try
				{
					dbcmd.CommandText = sql1;
					dbcmd.ExecuteNonQuery();
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
					return ex.Message.ToString();
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return ex.Message.ToString();
			}
			nc.Close();
			return "ok";
		}

		public String AddNewWorker(String str)
		{
			String[] list_params = str.Split(',');
			String surname = list_params[0];
			String name = list_params[1];
			String patronymic = list_params[2];
			String date_of_birthday = list_params[3];
			String sex = list_params[4];
			String having_children = list_params[5];
			string sql1 = "INSERT INTO employees(surname,name,patronymic,day_of_birth,sex,having_children) VALUES ('" +
				surname + "','" + name + "','" + patronymic + "','" + date_of_birthday + "','" + sex + "','" + having_children + "')";
			String res = RequestToDB(sql1);
			if (res == "ok")
			{
				foreach (MyIObserver o in all_o)
				{
					o.UpdateTable();
				}
				return res;
			} else return res;
		}

		public String UpdateWorker(String str)
		{
			String[] list_params = str.Split(',');
			String id = list_params[0];
			String surname = list_params[1];
			String name = list_params[2];
			String patronymic = list_params[3];
			String date_of_birthday = list_params[4];
			String sex = list_params[5];
			String having_children = list_params[6];
			string sql1 = "UPDATE employees SET id = '" + id + "',surname = '" + surname + "',name = '" + name + "',patronymic = '" +
				patronymic + "',day_of_birth = '" + date_of_birthday + "',sex = '" + sex + "',having_children = '" + having_children + "' WHERE id = " + id;
			String res = RequestToDB(sql1);
			if (res == "ok")
			{
				foreach (MyIObserver o in all_o)
				{
					o.UpdateTable();
				}
				return res;
			}
			else return res;
		}

		public String DeleteWorker(String id)
		{
			string sql1 = "DELETE FROM employees WHERE id = '" + id + "'";
			String res = RequestToDB(sql1);
			if (res == "ok")
			{
				foreach (MyIObserver o in all_o)
				{
					o.UpdateTable();
				}
				return res;
			}
			else return res;
		}

		public String GetIndexes()
		{
			String result = "";
			try
			{
				NpgsqlCommand com = new NpgsqlCommand("SELECT id FROM employees", nc);
				nc.Open();
				NpgsqlDataReader reader;
				reader = com.ExecuteReader();
				while (reader.Read())
				{
					try
					{
						String id = reader.GetInt64(0).ToString();
						result = result + id + ",";
					}
					catch (Exception ex)
					{ Console.WriteLine(ex.Message); }

				}
				nc.Close();
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
				return "error";
			}
			result = result.Substring(0, result.Length - 1);
			return result;
		}


	}
}
