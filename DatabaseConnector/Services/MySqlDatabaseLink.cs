using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;

namespace DatabaseConnector.Services
{
	public class MySqlDatabaseLink : IDatabaseLink
	{
		private MySqlConnection _connection;
		public MySqlDatabaseLink(string DatabaseString)
		{
			this._connection = new MySqlConnection(DatabaseString);
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

			MySqlCommand cmd = new MySqlCommand(Query, this._connection);
			cmd.ExecuteNonQuery();

			Disconnect();
		}

		public DbDataReader ReadExecute(string Query)
		{
			Connect();

			MySqlCommand cmd = new MySqlCommand(Query, this._connection);
			MySqlDataReader dataReader = cmd.ExecuteReader();

			return dataReader;
		}
	}
}
