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
		public List<T> Select<T>(Select SelectRequest);
	}
}
