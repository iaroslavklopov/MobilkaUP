using Xamarin.Forms;
using System.Data;
using MySqlConnector;
using System;

namespace TEST
{
    public partial class MainPage : ContentPage
    {
        Entry loginEntry, passwordEntry;
        StackLayout stackLayout, stackLayout2;
        Button button, button2, button3;
        public MainPage()
        {
            MySqlConnection conn = new MySqlConnection("server=127.0.0.1;port=3306;database=mydb;user id=root;password=admin;charset=utf8;Pooling=false;SslMode=None;");
            stackLayout = new StackLayout();
            stackLayout.BackgroundColor = Color.White;



			var entryStyle = new Style(typeof(Entry))
			{
				Setters = {
		new Setter { Property = Entry.PlaceholderColorProperty, Value = Color.Gray },
		new Setter { Property = Entry.TextColorProperty, Value = Color.Black },
		new Setter { Property = Entry.BackgroundColorProperty, Value = Color.FromHex("#ECECEC") },
		new Setter { Property = Entry.FontSizeProperty, Value = Device.GetNamedSize(NamedSize.Small, typeof(Entry)) },
		new Setter { Property = Entry.MarginProperty, Value = new Thickness(10) },
		new Setter { Property = Entry.HeightRequestProperty, Value = 40 },
		new Setter { Property = Entry.HorizontalOptionsProperty, Value = LayoutOptions.FillAndExpand },
	}
			};

			loginEntry = new Entry
			{
				Placeholder = "Логин",
				Text = "",
				Style = entryStyle
			};

			passwordEntry = new Entry
			{
				Placeholder = "Пароль",
				Text = "",
				IsPassword = true,
				Style = entryStyle
			};



			var buttonStyle = new Style(typeof(Button))
			{
				Setters = {
		new Setter { Property = Button.TextColorProperty, Value = Color.White },
		new Setter { Property = Button.BackgroundColorProperty, Value = Color.FromHex("#a6075b") },
		new Setter { Property = Button.FontSizeProperty, Value = Device.GetNamedSize(NamedSize.Micro, typeof(Button)) },
		new Setter { Property = Button.BorderWidthProperty, Value = 1 },
		new Setter { Property = Button.BorderColorProperty, Value = Color.FromHex("#a6075b") },
		new Setter { Property = Button.CornerRadiusProperty, Value = 5 },
		new Setter { Property = Button.HorizontalOptionsProperty, Value = LayoutOptions.Center },
		new Setter { Property = Button.MarginProperty, Value = new Thickness(10) },
		new Setter { Property = VisualElement.HeightRequestProperty, Value = 40 },
		new Setter { Property = VisualElement.WidthRequestProperty, Value = 150 },
		new Setter { Property = Button.FontFamilyProperty, Value = "Arial" },
		new Setter { Property = Button.FontAttributesProperty, Value = FontAttributes.Bold },
	}
			};

			button = new Button
			{
				Text = "Регистрация",
				Style = buttonStyle,
				WidthRequest = 150,
				ClassId = "button1"
			};
			button.Clicked += OnButtonClicked;

			button2 = new Button
			{
				Text = "Вход",
				Style = buttonStyle,
				WidthRequest = 150,
				ClassId = "button2"
			};
			button2.Clicked += OnButton2Clicked;

			button3 = new Button
			{
				Text = "Забыли пароль?",
				Style = buttonStyle,
				WidthRequest = 150,
				ClassId = "button3"
			};
			button3.Clicked += OnButton3Clicked;

			var grid = new Grid
			{
				ColumnDefinitions =
					{
						new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
						new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
						new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
					}
			};

			grid.Children.Add(button2, 0, 0);
			grid.Children.Add(button, 1, 0);
			grid.Children.Add(button3, 2, 0);

            stackLayout.Children.Add(loginEntry);
            stackLayout.Children.Add(passwordEntry);
			stackLayout.Children.Add(grid);
			this.Content = stackLayout;
        }
        private async void OnButtonClicked(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new Registration());
        }
        private async void OnButton2Clicked(object sender, System.EventArgs e)
        {
            MySqlConnection conn = new MySqlConnection("server=127.0.0.1;port=3306;database=mydb;user id=root;password=admin;charset=utf8;Pooling=false;SslMode=None;");
            string name = "";
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                    if (loginEntry.Text != "" && passwordEntry.Text != "")
                    {
                        if (name == "")
                        {
                            MySqlCommand cmd1 = new MySqlCommand("select Client_Id from Client where login = @login", conn);
                            cmd1.Parameters.AddWithValue("@login", loginEntry.Text);
                            if (cmd1.ExecuteScalar() != null)
                            {
                                name = cmd1.ExecuteScalar().ToString();
                                await Navigation.PushModalAsync(new NavigationPage(new ClientPage(Convert.ToInt32(name))));
                            }
                        }
                        if (name == "")
                        {
                            MySqlCommand cmd2 = new MySqlCommand("select Worker_Id from Worker where login = @login", conn);
                            cmd2.Parameters.AddWithValue("@login", loginEntry.Text);
                            if (cmd2.ExecuteScalar() != null)
                            {
                                name = cmd2.ExecuteScalar().ToString();
                                await Navigation.PushModalAsync(new NavigationPage(new WorkerPage(Convert.ToInt32(name))));
							}
                        }

                        if (name == "")
                        {
                            await DisplayAlert("Пользователя не существует", "Зарегистрируйтесь по кнопке", "OK");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Пустые поля", "Заполните все поля", "OK");
                    }
                }

            }
            catch (MySqlException ex)
            {
                await DisplayAlert("Сообщение об ошибке", ex.ToString(), "OK");
            }

        }
        private async void OnButton3Clicked(object sender, System.EventArgs e)
        {

            await Navigation.PushAsync(new RecoverAcc());

        }
		public bool db()
		{
			MySqlConnection conn = new MySqlConnection("server=127.0.0.1;port=3306;database=mydb;user id=root;password=admin;charset=utf8;Pooling=false;SslMode=None;");
			if (conn.State == ConnectionState.Closed)
			{
				conn.Open();
				return true;
			}
			else { return false; }
		}
		public bool auth(string Login, string Password)
		{
			MySqlConnection conn = new MySqlConnection("server=127.0.0.1;port=3306;database=mydb;user id=root;password=admin;charset=utf8;Pooling=false;SslMode=None;");
			if (conn.State == ConnectionState.Closed)
			{
				conn.Open();
				MySqlCommand cmd1 = new MySqlCommand("select Client_Id from Client where login = @login and password = @password", conn);
				cmd1.Parameters.AddWithValue("@login", Login);
				cmd1.Parameters.AddWithValue("@password", Password);
				if (cmd1.ExecuteScalar() != null)
				{
					return true;

				}
				else
				{
					MySqlCommand cmd2 = new MySqlCommand("select Worker_Id from Worker where login = @login and password = @password", conn);
					cmd2.Parameters.AddWithValue("@login", Login);
					cmd2.Parameters.AddWithValue("@password", Password);
					if (cmd2.ExecuteScalar() != null)
					{
						return true;
					}
					else
					{
						MySqlCommand cmd3 = new MySqlCommand("select Adminis_Id from Adminis where login = @login and password = @password", conn);
						cmd3.Parameters.AddWithValue("@login", Login);
						cmd3.Parameters.AddWithValue("@password", Password);
						if (cmd3.ExecuteScalar() != null)
						{
							return true;
						}
						else
						{
							MySqlCommand cmd4 = new MySqlCommand("select Manager_Id from Manager where login = @login and password = @password", conn);
							cmd4.Parameters.AddWithValue("@login", Login);
							cmd4.Parameters.AddWithValue("@password", Password);
							if (cmd4.ExecuteScalar() != null)
							{
								return true;
							}
							else
							{
								return false;
							}
						}
					}
				}
			}
			else { return false; }
		}
	}
}
