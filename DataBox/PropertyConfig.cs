using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataBox;

public static class Property
{
	public static IPropertyTypeStage Name(string PropertyName)
	{
		return new PropertyConfig(PropertyName);
	}
}


public class PropertyConfig :
	IPropertyTypeStage,
	IPropertyKeyStage,
	IPropertyCastStage,
	IPropertyCreateStage
{
	private string _name;
	private Type _type;
	private bool _isPrimaryKey;
	private bool _isForeignKey;
	private string? _dbName = null;

	public PropertyConfig(string Name)
	{
		_name = Name;
	}

	public IPropertyKeyStage TypeOf(Type Type)
	{
		_type = Type;
		return this;
	}

	public IPropertyCastStage IsForeignKey()
	{
		_isPrimaryKey = false;
		_isForeignKey = true;
		return this;
	}

	public IPropertyCastStage IsPrimaryKey()
	{
		_isPrimaryKey = true;
		_isForeignKey = false;
		return this;
	}

	public IPropertyCreateStage Cast(string PropertyName)
	{
		_dbName = PropertyName;
		return this;
	}

	public PropertyConfig Create()
	{
		return this;
	}
}

public interface IPropertyTypeStage
{
	public IPropertyKeyStage TypeOf(Type Type);
}

public interface IPropertyKeyStage : IPropertyCastStage, IPropertyCreateStage
{
	public IPropertyCastStage IsPrimaryKey();
	public IPropertyCastStage IsForeignKey();
}

public interface IPropertyCastStage : IPropertyCreateStage
{
	public IPropertyCreateStage Cast(string PropertyName);
}

public interface IPropertyCreateStage
{
	public PropertyConfig Create();
}