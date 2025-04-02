using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseConnector.Models
{
	public class Update : Select
	{

		public Update() : base()
		{

		}

		public void AddEditedValues(List<string> EditedValues)
		{
			UseEditedValues = true;
			this.EditedValues.AddRange(EditedValues);
		}
		
		public void AddEditedValues(string EditedValue)
		{
			UseEditedValues = true;
			EditedValues.Add(EditedValue);
		}

		public bool UseEditedValues { get; set; }
		public List<string> EditedValues { get; set; }

	}
}
