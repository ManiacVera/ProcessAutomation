using ProcessAutomation.Common.DTO.Constants;
using ProcessAutomation.Framework.DTO.Attribute;
using ProcessAutomation.Framework.DTO.Table;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAutomation.Common.DTO.Entity.Table
{
    public class CAT_USER : COMMON_CATALOG
    {
        public CAT_USER()
        {
        }

        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID_User { get; set; }
        
        [CodeField]
        [Required]
        [Column(TypeName = CommonConstants.VARCHAR_50)]
        public required string User_Name { get; set; }

        [Required]
        [Column(TypeName = CommonConstants.VARCHAR_100)]
        public required string Password { get; set; }

        [Required]
        [Column(TypeName = CommonConstants.VARCHAR_100)]
        public required string Name { get; set; }

        [Required]
        [Column(TypeName = CommonConstants.VARCHAR_100)]
        public required string Last_Name { get; set; }

        [NameField]
        [Required]
        [Column(TypeName = CommonConstants.VARCHAR_100)]
        public required string Email { get; set; }

        [Column(TypeName = CommonConstants.VARCHAR_100)]
        public string? Photo { get; set; }

        [Column(TypeName = CommonConstants.DATETIME)]
        public DateTime? Birthdate { get; set; }

        [NotMapped]
        public virtual string? ContextToken { get; set; }
        //public virtual IMAGE Image {get;set;}
    }
}
