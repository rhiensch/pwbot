using System.Globalization;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bot;

namespace BotTests
{
	/// <summary>
	/// Summary description for AttackAdviserTests
	/// </summary>
	[TestClass]
	public class AttackAdviserTests
	{
		public AttackAdviserTests()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext { get; set; }

		#region Additional test attributes
		//
		// You can use the following additional attributes as you write your tests:
		//
		// Use ClassInitialize to run code before running the first test in the class
		// [ClassInitialize()]
		// public static void MyClassInitialize(TestContext testContext) { }
		//
		// Use ClassCleanup to run code after all tests in a class have run
		// [ClassCleanup()]
		// public static void MyClassCleanup() { }
		//
		// Use TestInitialize to run code before running each test 
		[TestInitialize()]
		public void MyTestInitialize()
		{
			CultureInfo myCulture = new CultureInfo("en-US");
			Thread.CurrentThread.CurrentCulture = myCulture;

			attackerAdviser = new AttackAdviser();
		}
		//
		// Use TestCleanup to run code after each test has run
		// [TestCleanup()]
		// public void MyTestCleanup() { }
		//
		#endregion

		private AttackAdviser attackerAdviser;

		[TestMethod]
		public void TestMethod1()
		{
			//
			// TODO: Add test logic	here
			//
		}
	}
}
