using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAutomation.Framework.DTO.Table
{
    public class COMMON_FIELD
    {
        public int Alteration_Counter { get; set; }

        [Required]
        public int ID_User_Created { get; set; }

        [Required]
        [Column(TypeName = "DATETIME")]
        public DateTime Creation_Date { get; set; }

        public int? ID_User_Modified { get; set; }


        [Column(TypeName = "DATETIME")]
        public DateTime? Modification_Date { get; set; }
    }
}
