using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DatabaseConnector.Services
{
	public class SqlServerDatabaseLink : IDatabaseLink
	{
		private SqlConnection _connection;
		public SqlServerDatabaseLink(string DatabaseString)
		{
			_connection = new SqlConnection(DatabaseString);
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

			SqlCommand cmd = new SqlCommand(Query, _connection);
			cmd.ExecuteNonQuery();

			Disconnect();
		}

		public DbDataReader ReadExecute(string Query)
		{
			Connect();

			SqlCommand cmd = new SqlCommand(Query, _connection);
			DbDataReader dataReader = cmd.ExecuteReader();

			return dataReader;
		}
	}
}
