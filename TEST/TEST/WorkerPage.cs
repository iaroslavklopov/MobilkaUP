using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace TEST
{
    public class WorkerPage : ContentPage
    {
        int Id;
        Label textLabel1, textLabel2;
        StackLayout stackLayout, stackLayout2, stackLayout3, stackLayout4;
        Button button, button2, button3, ok;
        ScrollView scroll;
        Frame border;
        public WorkerPage(int id)
        {
            Id = id;
            Title = "Личный кабинет";
            stackLayout = new StackLayout();
            stackLayout.BackgroundColor = Color.White;
            scroll = new ScrollView();
            stackLayout3 = new StackLayout();
            MySqlConnection conn = new MySqlConnection("server=127.0.0.1;port=3306;database=mydb;user id=root;password=admin;charset=utf8;Pooling=false;SslMode=None;");
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                    MySqlCommand cmd1 = new MySqlCommand("select * from zakaz where worker_id = @worker_id ORDER BY zakaz_id DESC", conn);
                    cmd1.Parameters.AddWithValue("@worker_id", Id);
                    MySqlDataReader reader = cmd1.ExecuteReader();
                    while (reader.Read())
                    {
                        int usluga_id = Convert.ToInt32(reader["usluga_id"]);
                        MySqlConnection conn2 = new MySqlConnection("server=127.0.0.1;port=3306;database=mydb;user id=root;password=admin;charset=utf8;Pooling=false;SslMode=None;");
                        if (conn2.State == ConnectionState.Closed)
                        {
                            conn2.Open();
                            MySqlCommand cmd2 = new MySqlCommand("select name from Uslugi where usluga_id = @usluga_id", conn2);
                            cmd2.Parameters.AddWithValue("@usluga_id", usluga_id);
                            string name_usluga = cmd2.ExecuteScalar().ToString();

                            border = new Frame()
                            {
                                BorderColor = Color.Black,
                                CornerRadius = 10
                            };
                            stackLayout4 = new StackLayout()
                            {
                                Orientation = StackOrientation.Horizontal
                            };

                            textLabel1 = new Label
                            {
                                Text = name_usluga,
                                FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                                HorizontalOptions = LayoutOptions.StartAndExpand,
                                WidthRequest = 200,
                                TextColor = Color.Black
                            };
                            textLabel2 = new Label
                            {
                                Text = Convert.ToString(reader["status"]),
                                FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                                HorizontalOptions = LayoutOptions.CenterAndExpand,
                                TextColor = Color.Black
                            };
                            stackLayout4.Children.Add(textLabel1);
                            stackLayout4.Children.Add(textLabel2);

							if (Convert.ToString(reader["status"]) == "в работе")
							{
								ok = new Button
								{
									Text = "завершено",
									FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Button)),
									HorizontalOptions = LayoutOptions.EndAndExpand,
									BorderWidth = 1,
									TextColor = Color.White,
									BackgroundColor = Color.FromHex("#a6075b"),
									ClassId = "But" + Convert.ToString(reader["zakaz_id"])
								};
								ok.Clicked += ok_Click;
								stackLayout4.Children.Add(ok);
							}
							if (Convert.ToString(reader["status"]) == "создан")
                            {
                                ok = new Button
                                {
                                    Text = "начать заказ",
                                    FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Button)),
                                    HorizontalOptions = LayoutOptions.EndAndExpand,
                                    BorderWidth = 1,
                                    TextColor = Color.White,
                                    BackgroundColor = Color.FromHex("#a6075b"),
                                    ClassId = "But" + Convert.ToString(reader["zakaz_id"])
                                };
                                ok.Clicked += ok_Click2;
                                stackLayout4.Children.Add(ok);
                            }
							

							border.Content = stackLayout4;
                            stackLayout3.Children.Add(border);
                        }
                    }
                    reader.Close();
                }


                scroll.Content = stackLayout3;
                stackLayout.Children.Add(scroll);


                stackLayout2 = new StackLayout()
                {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.CenterAndExpand,
                    VerticalOptions = LayoutOptions.EndAndExpand
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
		new Setter { Property = Button.MarginProperty, Value = new Thickness(20) },
		new Setter { Property = VisualElement.HeightRequestProperty, Value = 40 },
		new Setter { Property = VisualElement.WidthRequestProperty, Value = 150 },
		new Setter { Property = Button.FontFamilyProperty, Value = "Arial" },
		new Setter { Property = Button.FontAttributesProperty, Value = FontAttributes.Bold },
	}
				};

				button2 = new Button
                {
                    Text = "Сообщения",
					Style = buttonStyle
				};

                button2.Clicked += OnButton2Clicked;

                button3 = new Button
                {
                    Text = "Выход",
					Style = buttonStyle
				};

                button3.Clicked += OnButton3Clicked;

                stackLayout2.Children.Add(button2);
                stackLayout2.Children.Add(button3);
                stackLayout.Children.Add(stackLayout2);
                this.Content = stackLayout;
            }
            catch (MySqlException ex)
            {
                DisplayAlert("Сообщение об ошибке", ex.ToString(), "OK");
            }

        }

        private async void OnButton2Clicked(object sender, System.EventArgs e)
        {
            await Navigation.PushAsync(new WorkerChats(Id));
        }
        private async void OnButton3Clicked(object sender, System.EventArgs e)
        {
            await Navigation.PushModalAsync(new NavigationPage(new MainPage()));
        }

        private async void ok_Click(object sender, System.EventArgs e)
        {
            MySqlConnection conn = new MySqlConnection("server=127.0.0.1;port=3306;database=mydb;user id=root;password=admin;charset=utf8;Pooling=false;SslMode=None;");
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
                Button button = (Button)sender;
                string text = button.ClassId;
                text = text.Replace("But", "");
                int get = Convert.ToInt32(text);

                MySqlCommand cmd = new MySqlCommand("UPDATE zakaz SET status = @status where zakaz_id = @zakaz_id;", conn);
                cmd.Parameters.AddWithValue("@status", "завершено");
                cmd.Parameters.AddWithValue("@zakaz_id", get);
                cmd.ExecuteNonQuery();
                await Navigation.PushModalAsync(new NavigationPage(new WorkerPage(Id)));

            }
        }
        private async void ok_Click2(object sender, System.EventArgs e)
        {
            MySqlConnection conn = new MySqlConnection("server=127.0.0.1;port=3306;database=mydb;user id=root;password=admin;charset=utf8;Pooling=false;SslMode=None;");
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
                Button button = (Button)sender;
                string text = button.ClassId;
                text = text.Replace("But", "");
                int get = Convert.ToInt32(text);

                MySqlCommand cmd = new MySqlCommand("UPDATE zakaz SET status = @status where zakaz_id = @zakaz_id;", conn);
                cmd.Parameters.AddWithValue("@status", "в работе");
                cmd.Parameters.AddWithValue("@zakaz_id", get);
                cmd.ExecuteNonQuery();
                await Navigation.PushModalAsync(new NavigationPage(new WorkerPage(Id)));

            }
        }
    }
}