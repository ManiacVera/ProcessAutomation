using System.ComponentModel.DataAnnotations;

namespace ProcessAutomationMAUI.DataAccess
{
    public class CAT_DECLARATION_TYPE
    {
        [Key]
        public int ID_Declaration_Type { get; set; }
        public string Declaration_Type { get; set; }
        public string Declaration_Type_Code { get; set; }
        public DateTime Creation_Date { get; set; }
        public DateTime Modification_Date { get; set; }
        public bool Active { get; set; }
    }
}
