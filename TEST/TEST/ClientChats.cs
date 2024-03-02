using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace TEST
{
    public class ClientChats : ContentPage
    {
        int Id;
        MySqlConnection conn = new MySqlConnection("server=127.0.0.1;port=3306;database=mydb;user id=root;password=admin;charset=utf8;Pooling=false;SslMode=None;");
        Button ok;
        StackLayout stackLayout;
        ScrollView scroll;
        public ClientChats(int id)
        {
            Id = id;
            Title = "Сообщения";
            stackLayout = new StackLayout();
            stackLayout.BackgroundColor = Color.White;
            scroll = new ScrollView();
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
                MySqlCommand cmd1 = new MySqlCommand("select * from zakaz where client_id = @client_id  ORDER BY zakaz_id DESC", conn);
                cmd1.Parameters.AddWithValue("@client_id", Id);
                MySqlDataReader reader = cmd1.ExecuteReader();
                while (reader.Read())
                {
                    if (Convert.ToString(reader["status"]) != "отменен")
                    {
                        int usluga_id = Convert.ToInt32(reader["usluga_id"]);
                        MySqlConnection conn2 = new MySqlConnection("server=127.0.0.1;port=3306;database=mydb;user id=root;password=admin;charset=utf8;Pooling=false;SslMode=None;");
                        if (conn2.State == ConnectionState.Closed)
                        {
                            conn2.Open();
                            MySqlCommand cmd2 = new MySqlCommand("select name from Uslugi where usluga_id = @usluga_id", conn2);
                            cmd2.Parameters.AddWithValue("@usluga_id", usluga_id);
                            string name_usluga = cmd2.ExecuteScalar().ToString();

                            ok = new Button
                            {
                                Text = name_usluga,
                                FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Button)),
                                HorizontalOptions = LayoutOptions.Fill,
                                Margin = new Thickness(10),
                                BorderWidth = 1,
                                TextColor = Color.White,
                                BackgroundColor = Color.FromHex("#a6075b"),
                                ClassId = "But" + Convert.ToString(reader["zakaz_id"])
                            };
                            ok.Clicked += ok_Click;
                            stackLayout.Children.Add(ok);
                        }
                    }
                }
                reader.Close();
            }
            scroll.Content = stackLayout;
            this.Content = scroll;
        }
        private async void ok_Click(object sender, System.EventArgs e)
        {
            Button button = (Button)sender;
            string text = button.ClassId;
            text = text.Replace("But", "");
            int get = Convert.ToInt32(text);
            await Navigation.PushAsync(new ClientSms(get, Id));
        }
    }
}