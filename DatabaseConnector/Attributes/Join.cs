using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseConnector.Attributes
{
	[AttributeUsage(AttributeTargets.Property)]
	public class Join : Attribute
	{
		public Join(string SourceTable, string SourceValue, string JoinTable, string JoinValue)
		{
			this.SourceTable = SourceTable;
			this.SourceValue = SourceValue;
			this.JoinTable = JoinTable;
			this.JoinValue = JoinValue;
		}

		public string SourceTable { get; set; }
		public string SourceValue { get; set; }
		public string JoinTable { get; set; }
		public string JoinValue { get; set; }
	}
}
