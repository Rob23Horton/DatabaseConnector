using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseConnector.Services
{
	internal interface IDatabaseLink
	{
		public bool Connect();
		public DbDataReader ReadExecute(string Query);
		public void Execute(string Query);
		public void Disconnect();
	}
}
