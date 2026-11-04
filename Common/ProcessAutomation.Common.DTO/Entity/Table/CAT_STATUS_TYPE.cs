using ProcessAutomation.Framework.DTO.Attribute;
using ProcessAutomation.Framework.DTO.Table;
using System.ComponentModel.DataAnnotations;

namespace ProcessAutomation.Common.DTO.Entity.Table
{
	public class CAT_STATUS_TYPE : COMMON_CATALOG
	{
		[Key]
		public int ID_Status_Type { get; set; }
		[NameField]
		public required string Status_Type { get; set; }
		[CodeField]
		public required string Status_Type_Code { get; set; }
	}
}
