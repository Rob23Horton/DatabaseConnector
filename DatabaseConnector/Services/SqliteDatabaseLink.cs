using DatabaseConnector.Models;
using System.Data.Common;
using System.Data.SQLite;


namespace DatabaseConnector.Services
{
	public class SqliteDatabaseLink : IDatabaseLink
	{
		private SQLiteConnection _connection;
		public SqliteDatabaseLink(string DatabaseString)
		{
			_connection = new SQLiteConnection(DatabaseString);
		}

		public string BuildCreateTableQuery(List<Property> Properties)
		{
			string query = "";

			//Stupid Dumb Dumb way of doing it but who tf cares (Not me XD)
			Properties = Properties.OrderByDescending(p => p.IsPrimaryKey).ToList();
			//Adds the primary keys to the query
			Properties.Where(p => p.IsPrimaryKey).ToList().ForEach(p => query = Properties.Last() == p ? $"{query} {p.Name} {p.Type} PRIMARY KEY" : $"{query} {p.Name} {p.Type} PRIMARY KEY,");
			//Adds the non primary keys to the query
			Properties.Where(p => !p.IsPrimaryKey).ToList().ForEach(p => query = Properties.Last() == p ? $"{query} {p.Name} {p.Type}" : $"{query} {p.Name} {p.Type},");

			return query;
		}

		public bool Connect()
		{
			try
			{
				_connection.Open();

				return true;
			}
			catch (Exception e)
			{
				Console.WriteLine($"Failed to connect to database at {DateTime.Now} due to {e.ToString()}");
				return false;
			}
		}

		public void Disconnect()
		{
			try
			{
				_connection.Close();
			}
			catch (Exception e)
			{
				Console.WriteLine($"Failed to close connection to database at {DateTime.Now} due to {e.ToString()}");
			}
		}

		public void Execute(string Query)
		{
			Connect();

			//Corrects boolean to string
			Query = Query.Replace(", b'", ", '");
			Query = Query.Replace("= b'", "= '");

			SQLiteCommand cmd = new SQLiteCommand(Query, _connection);
			cmd.ExecuteNonQuery();

			Disconnect();
		}

		public DbDataReader ReadExecute(string Query)
		{
			Connect();

			//Corrects boolean to string
			Query = Query.Replace(", b'", ", '");
			Query = Query.Replace("= b'", "= '");

			SQLiteCommand cmd = new SQLiteCommand(Query, _connection);
			DbDataReader dataReader = cmd.ExecuteReader();

			return dataReader;
		}
	}
}
