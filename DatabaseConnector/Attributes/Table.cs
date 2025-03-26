using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseConnector.Attributes
{
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
	public class Table : Attribute
	{
		public Table(string Name)
		{
			this.Name = Name;
		}

		public string Name { get; set; }
	}
}
