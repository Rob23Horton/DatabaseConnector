using Azure.Core;
using DatabaseConnector.Attributes;
using DatabaseConnector.Models;
using MySqlConnector;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Net.Http.Headers;
using System.Reflection;

namespace DatabaseConnector.Services
{
	public class DatabaseConnector : IDatabaseConnector
	{
		private readonly IDatabaseLink _database;
		public DatabaseConnector(DatabaseType Type, string DatabaseString)
		{
			switch (Type)
			{
				case DatabaseType.MySql:
					_database = new MySqlDatabaseLink(DatabaseString);

					break;

				case DatabaseType.SqlServer:
					_database = new SqlServerDatabaseLink(DatabaseString);

					break;

				case DatabaseType.Sqlite:
					_database = new SqliteDatabaseLink(DatabaseString);

					break;

				case DatabaseType.MSqlite:
					_database = new MSqliteDatabaseLink(DatabaseString);

					break;
			}
		}

		private string GetWheres(string DefaultTable, List<Where> Where)
		{
			string query = "";

			foreach (Where where in Where)
			{
				//Fills in the blank table value
				if (String.IsNullOrEmpty(where.Table))
				{
					where.Table = DefaultTable;
				}

				//Adding like or just =
				if (where.IsLike)
				{
					query += $"{where.Table}.{where.ValueName} LIKE ";
				}
				else
				{
					query += $"{where.Table}.{where.ValueName} = ";
				}

				//Fills in the value
				if (where.Value is string strVal)
				{
					query += $"'{_database.EscapeString(strVal)}' AND ";
				}
				else if (where.Value is int intVal)
				{
					query += $"{intVal} AND ";
				}
				else if (where.Value is long longVal)
				{
					query += $"{longVal} AND ";
				}
				else if (where.Value is bool boolVal)
				{
					query += $"b'{(boolVal ? '1' : '0')}' AND ";
				}
				else if (where.Value is DateTime dateVal)
				{
					query += $"'{dateVal.ToString("yyyy-MM-dd HH:mm:ss")}' AND ";
				}
				else if (where.Value is AnotherTableValue anthTblVal)
				{
					query += $"{anthTblVal.Table}.{anthTblVal.ValueName} AND ";
				}
			}

			if (query.Length > 0)
			{
				query = "WHERE " + query.Substring(0, query.Length - 5);
			}

			return query;
		}

		public List<T> Select<T>(Select SelectRequest)
		{
			Type classType = typeof(T);
			PropertyInfo[] ClassPropertyInfo = classType.GetProperties();

			//Gets the table attribute and gets the table name value from it
			Table? tableAttribute = (Table?)classType.GetCustomAttribute(typeof(Table), false);
			if (tableAttribute is null)
			{
				throw new Exception("Class must have table attribute!");
			}

			string table = tableAttribute.Name;



			string values = "";
			string joins = "";

			foreach (PropertyInfo propertyInfo in ClassPropertyInfo)
			{
				//Creates values string
				string Table = table;
				string Name = propertyInfo.Name;
				string As = "";

				//Adds As value to query so that it can be found when creating object
				NameCast? castAttribute = (NameCast?)propertyInfo.GetCustomAttribute(typeof(NameCast), false);
				if (castAttribute is not null)
				{
					As = Name;
					Name = castAttribute.Name;
				}

				SourceTable? sourceAttribute = (SourceTable?)propertyInfo.GetCustomAttribute(typeof(SourceTable), false);
				if (sourceAttribute is not null)
				{
					Table = sourceAttribute.TableName;
				}

				values += $"{Table}.{Name} {(string.IsNullOrEmpty(As) ? "" : $"As {As}")}, ";


				//Creates joins string
				object[] joinAttributes = propertyInfo.GetCustomAttributes(typeof(Join), false);
				foreach (Join joinAttribute in joinAttributes.Where(j => j.GetType() == typeof(Join)))
				{
					joins += $"JOIN {joinAttribute.JoinTable} ON {joinAttribute.SourceTable}.{joinAttribute.SourceValue} = {joinAttribute.JoinTable}.{joinAttribute.JoinValue} ";
				}

			}

			//Removes extra ', ' off the end fo values
			if (values.Length > 0)
			{
				values = values.Substring(0, values.Length - 2);
			}



			//Creates where string
			string wheres = GetWheres(table, SelectRequest.Wheres);


			string query = $"SELECT {values} FROM {table} {joins} {wheres};";

			List<T> result = new List<T>();

			//Executes query and returns a datareader for the data
			DbDataReader dataReader = _database.ReadExecute(query);

			//Gets all items from data reader
			while (dataReader.Read())
			{
				T? item = (T?)Activator.CreateInstance(classType);

				if (item is null)
				{
					continue;
				}

				foreach (PropertyInfo currentPropertyInfo in ClassPropertyInfo)
				{
					try
					{
						if (currentPropertyInfo.PropertyType == typeof(bool))
						{
							currentPropertyInfo.SetValue(item, dataReader.GetBoolean(currentPropertyInfo.Name));
						}
						if (currentPropertyInfo.PropertyType == typeof(DateTime))
						{
							if (dataReader[currentPropertyInfo.Name].GetType() == typeof(DateTime))
							{
								currentPropertyInfo.SetValue(item, dataReader[currentPropertyInfo.Name]);
							}
							else //Parses string of DateTime to DateTime
							{
								currentPropertyInfo.SetValue(item, DateTime.Parse(dataReader[currentPropertyInfo.Name].ToString()!));
							}

						}
						else
						{
							currentPropertyInfo.SetValue(item, dataReader[currentPropertyInfo.Name]);
						}
					}
					catch
					{
						continue;
					}
				}

				result.Add(item);
			}

			dataReader.Close();
			_database.Disconnect();

			return result;
		}

		public void CreateTable<T>()
		{
			Type classType = typeof(T);
			PropertyInfo[] ClassPropertyInfo = classType.GetProperties();

			//Gets the table attribute and gets the table name value from it
			Table? tableAttribute = (Table?)classType.GetCustomAttribute(typeof(Table), false);
			if (tableAttribute is null)
			{
				throw new Exception("Class must have table attribute!");
			}

			List<Property> properties = new List<Property>();

			foreach (PropertyInfo propertyInfo in ClassPropertyInfo)
			{
				SourceTable? sourceAttribute = (SourceTable?)propertyInfo.GetCustomAttribute(typeof(SourceTable), false);

				if (sourceAttribute is not null && sourceAttribute.TableName != tableAttribute.Name)
				{
					continue;
				}

				Property newProperty = new Property();

				//Name which is used as the property name on the table
				newProperty.Name = propertyInfo.Name;
				newProperty.Type = propertyInfo.PropertyType.Name.ToString();

				//Adds As value to query so that it can be found when creating object
				NameCast? castAttribute = (NameCast?)propertyInfo.GetCustomAttribute(typeof(NameCast), false);
				if (castAttribute is not null)
				{
					newProperty.Name = castAttribute.Name;
				}

				PropertyType? typeAttribute = (PropertyType?)propertyInfo.GetCustomAttribute(typeof(PropertyType), false);
				if (typeAttribute is not null)
				{
					newProperty.Type = typeAttribute.Type;

					if (typeAttribute.IsPrimaryKey)
					{
						newProperty.IsPrimaryKey = true;
					}
				}

				properties.Add(newProperty);
			}

			//Creates query
			string query = $"CREATE TABLE {tableAttribute.Name} ({_database.BuildCreateTableQuery(properties)});";

			Console.WriteLine(query);

			//Executes query and returns a datareader for the data
			_database.Execute(query);
		}

		public void Insert<T>(T Item)
		{
			Insert<T>(Item, false);
		}

		public void Insert<T>(T Item, bool InsertPrimaryKey)
		{
			Type classType = typeof(T);
			PropertyInfo[] ClassPropertyInfo = classType.GetProperties();

			//Gets the table attribute and gets the table name value from it
			Table? tableAttribute = (Table?)classType.GetCustomAttribute(typeof(Table), false);
			if (tableAttribute is null)
			{
				throw new Exception("Class must have table attribute!");
			}

			string table = tableAttribute.Name;

			string properties = "";
			string values = "";

			foreach (PropertyInfo propertyInfo in ClassPropertyInfo)
			{
				string Name = propertyInfo.Name;

				//Checks if the property should be included

				if (!InsertPrimaryKey)
				{
					//Don't include if it's a primary key
					PropertyType? primaryKeyAttribute = (PropertyType?)propertyInfo.GetCustomAttribute(typeof(PropertyType), false);
					if (primaryKeyAttribute is not null && primaryKeyAttribute.IsPrimaryKey)
					{
						continue;
					}
				}

				//If there are joins it's not part of this table
				object[] joinAttributes = propertyInfo.GetCustomAttributes(typeof(Join), false);
				if (joinAttributes.Length > 0)
				{
					continue;
				}

				//If table is different then it's not part of the table
				SourceTable? sourceAttribute = (SourceTable?)propertyInfo.GetCustomAttribute(typeof(SourceTable), false);
				if (sourceAttribute is not null && sourceAttribute.TableName != table)
				{
					continue;
				}


				//Changes the name to the correct one for the insert
				NameCast? castAttribute = (NameCast?)propertyInfo.GetCustomAttribute(typeof(NameCast), false);
				if (castAttribute is not null)
				{
					Name = castAttribute.Name;
				}

				//Works out value type
				object value = propertyInfo.GetValue(Item);
				if (value is string strVal)
				{
					values += $"'{_database.EscapeString(strVal)}', ";
				}
				else if (value is int intVal)
				{
					values += $"{intVal}, ";
				}
				else if (value is long longVal)
				{
					values += $"{longVal}, ";
				}
				else if (value is bool boolVal)
				{
					values += $"b'{(boolVal ? '1' : '0')}', ";
				}
				else if (value is DateTime dateVal)
				{
					values += $"'{dateVal.ToString("yyyy-MM-dd HH:mm:ss")}', ";
				}
				else //Skip if type is unsupported
				{
					continue;
				}

				properties += $"{Name}, ";
			}

			//Removes extra ', ' off the end off properties and values
			if (values.Length > 0)
			{
				properties = properties.Substring(0, properties.Length - 2);
				values = values.Substring(0, values.Length - 2);
			}

			string query = $"INSERT INTO {table} ({properties}) VALUES ({values})";

			_database.Execute(query);

		}

		public void Update<T>(T Item, Update UpdateRequest)
		{
			Type classType = typeof(T);
			PropertyInfo[] ClassPropertyInfo = classType.GetProperties();

			//Gets the table attribute and gets the table name value from it
			Table? tableAttribute = (Table?)classType.GetCustomAttribute(typeof(Table), false);
			if (tableAttribute is null)
			{
				throw new Exception("Class must have table attribute!");
			}

			string table = tableAttribute.Name;

			string values = "";

			foreach (PropertyInfo propertyInfo in ClassPropertyInfo)
			{
				string Name = propertyInfo.Name;

				//Checks if the property should be included
				//If there are joins it's not part of this table
				object[] joinAttributes = propertyInfo.GetCustomAttributes(typeof(Join), false);
				if (joinAttributes.Length > 0)
				{
					continue;
				}
				//If table is different then it's not part of the table
				SourceTable? sourceAttribute = (SourceTable?)propertyInfo.GetCustomAttribute(typeof(SourceTable), false);
				if (sourceAttribute is not null && sourceAttribute.TableName != table)
				{
					continue;
				}


				//Changes the name to the correct one for the insert
				NameCast? castAttribute = (NameCast?)propertyInfo.GetCustomAttribute(typeof(NameCast), false);
				if (castAttribute is not null)
				{
					Name = castAttribute.Name;
				}


				//If it's a primary key and isn't null then use it as a where statement
				PropertyType? primaryKeyAttribute = (PropertyType?)propertyInfo.GetCustomAttribute(typeof(PropertyType), false);
				if (primaryKeyAttribute is not null && primaryKeyAttribute.IsPrimaryKey)
				{
					if (propertyInfo.PropertyType == typeof(int) || propertyInfo.PropertyType == typeof(long)) //If the type is int or long
					{
						int intVal = int.Parse(propertyInfo.GetValue(Item)!.ToString()!);

						//A Primary key shouldn't ever be 0
						if (intVal != 0)
						{
							UpdateRequest.AddWhere(Name, intVal);
						}
					}
					else if(propertyInfo.GetValue(Item) is not null && propertyInfo.GetValue(Item) != default)
					{
						UpdateRequest.AddWhere(Name, propertyInfo.GetValue(Item));
					}

					continue;
				}


				//Excludes the value if it hasn't been edited
				//Has to be here due to it being after the primary key is added to the where statement
				if (UpdateRequest.UseEditedValues && !UpdateRequest.EditedValues.Contains(Name))
				{
					continue;
				}


				//Works out value type
				object value = propertyInfo.GetValue(Item);

				string currentValue = "";
				if (value is string strVal)
				{
					currentValue = $"'{_database.EscapeString(strVal)}', ";
				}
				else if (value is int intVal)
				{
					currentValue = $"{intVal}, ";
				}
				else if (value is long longVal)
				{
					currentValue = $"{longVal}, ";
				}
				else if (value is bool boolVal)
				{
					currentValue = $"b'{(boolVal ? '1' : '0')}', ";
				}
				else if (value is DateTime dateVal)
				{
					currentValue = $"'{dateVal.ToString("yyyy-MM-dd HH:mm:ss")}', ";
				}
				else //Skip if type is unsupported
				{
					continue;
				}

				values += $"{Name} = {currentValue}";
			}

			//Removes extra ', ' off the end off properties and values
			if (values.Length > 0)
			{
				values = values.Substring(0, values.Length - 2);
			}

			string where = GetWheres(table, UpdateRequest.Wheres);

			string query = $"UPDATE {table} SET {values} {where}";

			_database.Execute(query);
		}

		public void Delete<T>(T Item)
		{
			Type classType = typeof(T);
			PropertyInfo[] ClassPropertyInfo = classType.GetProperties();

			//Gets the table attribute and gets the table name value from it
			Table? tableAttribute = (Table?)classType.GetCustomAttribute(typeof(Table), false);
			if (tableAttribute is null)
			{
				throw new Exception("Class must have table attribute!");
			}

			string table = tableAttribute.Name;

			Where PrimaryKeyWhere = new Where("", "", "");

			foreach (PropertyInfo property in ClassPropertyInfo)
			{
				PropertyType? typeAttribute = (PropertyType?)property.GetCustomAttribute(typeof(PropertyType), false);
				if (typeAttribute is not null && typeAttribute.IsPrimaryKey)
				{
					string Name = property.Name;

					NameCast? castAttribute = (NameCast?)property.GetCustomAttribute(typeof(NameCast), false);
					if (castAttribute is not null)
					{
						Name = castAttribute.Name;
					}

					PrimaryKeyWhere = new Where("", Name, property.GetValue(Item)!);

					break;
				}
			}

			string where = GetWheres(table, new List<Where>() { PrimaryKeyWhere });

			string query = $"DELETE FROM {table} {where}";

			_database.Execute(query);

		}
	}
}
