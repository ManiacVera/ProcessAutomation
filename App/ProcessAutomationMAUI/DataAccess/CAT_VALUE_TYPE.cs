using System.ComponentModel.DataAnnotations;

namespace ProcessAutomationMAUI.DataAccess
{
    public class CAT_VALUE_TYPE
    {
        [Key]
        //[Required]
        //[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID_Value_Type { get; set; }
        public string Value_Type { get; set; }
        public string Value_Type_Code { get; set; }
        public DateTime Creation_Date { get; set; }
        public DateTime Modification_Date { get; set; }
        public bool Active { get; set; }
    }
}
