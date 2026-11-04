using System.ComponentModel.DataAnnotations;

namespace ProcessAutomationMAUI.DataAccess
{
    public class CAT_ENCAPSULATED_PARTITION_TYPE
    {
        [Key]
        public int ID_Encapsulated_Partition_Type { get; set; }
        public string Encapsulated_Partition_Type { get; set; }
        public string Encapsulated_Partition_Type_Code { get; set; }
        public DateTime Creation_Date { get; set; }
        public DateTime Modification_Date { get; set; }
        public bool Active { get; set; }
    }
}
