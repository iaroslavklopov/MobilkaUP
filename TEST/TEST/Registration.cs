using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using MySql.Data.MySqlClient;

namespace TEST
{
    public class Registration : ContentPage
    {
        Label textLabel, textLabel2;
        Entry loginEntry, passwordEntry, passwordEntry2, nameEntry, mailEntry, numberEntry;
        StackLayout stackLayout;
        Button button;
        public Registration()
        {
            Title = "Регистрация";
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

			nameEntry = new Entry
            {
                Placeholder = "Имя",
				Style = entryStyle

			};
            numberEntry = new Entry
            {
                Placeholder = "Номер телефона",
				Style = entryStyle
			};

            mailEntry = new Entry
            {
                Placeholder = "Почта",
				Style = entryStyle
			};


            loginEntry = new Entry
            {
                Placeholder = "Логин",
				Style = entryStyle
			};

            passwordEntry = new Entry
            {
                Placeholder = "Пароль",
                IsPassword = true,
				Style = entryStyle

			};

            passwordEntry2 = new Entry
            {
                Placeholder = "Пароль",
                IsPassword = true,
				Style = entryStyle
			};

            textLabel = new Label
            {
                Text = "",
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                TextColor = Color.Black
            };
            passwordEntry.TextChanged += passwordEntry_TextChanged;
            passwordEntry2.TextChanged += passwordEntry_TextChanged;

			Button button = new Button
			{
				Text = "Регистрация",
				FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Button)),
				BorderWidth = 1,
				HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.EndAndExpand, 
				Margin = new Thickness(20),
				TextColor = Color.White,
				BackgroundColor = Color.FromHex("#a6075b")
			};

			button.Clicked += OnButtonClicked;


			textLabel2 = new Label
            {
                Text = "",
                FontSize = Device.GetNamedSize(NamedSize.Large, typeof(Label)),
                HorizontalOptions = LayoutOptions.Center,
                TextColor = Color.Black
            };

            stackLayout.Children.Add(nameEntry);
            stackLayout.Children.Add(numberEntry);
            stackLayout.Children.Add(mailEntry);
            stackLayout.Children.Add(loginEntry);
            stackLayout.Children.Add(passwordEntry);
            stackLayout.Children.Add(passwordEntry2);
            stackLayout.Children.Add(textLabel);
            stackLayout.Children.Add(button);
            stackLayout.Children.Add(textLabel2);
            this.Content = stackLayout;

        }
        void passwordEntry_TextChanged(object sender, EventArgs e)
        {
            if (passwordEntry.Text != passwordEntry2.Text) { textLabel.Text = "Пароли не совпадают"; }
            else { textLabel.Text = ""; }
        }

        private async void OnButtonClicked(object sender, System.EventArgs e)
        {
            MySqlConnection conn = new MySqlConnection("server=127.0.0.1;port=3306;database=mydb;user id=root;password=admin;charset=utf8;Pooling=false;SslMode=None;");
            string name = "";
            try
            {

                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                    if (nameEntry.Text != "" && numberEntry.Text != "" && mailEntry.Text != "" && loginEntry.Text != "" && passwordEntry.Text != "")
                    {
                        MySqlCommand cmd1 = new MySqlCommand("select Client_Id from Client where login = @login or mail = @mail", conn);
                        cmd1.Parameters.AddWithValue("@login", loginEntry.Text);
                        cmd1.Parameters.AddWithValue("@mail", mailEntry.Text);
                        if (cmd1.ExecuteScalar() != null) { name = cmd1.ExecuteScalar().ToString(); }

                        MySqlCommand cmd2 = new MySqlCommand("select Worker_Id from Worker where login = @login or mail = @mail", conn);
                        cmd2.Parameters.AddWithValue("@login", loginEntry.Text);
                        cmd2.Parameters.AddWithValue("@mail", mailEntry.Text);
                        if (cmd2.ExecuteScalar() != null) { name = cmd2.ExecuteScalar().ToString(); }

                        MySqlCommand cmd3 = new MySqlCommand("select Adminis_Id from Adminis where login = @login", conn);
                        cmd3.Parameters.AddWithValue("@login", loginEntry.Text);
                        if (cmd3.ExecuteScalar() != null) { name = cmd3.ExecuteScalar().ToString(); }


                        if (name == "")
                        {
                            MySqlCommand cmd = new MySqlCommand("insert into client (name, number, mail, login, password) values(@name, @number, @mail, @login, @password);", conn);
                            cmd.Parameters.AddWithValue("@name", nameEntry.Text);
                            cmd.Parameters.AddWithValue("@number", numberEntry.Text);
                            cmd.Parameters.AddWithValue("@mail", mailEntry.Text);
                            cmd.Parameters.AddWithValue("@login", loginEntry.Text);
                            cmd.Parameters.AddWithValue("@password", passwordEntry.Text);
                            cmd.ExecuteNonQuery();
                            await DisplayAlert("Успешная регистрация", " ", "OK");
                            await Navigation.PopAsync();
                        }
                        else
                        {
                            await DisplayAlert("Пользователь с таким логином уже существует", " ", "OK");
                        }
                    }
                    else { await DisplayAlert("Пустые поля", "Заполните все поля", "OK"); }
                }


            }
            catch (MySqlException ex)
            {
                await DisplayAlert("Сообщение об ошибке", ex.ToString(), "OK");
            }

        }
		public bool Reg(string Name, string Number, string Mail, string Login, string Password)
		{
			using (MySqlConnection conn = new MySqlConnection("server=127.0.0.1;port=3306;database=mydb;user id=root;password=admin;charset=utf8;Pooling=false;SslMode=None;"))
			{
				try
				{
					conn.Open();

					MySqlCommand cmd1 = new MySqlCommand("SELECT Client_Id FROM Client WHERE login = @login OR mail = @mail", conn);
					cmd1.Parameters.AddWithValue("@login", Login);
					cmd1.Parameters.AddWithValue("@mail", Mail);
					if (cmd1.ExecuteScalar() != null)
					{
						return false;
					}

					MySqlCommand cmd2 = new MySqlCommand("SELECT Worker_Id FROM Worker WHERE login = @login OR mail = @mail", conn);
					cmd2.Parameters.AddWithValue("@login", Login);
					cmd2.Parameters.AddWithValue("@mail", Mail);
					if (cmd2.ExecuteScalar() != null)
					{
						return false;
					}

					MySqlCommand cmd3 = new MySqlCommand("SELECT Adminis_Id FROM Adminis WHERE login = @login", conn);
					cmd3.Parameters.AddWithValue("@login", Login);
					if (cmd3.ExecuteScalar() != null)
					{
						return false;
					}

					MySqlCommand cmd4 = new MySqlCommand("SELECT Manager_Id FROM Manager WHERE login = @login OR mail = @mail", conn);
					cmd4.Parameters.AddWithValue("@login", Login);
					cmd4.Parameters.AddWithValue("@mail", Mail);
					if (cmd4.ExecuteScalar() != null)
					{
						return false;
					}

					MySqlCommand cmd = new MySqlCommand("INSERT INTO client (name, number, mail, login, password) VALUES (@name, @number, @mail, @login, @password);", conn);
					cmd.Parameters.AddWithValue("@name", Name);
					cmd.Parameters.AddWithValue("@number", Number);
					cmd.Parameters.AddWithValue("@mail", Mail);
					cmd.Parameters.AddWithValue("@login", Login);
					cmd.Parameters.AddWithValue("@password", Password);
					cmd.ExecuteNonQuery();
					return true;
				}
				catch (Exception ex)
				{
					return false;
				}
			}
		}
	}
}