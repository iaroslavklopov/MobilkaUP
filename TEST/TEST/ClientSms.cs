using MySqlConnector;
using System;
using System.Data;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TEST
{
	public class ClientSms : ContentPage
	{
		Label textLabel;
		Entry textEntry;
		RelativeLayout relativeLayout;
		StackLayout stackLayout1;
		Button button;
		ScrollView scroll;
		int ZakazId;
		int ClientId;

		public ClientSms(int zakazid, int clientid)
		{
			Title = "Чат";
			ZakazId = zakazid;
			ClientId = clientid;
			MySqlConnection conn = new MySqlConnection("server=127.0.0.1;port=3306;database=mydb;user id=root;password=admin;charset=utf8;Pooling=false;SslMode=None;");
			StackLayout stackLayout = new StackLayout { BackgroundColor = Color.White };

			relativeLayout = new RelativeLayout();

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
				MySqlCommand cmd = new MySqlCommand("select worker_id from zakaz where zakaz_id = @zakaz_id LIMIT 1", conn);
				cmd.Parameters.AddWithValue("@zakaz_id", ZakazId);
				int workerid = Convert.ToInt32(cmd.ExecuteScalar().ToString());

				MySqlCommand cmd2 = new MySqlCommand("select name from worker where worker_id = @worker_id", conn);
				cmd2.Parameters.AddWithValue("@worker_id", workerid);
				textLabel.Text = cmd2.ExecuteScalar().ToString();

				MySqlCommand cmd1 = new MySqlCommand("select * from sms where zakaz_id = @zakaz_id", conn);
				cmd1.Parameters.AddWithValue("@zakaz_id", ZakazId);
				MySqlDataReader reader = cmd1.ExecuteReader();
				while (reader.Read())
				{
					var bodyFrame = new Frame
					{
						Margin = new Thickness(20),
						Padding = new Thickness(10),
						BackgroundColor = Color.LightGray
					};

					var bodyLabel = new Label
					{
						Text = Convert.ToString(reader["sender"]) + "    |    " + Convert.ToString(reader["text"]),
						TextColor = Color.Black,
						FontSize = 16
					};

					bodyFrame.Content = bodyLabel;
					stackLayout1.Children.Add(bodyFrame);
				}
				reader.Close();
			}

			textEntry = new Entry
			{
				Text = "",
				Margin = new Thickness(10),
				TextColor = Color.Black,
				HorizontalOptions = LayoutOptions.FillAndExpand
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

			var grid = new Grid();
			grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

			grid.Children.Add(textEntry, 0, 0);
			grid.Children.Add(button, 1, 0);

			relativeLayout.Children.Add(
				stackLayout,
				Xamarin.Forms.Constraint.Constant(0),
				Xamarin.Forms.Constraint.Constant(0),
				Xamarin.Forms.Constraint.RelativeToParent((parent) => { return parent.Width; }),
				Xamarin.Forms.Constraint.RelativeToParent((parent) => { return parent.Height - 60; })
			);

			relativeLayout.Children.Add(
				grid,
				Xamarin.Forms.Constraint.Constant(0),
				Xamarin.Forms.Constraint.RelativeToParent((parent) => { return parent.Height - 60; }),
				Xamarin.Forms.Constraint.RelativeToParent((parent) => { return parent.Width; })
			);

			scroll.Content = stackLayout1;
			stackLayout.Children.Add(textLabel);
			stackLayout.Children.Add(scroll);
			this.Content = relativeLayout;

			Device.BeginInvokeOnMainThread(async () =>
			{
				await Task.Delay(100);
				await scroll.ScrollToAsync(0, scroll.Content.Height, true);
			});
		}

		private async void OnButtonClicked(object sender, System.EventArgs e)
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

							MySqlCommand cmd2 = new MySqlCommand("select name from client where client_id = @client_id", conn);
							cmd2.Parameters.AddWithValue("@client_id", ClientId);
							string nameclient = cmd2.ExecuteScalar().ToString();

							MySqlCommand cmd = new MySqlCommand("insert into sms (text, sender, zakaz_id) values (@text, @sender, @zakaz_id);", conn);
							cmd.Parameters.AddWithValue("@text", textEntry.Text);
							cmd.Parameters.AddWithValue("@sender", nameclient);
							cmd.Parameters.AddWithValue("@zakaz_id", ZakazId);
							cmd.ExecuteNonQuery();

							var bodyFrame = new Frame
							{
								Margin = new Thickness(20),
								Padding = new Thickness(10),
								BackgroundColor = Color.LightGray
							};

							var bodyLabel = new Label
							{
								Text = nameclient + "    |    " + textEntry.Text,
								TextColor = Color.Black,
								FontSize = 16
							};

							bodyFrame.Content = bodyLabel;
							stackLayout1.Children.Add(bodyFrame);

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
