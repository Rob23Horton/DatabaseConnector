using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DatabaseConnector.Models;

namespace DatabaseConnector.Services
{
	public interface IDatabaseConnector
	{
		public void CreateTable<T>();
		public List<T> Select<T>(Select SelectRequest);
		public void Insert<T>(T Item);
		public void Update<T>(T Item, Select UpdateRequest);
		public void Delete<T>(T Item);
	}
}
