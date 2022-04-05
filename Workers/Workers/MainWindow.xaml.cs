using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace Workers
{

	/*Main client module. Requests information from the server, waits for responses from the server,
	  and displays the information using a graphical interface. */

	public partial class MainWindow : Window
	{
		Socket socket;
		DataGrid DataGridCamiao;


		public MainWindow()
		{
			InitializeComponent();
			ConnectToServer("127.0.0.1", 8005);
		}

		public void ConnectToServer(String host, int port)
		{
			try
			{
				IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse(host), port);	// Getting address
				socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);	// Creating socket
				socket.Connect(ipPoint);	// Connection to server	
				byte[] in_data = new byte[256]; // Input stream				
				int bytes = 0; // Length of input stream
				// Listening server
				Thread t_listen = new Thread(() => {
					while (true)
					{
						StringBuilder builder = new StringBuilder();
						do
						{
							bytes = socket.Receive(in_data);
							builder.Append(Encoding.Unicode.GetString(in_data, 0, bytes));
						}
						while (socket.Available > 0);
						ParseResponses(builder.ToString());
					}
				});
				t_listen.Start();
				byte[] out_data = Encoding.Unicode.GetBytes("get_table");	// Output stream
				socket.Send(out_data);
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message);
			}

		}

		public void ParseResponses(String str)
		{
			String[] splited = str.Split('|');
			switch(splited[0])
			{
				case "table":
					// The response comes in the following format:
					// table|[Row1Col1,Row1Col2...Row1Coln;Row2Col1,Row2Col2,...,Row2Coln,...,RownColn]
					ResetTable(splited[1]);
					break;
				case "error":
					MessageBox.Show(splited[1]);
					break;
				default:
					MessageBox.Show("Received an unexpected response from the server!");
					break;
			}
		}

		public void ResetTable(String str)
		{
			String[] lines = str.Split(';');
			List<Worker> result = new List<Worker>(lines.Count());
			foreach (String line in lines)
			{
				if (line != "")
				{
					String[] t = line.Split(',');
					result.Add(new Worker(t[0], t[1], t[2], t[3], t[4], t[5], t[6]));
				}
			}
			Application.Current.Dispatcher.BeginInvoke(new Action(() =>
			{
				data_grid_1.ItemsSource = result;
			}));
		}

		private void Button_1_Click(object sender, RoutedEventArgs e)
		{
			//Clear textBoxes
			button_5.Visibility = Visibility.Hidden;
			button_4.Visibility = Visibility.Visible;
			canvas_2.Visibility = Visibility.Visible;
			textBox_surname.Text = "";
			textBox_surname.Text = "";
			textBox_name.Text = "";
			textBox_patronymic.Text = "";
			datePicker_1.Text = "";
			checkBox_1.IsChecked = false;
			rb_1.IsChecked = true;
		}

		private void Button_4_Click(object sender, RoutedEventArgs e)
		{
			String s = ReadDataFromScreen(false);
			if (s != "error")
			{
				String res = "set_line|" + s;
				byte[] in_data = Encoding.Unicode.GetBytes(res);
				socket.Send(in_data);
				canvas_2.Visibility = Visibility.Hidden;
				button_4.Visibility = Visibility.Hidden;
				button_5.Visibility = Visibility.Hidden;
			}
		}

		private void Button_2_Click(object sender, RoutedEventArgs e)
		{
			button_4.Visibility = Visibility.Hidden;
			button_5.Visibility = Visibility.Visible;
			canvas_2.Visibility = Visibility.Visible;
		}

		private void Data_grid_1_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			DataGridCamiao = sender as DataGrid;
			if (DataGridCamiao.SelectedItem != null)
			{
				var item = DataGridCamiao.SelectedItem as Worker;
				if (item != null)
				{
					textBox_surname.Text = item.surname;
					textBox_name.Text = item.name;
					textBox_patronymic.Text = item.patronymic;
					datePicker_1.Text = item.date_of_birthday;
					if (item.sex == "м")
						rb_1.IsChecked = true;
					else
						rb_2.IsChecked = true;
					if (item.having_children == "да")
						checkBox_1.IsChecked = true;
					else
						checkBox_1.IsChecked = false;
				}
			}
		}

		public String ReadDataFromScreen(bool is_need_id)
		{
			String line = "";
			if (is_need_id)
			{
				if (DataGridCamiao == null || DataGridCamiao.SelectedItem == null)
				{
					MessageBox.Show("Choose an employee!");
					return "error";
				}
				var item = DataGridCamiao.SelectedItem as Worker;
				line = item.id + ",";
			}
			String surname = textBox_surname.Text;
			String name = textBox_name.Text;
			String patronymic = textBox_patronymic.Text;
			String data_of_birthday = datePicker_1.Text;
			String sex = "";
			if (rb_1.IsChecked == true)
				sex = "м";
			else if (rb_2.IsChecked == true)
				sex = "ж";
			String having_children = "";
			if (checkBox_1.IsChecked == true)
				having_children = "да";
			else having_children = "нет";
			line = line + surname + "," + name + "," + patronymic + "," + data_of_birthday + "," + sex + "," + having_children;
			return line;
		}

		private void Button_5_Click(object sender, RoutedEventArgs e)
		{
			String s = ReadDataFromScreen(true);
			if (s != "error")
			{
				String res = "update_line|" + s;
				byte[] in_data = Encoding.Unicode.GetBytes(res);
				socket.Send(in_data);
				canvas_2.Visibility = Visibility.Hidden;
				button_5.Visibility = Visibility.Visible;
			}
		}

		private void Button_3_Click(object sender, RoutedEventArgs e)
		{
			if (DataGridCamiao != null)
			{
				var item = DataGridCamiao.SelectedItem as Worker;
				if (item != null)
				{
					String res = "delete_line|" + item.id;
					byte[] data1 = Encoding.Unicode.GetBytes(res);
					socket.Send(data1);

				}
			}
		}
	}
}
