using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using Xamarin.Forms;
using MySql.Data.MySqlClient;
using System.Net.Mail;
using System.Net;

namespace TEST
{
    public class RecoverAcc : ContentPage
    {
        Label textLabel;
        Entry mailEntry;
        StackLayout stackLayout;
        Button button;
        public RecoverAcc()
        {
            Title = "Восстановить доступ";
            stackLayout = new StackLayout();
            stackLayout.BackgroundColor = Color.White;


			mailEntry = new Entry
			{
				Placeholder = "Почта",
				Margin = new Thickness(10),
				TextColor = Color.Black,
				Text = "",
				FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label))
			};

			textLabel = new Label
			{
				Text = "",
				FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Label)),
				TextColor = Color.Black
			};

			button = new Button
            {
                Text = "Восстановить",
                FontSize = Device.GetNamedSize(NamedSize.Medium, typeof(Button)),
                BorderWidth = 1,
                HorizontalOptions = LayoutOptions.Center,
				VerticalOptions = LayoutOptions.EndAndExpand,
				Margin = new Thickness(20),
                TextColor = Color.White,
                BackgroundColor = Color.FromHex("#a6075b")

            };
            button.Clicked += OnButtonClicked;


            stackLayout.Children.Add(mailEntry);
            stackLayout.Children.Add(textLabel);
            stackLayout.Children.Add(button);
            stackLayout.Children.Add(textLabel);
            this.Content = stackLayout;

        }

        private async void OnButtonClicked(object sender, System.EventArgs e)
        {
            MySqlConnection conn = new MySqlConnection("server=127.0.0.1;port=3306;database=mydb;user id=root;password=admin;charset=utf8;Pooling=false;SslMode=None;");
            string log = "";
            string pas = "";
            try
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                    if (mailEntry.Text != "")
                    {
                        MailAddress from = new MailAddress("38_g@inbox.ru", "Восстановление данных");
                        MailAddress to = new MailAddress(mailEntry.Text);
                        MailMessage m = new MailMessage(from, to);
                        m.Subject = "Маркетинговое агентство";
                        if (log == "")
                        {
                            MySqlCommand cmd1 = new MySqlCommand("select Client_Id from Client where mail = @mail", conn);
                            cmd1.Parameters.AddWithValue("@mail", mailEntry.Text);
                            if (cmd1.ExecuteScalar() != null)
                            {
                                MySqlCommand cmd2 = new MySqlCommand("select login from Client where mail = @mail", conn);
                                cmd2.Parameters.AddWithValue("@mail", mailEntry.Text);

                                MySqlCommand cmd3 = new MySqlCommand("select password from Client where mail = @mail", conn);
                                cmd3.Parameters.AddWithValue("@mail", mailEntry.Text);

                                log = cmd2.ExecuteScalar().ToString();
                                pas = cmd3.ExecuteScalar().ToString();
                                m.Body = "<h1>Логин: " + log + "</h1>" + "<h1>Пароль: " + pas + "</h1>";
                                m.IsBodyHtml = true;
                                SmtpClient smtp = new SmtpClient("smtp.mail.ru", 587);
                                smtp.Credentials = new NetworkCredential("38_g@inbox.ru", "pPydF1U0ePfCmTXXTaum");
                                smtp.EnableSsl = true;
                                smtp.Send(m);
                                await DisplayAlert("Логин и пароль отправлены на почту", "", "OK");
                                await Navigation.PopAsync();
                            }
                        }
                        else if (log == "")
                        {
                            MySqlCommand cmd1 = new MySqlCommand("select Worker_Id from worker where mail = @mail", conn);
                            cmd1.Parameters.AddWithValue("@mail", mailEntry.Text);
                            if (cmd1.ExecuteScalar() != null)
                            {
                                MySqlCommand cmd2 = new MySqlCommand("select login from worker where mail = @mail", conn);
                                cmd2.Parameters.AddWithValue("@mail", mailEntry.Text);

                                MySqlCommand cmd3 = new MySqlCommand("select password from worker where mail = @mail", conn);
                                cmd3.Parameters.AddWithValue("@mail", mailEntry.Text);

                                log = cmd2.ExecuteScalar().ToString();
                                pas = cmd3.ExecuteScalar().ToString();
                                m.Body = "<h1>Логин: " + log + "</h1>" + "<h1>Пароль: " + pas + "</h1>";
                                m.IsBodyHtml = true;
                                SmtpClient smtp = new SmtpClient("smtp.mail.ru", 587);
                                smtp.Credentials = new NetworkCredential("38_g@inbox.ru", "pPydF1U0ePfCmTXXTaum");
                                smtp.EnableSsl = true;
                                smtp.Send(m);
                                await DisplayAlert("Логин и пароль отправлены на почту", "", "OK");
                                await Navigation.PopAsync();
                            }
                        }

                        if (log == "")
                        {
                            await DisplayAlert("Пользователя не существует", "", "OK");
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
                await DisplayAlert("Пользователя не существует", "", "OK");
            }
        }

    }
}
