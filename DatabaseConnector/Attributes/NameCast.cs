using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseConnector.Attributes
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple=false)]
	public class NameCast : Attribute
	{
		public NameCast(string Name)
		{
			this.Name = Name;
		}

		public string Name { get; set; }
	}
}
