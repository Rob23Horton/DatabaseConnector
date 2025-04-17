using DatabaseConnector.Models;
using Microsoft.Data.SqlClient;
using MySqlConnector;
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

		public string BuildCreateTableQuery(List<Property> Properties)
		{
			string query = "";

			//Stupid Dumb Dumb way of doing it but who tf cares (Not me XD)
			Properties = Properties.OrderByDescending(p => p.IsPrimaryKey).ToList();
			//Adds the primary keys to the query
			Properties.Where(p => p.IsPrimaryKey).ToList().ForEach(p => query = Properties.Last() == p ? $"{query} {p.Name} {p.Type} IDENTITY(1,1) PRIMARY KEY" : $"{query} {p.Name} {p.Type} IDENTITY(1,1) PRIMARY KEY,");
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

		public string EscapeString(string Value)
		{
			return MySqlHelper.EscapeString(Value);
		}
	}
}
