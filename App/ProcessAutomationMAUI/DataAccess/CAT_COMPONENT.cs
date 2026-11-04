using System.ComponentModel.DataAnnotations;

namespace ProcessAutomationMAUI.DataAccess
{
    public class CAT_COMPONENT
    {
        [Key]
        public int ID_Component { get; set; }
        public string Component { get; set; }
        public string Component_Code { get; set; }
        public DateTime Creation_Date { get; set; }
        public DateTime Modification_Date { get; set; }
        public bool Active { get; set; }
    }
}
