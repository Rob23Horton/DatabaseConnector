using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBox;

public static class Config()
{
	public static DataConfig New(Type Type)
	{
		return new DataConfig(Type);
	}
}

public class DataConfig
{
	private readonly Type _type;
	private List<PropertyConfig> _properties;

	public DataConfig(Type ObjectType)
	{
		_type = ObjectType;
		_properties = new List<PropertyConfig>();
	}

	public bool AddProperty(PropertyConfig Property)
	{
		if (_properties.Contains(Property)) return false;

		_properties.Add(Property);
		return true;
	}
}
