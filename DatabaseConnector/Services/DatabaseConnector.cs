using DatabaseConnector.Attributes;
using DatabaseConnector.Models;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
			}
		}

		private string GetWheres(List<Where> Where)
		{
			string query = "";

			foreach (Where where in Where)
			{
				query += $"{where.Table}.{where.ValueName} = ";

				if (where.Value is string strVal)
				{
					query += $"'{MySqlHelper.EscapeString(strVal)}' AND ";
				}
				else if (where.Value is int intVal)
				{
					query += $"{intVal} AND ";
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
			string wheres = GetWheres(SelectRequest.Wheres);


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
	}
}
