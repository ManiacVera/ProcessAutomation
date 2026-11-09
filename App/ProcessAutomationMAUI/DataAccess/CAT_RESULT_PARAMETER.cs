using System.ComponentModel.DataAnnotations;

namespace ProcessAutomationMAUI.DataAccess
{
    public class CAT_RESULT_PARAMETER
    {
        [Key]
        public int ID_Result_Parameter { get; set; }
        public string Result_Parameter { get; set; }
        public object Value { get; set; }
        public string Control_Type { get; set; }
        public DateTime Creation_Date { get; set; }
        public DateTime Modification_Date { get; set; }
        public bool Active { get; set; }
    }
}
