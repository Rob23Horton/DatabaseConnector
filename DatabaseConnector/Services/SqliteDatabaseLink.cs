using DatabaseConnector.Models;
using Microsoft.Data.Sqlite;
using System.Data.Common;


namespace DatabaseConnector.Services
{
	public class SqliteDatabaseLink : IDatabaseLink
	{
		private SqliteConnection _connection;
		public SqliteDatabaseLink(string DatabaseString)
		{
			_connection = new SqliteConnection(DatabaseString);
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
			catch
			{
				Console.WriteLine($"Failed to connect to database at {DateTime.Now}");
				return false;
			}
		}

		public void Disconnect()
		{
			try
			{
				_connection.Close();
			}
			catch
			{
				Console.WriteLine($"Failed to close connection to database at {DateTime.Now}");
			}
		}

		public void Execute(string Query)
		{
			Connect();

			//Corrects boolean to string
			Query = Query.Replace(", b'", ", '");
			Query = Query.Replace("= b'", "= '");

			SqliteCommand cmd = new SqliteCommand(Query, _connection);
			cmd.ExecuteNonQuery();

			Disconnect();
		}

		public DbDataReader ReadExecute(string Query)
		{
			Connect();

			//Corrects boolean to string
			Query = Query.Replace(", b'", ", '");
			Query = Query.Replace("= b'", "= '");

			SqliteCommand cmd = new SqliteCommand(Query, _connection);
			DbDataReader dataReader = cmd.ExecuteReader();

			return dataReader;
		}
	}
}
