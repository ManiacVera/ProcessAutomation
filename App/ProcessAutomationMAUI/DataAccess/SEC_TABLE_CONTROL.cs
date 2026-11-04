using System.ComponentModel.DataAnnotations;

namespace ProcessAutomationMAUI.DataAccess
{
    public class SEC_TABLE_CONTROL
    {
        [Key]
        public int ID_Table_Control { get; set; }
        public string Table_Name { get; set; }
        public DateTime Creation_Date { get; set; }
        public DateTime Last_Update { get; set; }
        public bool Active { get; set; }
    }
}
