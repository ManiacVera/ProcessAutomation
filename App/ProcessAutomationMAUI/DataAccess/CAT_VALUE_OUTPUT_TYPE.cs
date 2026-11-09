using System.ComponentModel.DataAnnotations;

namespace ProcessAutomationMAUI.DataAccess
{
    public class CAT_VALUE_OUTPUT_TYPE
    {
        [Key]
        public int ID_Value_Output_Type { get; set; }
        public string Value_Output_Type { get; set; }
        public string Value_Output_Type_Code { get; set; }
        public DateTime Creation_Date { get; set; }
        public DateTime Modification_Date { get; set; }
        public bool Active { get; set; }
    }
}
