using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseConnector.Models
{
	public class AnotherTableValue
	{
		public AnotherTableValue()
		{

		}

		public AnotherTableValue(string Table, string ValueName)
		{
			this.Table = Table;
			this.ValueName = ValueName;
		}

		public string Table { get; set; }
		public string ValueName { get; set; }
	}
}
