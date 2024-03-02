using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace TEST
{
    public class NewZakaz : ContentPage
    {
        int Id;
        Label textLabel;
        Entry textEntry;
        StackLayout stackLayout;
        Button button;
        Picker listBox1;
        MySqlConnection conn = new MySqlConnection("server=127.0.0.1;port=3306;database=mydb;user id=root;password=admin;charset=utf8;Pooling=false;SslMode=None;");
        public NewZakaz(int id)
        {
            Id = id;
            Title = "Новый заказ";
            stackLayout = new StackLayout();
            stackLayout.BackgroundColor = Color.White;

            textLabel = new Label
            {
                Text = "Услуга",
                FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
                TextColor = Color.Black,
                Margin = new Thickness(10)
            };
            listBox1 = new Picker
            {
                Margin = new Thickness(10)

            };
            textEntry = new Entry
            {
                Placeholder = "Пожелания",
                Margin = new Thickness(10),
                TextColor = Color.Black
            };

            button = new Button
            {
                Text = "Создать",
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Button)),
                BorderWidth = 1,
                HorizontalOptions = LayoutOptions.Center,
                Margin = new Thickness(20),
                TextColor = Color.White,
                BackgroundColor = Color.FromHex("#a6075b"),

            };
            button.Clicked += OnButtonClicked;

            MySqlCommand cmd1 = new MySqlCommand("select * from uslugi", conn);
            MySqlDataReader reader;
            try
            {
                cmd1.Connection.Open();
                reader = cmd1.ExecuteReader();
                while (reader.Read())
                {
                    string x = Convert.ToString(reader["name"]) + "-" + Convert.ToString(reader["price"]);
                    listBox1.Items.Add(x);
                }
                reader.Close();
            }
            catch (MySqlException ex)
            {
                Console.WriteLine("Error: \r\n{0}", ex.ToString());
            }
            finally
            {
                cmd1.Connection.Close();
            }



            stackLayout.Children.Add(textLabel);
            stackLayout.Children.Add(listBox1);
            stackLayout.Children.Add(textEntry);
            stackLayout.Children.Add(button);
            this.Content = stackLayout;
        }
        private async void OnButtonClicked(object sender, System.EventArgs e)
        {
            if (textEntry.Text != "" && listBox1.SelectedItem.ToString() != "")
            {
                string usluga = listBox1.SelectedItem.ToString();
                string[] words = usluga.Split(new char[] { '-' });
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                    MySqlCommand cmd1 = new MySqlCommand("select Usluga_id from Uslugi where name = @name", conn);
                    cmd1.Parameters.AddWithValue("@name", words[0]);
                    int usluga_id = Convert.ToInt32(cmd1.ExecuteScalar().ToString());

                    MySqlCommand cmd2 = new MySqlCommand("select otdel from Uslugi where name = @name", conn);
                    cmd2.Parameters.AddWithValue("@name", words[0]);
                    string otdel = cmd2.ExecuteScalar().ToString();

                    string x = "";
                    MySqlCommand cmd3 = new MySqlCommand("select * from worker where otdel = @otdel", conn);
                    cmd3.Parameters.AddWithValue("@otdel", otdel);
                    MySqlDataReader reader = cmd3.ExecuteReader();
                    while (reader.Read())
                    {
                        x = x + " " + Convert.ToString(reader["worker_id"]);
                    }
                    reader.Close();
                    x = x.Substring(1);
                    string[] ids = x.Split(new char[] { ' ' });
                    int worker_id = Convert.ToInt32(ids[new Random().Next(0, ids.Length)]);

                    MySqlCommand cmd = new MySqlCommand("insert into zakaz(wishes, client_id, worker_id, usluga_id, status) values (@wishes, @client_id, @worker_id, @usluga_id, @status);", conn);
                    cmd.Parameters.AddWithValue("@wishes", textEntry.Text);
                    cmd.Parameters.AddWithValue("@client_id", Id);
                    cmd.Parameters.AddWithValue("@worker_id", worker_id);
                    cmd.Parameters.AddWithValue("@usluga_id", usluga_id);
                    cmd.Parameters.AddWithValue("@status", "создан");
                    cmd.ExecuteNonQuery();
                    await DisplayAlert("Заказ создан! Теперь вы можете вести диалог с исполнителем.", "", "OK");
                    await Navigation.PushModalAsync(new NavigationPage(new ClientPage(Id)));
                }
            }
            else
            {
                await DisplayAlert("Пустые поля", "Заполните все поля", "OK");
            }


        }
    }
    
}