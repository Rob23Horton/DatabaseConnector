using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseConnector.Models
{
	public class Where
	{
		public Where(string Table, string ValueName, object Value)
		{
			this.Table = Table;
			this.ValueName = ValueName;
			this.Value = Value;
		}
		public Where(string Table, string ValueName, object Value, bool IsLike)
		{
			this.Table = Table;
			this.ValueName = ValueName;
			this.Value = Value;
			this.IsLike = IsLike;
		}

		public string Table { get; set; }
		public string ValueName { get; set; }
		public object Value { get; set; }
		public bool IsLike { get; set; }
	}
}
