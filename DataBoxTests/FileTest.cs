using DataBox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseConnectorTests;

public class FileTest : IDataRecord<FileTest>
{
	public int Id { get; set; }
	public string Name { get; set; }
	public Folder Folder { get; set; }


	public DataConfig GetConfig()
	{
		var config = Config.New(this.GetType());

		config.AddProperty(Property.Name(nameof(Id))
				.TypeOf(Id.GetType())
				.IsPrimaryKey()
				.Cast("FileId")
				.Create()
			);

		config.AddProperty(Property.Name(nameof(Name))
				.TypeOf(Name.GetType())
				.Create()
			);

		config.AddProperty(Property.Name(nameof(Folder))
				.TypeOf(Folder.GetType())
				.Create()
			);

		return config;
	}
}
