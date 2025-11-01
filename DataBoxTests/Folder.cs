using DataBox;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseConnectorTests;

public class Folder : IDataRecord<Folder>
{
	public int Id { get; set; }
	public string Name { get; set; }
	public int? ParentId { get; set; }

	public DataConfig GetConfig()
	{
		var config = new DataConfig(this.GetType());

		config.AddProperty(
			Property.Name(nameof(Id))
				.TypeOf(Id.GetType())
				.IsPrimaryKey()
				.Cast("FolderId")
				.Create()
			);

		config.AddProperty(
			Property.Name(nameof(Name))
				.TypeOf(Name.GetType())
				.Cast("FilePath")
				.Create()
			);

		config.AddProperty(
			Property.Name(nameof(ParentId))
				.TypeOf(typeof(int?))
				.Create()
			);

		return config;
	}
}
