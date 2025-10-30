using DataBox.Services;

namespace DatabaseConnectorTests;

[TestClass]
public class UnitTest1
{
	[TestMethod]
	public void TestMethod1()
	{
		using var db1 = 
			Database.UseSqlite()
				.WithFile("path.db")
				.Connect();

		var db2 =
			Database.UseMySql()
				.ForServer("server")
				.ForDatabase("db")
				.AsIntegratedSecurity()
				.Connect();
	}
}
