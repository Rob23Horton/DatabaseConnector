using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseConnector.Attributes
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple=false)]
	public class SourceTable : Attribute
	{
		public SourceTable(string TableName)
		{
			this.TableName = TableName;
		}

		public string TableName { get; set; }
	}
}
