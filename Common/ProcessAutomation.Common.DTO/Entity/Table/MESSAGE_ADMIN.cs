using ProcessAutomation.Common.DTO.Constants;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAutomation.Common.DTO.Entity.Table
{
    public class MESSAGE_ADMIN
    {
        public MESSAGE_ADMIN()
        {
        }

        [Key]
        [Required]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID_Message_Admin { get; set; }

        [Required]
        public int ID_User { get; set; }

        [Required]

        [Column(TypeName = CommonConstants.VARCHAR_1000)]
        public required string Message { get; set; }

        [Required]
        [Column(TypeName = CommonConstants.VARCHAR_30)]
        public required string Message_Type { get; set; }

        [Required]
        [Column(TypeName = CommonConstants.DATETIME)]
        public required DateTime Creation_Date { get; set; }

        [Column(TypeName = CommonConstants.DATETIME)]
        public DateTime? Modification_Date { get; set; }

        [Required]
        public bool Active { get; set; }

        [ForeignKey("ID_User")]
        public CAT_USER? Cat_User { get; set; }

    }
}
