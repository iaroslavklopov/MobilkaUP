using MySqlConnector;
using System.Data;

namespace TestProject
{
	public class Methods
	{
		public bool db()
		{
			MySqlConnection conn = new MySqlConnection("server=127.0.0.1;port=3306;database=mydb;user id=root;password=1234;charset=utf8;Pooling=false;SslMode=None;");
			if (conn.State == ConnectionState.Closed)
			{
				conn.Open();
				return true;
			}
			else { return false; }
		}
		public bool auth(string Login, string Password)
		{
			MySqlConnection conn = new MySqlConnection("server=127.0.0.1;port=3306;database=mydb;user id=root;password=1234;charset=utf8;Pooling=false;SslMode=None;");
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
		public bool Reg(string Name, string Number, string Mail, string Login, string Password)
		{
			using (MySqlConnection conn = new MySqlConnection("server=127.0.0.1;port=3306;database=mydb;user id=root;password=1234;charset=utf8;Pooling=false;SslMode=None;"))
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
		public bool sendsms(string text, int idwork, int idzakaz)
		{
			try
			{
				if (text.Length < 150)
				{
					MySqlConnection conn = new MySqlConnection("server=127.0.0.1;port=3306;database=mydb;user id=root;password=1234;charset=utf8;Pooling=false;SslMode=None;");
					if (conn.State == ConnectionState.Closed)
					{
						conn.Open();

						MySqlCommand cmd2 = new MySqlCommand("select name from worker where worker_id = @worker_id", conn);
						cmd2.Parameters.AddWithValue("@worker_id", idwork);
						string nameclient = cmd2.ExecuteScalar().ToString();

						MySqlCommand cmd = new MySqlCommand("insert into sms (text, sender, zakaz_id) values (@text, @sender, @zakaz_id);", conn);
						cmd.Parameters.AddWithValue("@text", text);
						cmd.Parameters.AddWithValue("@sender", nameclient);
						cmd.Parameters.AddWithValue("@zakaz_id", idzakaz);
						cmd.ExecuteNonQuery();
						return true;
					}
					else { return false; }
				}
				else { return false; }
			}
			catch (Exception ex)
			{
				return false;
			}
		}

		public bool cancelorder(int idzakaz)
		{
			MySqlConnection conn = new MySqlConnection("server=127.0.0.1;port=3306;database=mydb;user id=root;password=1234;charset=utf8;Pooling=false;SslMode=None;");

			if (conn.State == ConnectionState.Closed)
			{
				conn.Open();
				try
				{
					MySqlCommand cmd = new MySqlCommand("UPDATE zakaz SET status = @status where zakaz_id = @zakaz_id;", conn);
					cmd.Parameters.AddWithValue("@status", "отменен");
					cmd.Parameters.AddWithValue("@zakaz_id", idzakaz);
					cmd.ExecuteNonQuery();
					return true;
				}
				catch
				{
					return false;
				}
			}
			else { return false; }
		}
	}
}
