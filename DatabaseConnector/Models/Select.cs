using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseConnector.Models
{
	public class Select
	{
		public Select()
		{
			
		}

		public Select(string Table)
		{
			this.Table = Table;
		}

		public void AddWhere(Where Where)
		{
			this.Wheres.Add(Where);
		}
		public void AddWhere(string ValueName, object Value)
		{
			AddWhere(new Where(this.Table, ValueName, Value));
		}
		public void AddWhere(string Table, string ValueName, object Value)
		{
			AddWhere(new Where(Table, ValueName, Value));
		}

		public string Table { get; set; }
		public List<Where> Wheres { get; set; } = new List<Where>();
	}
}
