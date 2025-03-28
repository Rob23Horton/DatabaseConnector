using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseConnector.Models
{
	public class Property
	{
		public string Name { get; set; }
		public string Type { get; set; }
		public bool IsPrimaryKey { get; set; }
	}
}
