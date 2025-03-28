using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseConnector.Models;
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

		public string BuildCreateTableQuery(List<Property> Properties)
		{
			string query = "";

			//Stupid Dumb Dumb way of doing it but who tf cares (Not me XD)
			Properties = Properties.OrderByDescending(p => p.IsPrimaryKey).ToList();
			//Adds the primary keys to the query
			Properties.Where(p => p.IsPrimaryKey).ToList().ForEach(p => query = Properties.Last() == p ? $"{query} {p.Name} {p.Type} PRIMARY KEY AUTO_INCREMENT" : $"{query} {p.Name} {p.Type} PRIMARY KEY AUTO_INCREMENT,");
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
