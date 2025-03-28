using System.Data.Common;
using DatabaseConnector.Models;

namespace DatabaseConnector.Services
{
	internal interface IDatabaseLink
	{
		public bool Connect();
		public DbDataReader ReadExecute(string Query);
		public void Execute(string Query);
		public void Disconnect();
		public string BuildCreateTableQuery(List<Property> Properties);
	}
}
