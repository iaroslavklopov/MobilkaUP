using MySqlConnector;
using System;
using System.Data;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TEST
{
	public class WorkerSms : ContentPage
	{
		Label textLabel;
		Entry textEntry;
		StackLayout stackLayout, stackLayout1;
		Button button;
		ScrollView scroll;
		Entry body;
		int ZakazId;
		int WorkerId;

		public WorkerSms(int zakazid, int workerid)
		{
			Title = "Чат";
			ZakazId = zakazid;
			WorkerId = workerid;
			MySqlConnection conn = new MySqlConnection("server=127.0.0.1;port=3306;database=mydb;user id=root;password=admin;charset=utf8;Pooling=false;SslMode=None;");
			stackLayout = new StackLayout { BackgroundColor = Color.White };

			textLabel = new Label
			{
				Text = "",
				FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
				TextColor = Color.Black,
				HorizontalOptions = LayoutOptions.Center
			};

			scroll = new ScrollView();
			stackLayout1 = new StackLayout();

			if (conn.State == ConnectionState.Closed)
			{
				conn.Open();
				MySqlCommand cmd = new MySqlCommand("select client_id from zakaz where zakaz_id = @zakaz_id LIMIT 1", conn);
				cmd.Parameters.AddWithValue("@zakaz_id", ZakazId);
				int clientid = Convert.ToInt32(cmd.ExecuteScalar().ToString());

				MySqlCommand cmd2 = new MySqlCommand("select name from client where client_id = @client_id", conn);
				cmd2.Parameters.AddWithValue("@client_id", clientid);
				textLabel.Text = cmd2.ExecuteScalar().ToString();

				MySqlCommand cmd1 = new MySqlCommand("select * from sms where zakaz_id = @zakaz_id", conn);
				cmd1.Parameters.AddWithValue("@zakaz_id", ZakazId);
				MySqlDataReader reader = cmd1.ExecuteReader();
				while (reader.Read())
				{
					body = new Entry
					{
						Text = Convert.ToString(reader["sender"]) + "    |    " + Convert.ToString(reader["text"]),
						Margin = new Thickness(20),
						TextColor = Color.Black,
						HeightRequest = 60,
						FontSize = 16,
						IsEnabled = false
					};
					stackLayout1.Children.Add(body);
				}
				reader.Close();
			}

			textEntry = new Entry
			{
				Text = "",
				Margin = new Thickness(10),
				TextColor = Color.Black
			};

			button = new Button
			{
				Text = " ↑ ",
				FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Button)),
				BorderWidth = 1,
				HorizontalOptions = LayoutOptions.End,
				Margin = new Thickness(10),
				TextColor = Color.White,
				BackgroundColor = Color.FromHex("#a6075b")
			};
			button.Clicked += OnButtonClicked;

			var entryLayout = new StackLayout
			{
				Orientation = StackOrientation.Horizontal,
				VerticalOptions = LayoutOptions.Center,
				Children = { textEntry, button }
			};

			scroll.Content = stackLayout1;
			stackLayout.Children.Add(textLabel);
			stackLayout.Children.Add(scroll);
			stackLayout.Children.Add(entryLayout);
			this.Content = stackLayout;

			Device.BeginInvokeOnMainThread(async () =>
			{
				await Task.Delay(100);
				await scroll.ScrollToAsync(0, scroll.Content.Height, true);
			});

			SizeChanged += OnPageSizeChanged;
		}

		private void OnPageSizeChanged(object sender, EventArgs e)
		{
			textEntry.WidthRequest = stackLayout.Width - (2 * textEntry.Margin.HorizontalThickness) - button.Width - (2 * button.Margin.HorizontalThickness);
		}

		private async void OnButtonClicked(object sender, EventArgs e)
		{
			try
			{
				if (textEntry.Text != "")
				{
					if (textEntry.Text.Length < 150)
					{
						MySqlConnection conn = new MySqlConnection("server=127.0.0.1;port=3306;database=mydb;user id=root;password=admin;charset=utf8;Pooling=false;SslMode=None;");
						if (conn.State == ConnectionState.Closed)
						{
							conn.Open();

							MySqlCommand cmd2 = new MySqlCommand("select name from worker where worker_id = @worker_id", conn);
							cmd2.Parameters.AddWithValue("@worker_id", WorkerId);
							string nameclient = cmd2.ExecuteScalar().ToString();

							MySqlCommand cmd = new MySqlCommand("insert into sms (text, sender, zakaz_id) values (@text, @sender, @zakaz_id);", conn);
							cmd.Parameters.AddWithValue("@text", textEntry.Text);
							cmd.Parameters.AddWithValue("@sender", nameclient);
							cmd.Parameters.AddWithValue("@zakaz_id", ZakazId);
							cmd.ExecuteNonQuery();

							body = new Entry
							{
								Text = nameclient + "    |    " + textEntry.Text,
								Margin = new Thickness(20),
								TextColor = Color.Black,
								HeightRequest = 60,
								FontSize = 16,
								IsEnabled = false
							};
							stackLayout1.Children.Add(body);
							textEntry.Text = "";

							Device.BeginInvokeOnMainThread(async () =>
							{
								await Task.Delay(100);
								await scroll.ScrollToAsync(0, scroll.Content.Height, true);
							});
						}
					}
					else { await DisplayAlert("Сообщение длинное, ограничение - 150 символов", "", "OK"); }
				}
			}
			catch (Exception ex)
			{
				await DisplayAlert("Сообщение об ошибке", ex.ToString(), "OK");
			}
		}
	}
}
