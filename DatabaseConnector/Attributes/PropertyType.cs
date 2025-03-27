using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseConnector.Attributes
{
	public class PropertyType : Attribute
	{
		public PropertyType(string Type, bool IsPrimaryKey)
		{
			this.Type = Type;
			this.IsPrimaryKey = IsPrimaryKey;
		}

		public PropertyType(string Type)
		{
			this.Type = Type;
		}

		public string Type { get; set; }
		public bool IsPrimaryKey { get; set; }
	}
}
