using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Win32;
using MySqlConnector;
using System.Data;
using TEST;
using Xamarin.Forms;
using Xamarin.Forms.PlatformConfiguration;

namespace TestProject
{
	[TestClass]
	public class UnitTest1
	{

		[TestMethod]
		public void datab()
		{
			bool expected = true;
			var af = new Methods();
			bool actual = af.db();
			Assert.AreEqual(expected, actual);
		}


		[TestMethod]
		public void Autoriz()
		{
			bool expected = true;
			var af = new Methods();
			bool actual = af.auth("almira", "123");
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void Registr()
		{
			bool expected = true; ;
			var af = new Methods();
			bool actual = af.Reg("георгий", "89050005432", "anon2@mail.ru", "anon2", "123");
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void smsfromworker()
		{
			bool expected = true; ;
			var af = new Methods();
			bool actual = af.sendsms("Добрый день! Чем я могу вам помочь?", 1, 1);
			Assert.AreEqual(expected, actual);
		}

		[TestMethod]
		public void cancelorder()
		{
			bool expected = true; ;
			var af = new Methods();
			bool actual = af.cancelorder(5);
			Assert.AreEqual(expected, actual);
		}

	}
}