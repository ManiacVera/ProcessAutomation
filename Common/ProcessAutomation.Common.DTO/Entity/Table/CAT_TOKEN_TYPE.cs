using ProcessAutomation.Framework.DTO.Attribute;
using ProcessAutomation.Framework.DTO.Table;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAutomation.Common.DTO.Entity.Table
{
    public class CAT_TOKEN_TYPE : COMMON_CATALOG
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID_Token_Type { get; set; }

        [NameField]
        [Required]
        public required string Token_Type { get; set; }

        [CodeField]
        [Required]
        public required string Token_Type_Code { get; set; }
    }
}
