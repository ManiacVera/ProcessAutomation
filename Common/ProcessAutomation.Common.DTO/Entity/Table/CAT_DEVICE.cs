using ProcessAutomation.Framework.DTO.Attribute;
using ProcessAutomation.Framework.DTO.Table;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAutomation.Common.DTO.Entity.Table
{
    public class CAT_DEVICE : COMMON_CATALOG
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID_Device { get; set; }

        [NameField]
        [Required]
        public required string Device { get; set; }

        [CodeField]
        [Required]
        public required string Device_Code { get; set; }
    }
}
