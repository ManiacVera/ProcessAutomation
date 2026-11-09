using ProcessAutomation.Framework.DTO.Attribute;
using ProcessAutomation.Framework.DTO.Table;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAutomation.Common.DTO.Entity.Table
{
    public class CAT_STATUS : COMMON_CATALOG
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID_Status { get; set; }

        [NameField]
        [Required]
        public required string Status { get; set; }

        [NumberField]
        [Required]
        public int Number { get; set; }

        [Required]
        public int ID_Status_Type { get; set; }

        [ForeignKey("ID_Status_Type")]
        public virtual required CAT_STATUS_TYPE Status_Type { get; set; }
    }
}
