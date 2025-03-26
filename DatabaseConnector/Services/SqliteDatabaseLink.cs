using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseConnector.Services
{
	public class SqliteDatabaseLink : IDatabaseLink
	{
		private SqliteConnection _connection;
		public SqliteDatabaseLink(string DatabaseString)
		{
			_connection = new SqliteConnection(DatabaseString);
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

			SqliteCommand cmd = new SqliteCommand(Query, _connection);
			cmd.ExecuteNonQuery();

			Disconnect();
		}

		public DbDataReader ReadExecute(string Query)
		{
			Connect();

			SqliteCommand cmd = new SqliteCommand(Query, _connection);
			DbDataReader dataReader = cmd.ExecuteReader();

			return dataReader;
		}
	}
}
