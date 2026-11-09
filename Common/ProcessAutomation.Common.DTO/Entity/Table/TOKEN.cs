using ProcessAutomation.Common.DTO.Constants;
using ProcessAutomation.Framework.DTO.Attribute;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAutomation.Common.DTO.Entity.Table
{
    public class TOKEN
    {
        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID_Token { get; set; }

        [NameField]
        [Required]
        [Column(TypeName = CommonConstants.VARCHAR_100)]
        public required string Token { get; set; }

        [Required]
        [Column(TypeName = CommonConstants.DATETIME)]
        public required DateTime Expiration_Date { get; set; }

        [Column(TypeName = CommonConstants.DATETIME)]
        public DateTime? Finish_Date { get; set; }

        [Required]
        public int ID_Token_Type { get; set; }

        [Required]
        public int ID_User_Created { get; set; }

        [Required]
        [Column(TypeName = CommonConstants.DATETIME)]
        public required DateTime Creation_Date { get; set; }

        [Required]
        public bool Active { get; set; }
    }
}
