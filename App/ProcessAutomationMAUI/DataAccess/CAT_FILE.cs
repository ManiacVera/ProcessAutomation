using System.ComponentModel.DataAnnotations;

namespace ProcessAutomationMAUI.DataAccess
{
    public class CAT_FILE
    {
        [Key]
        public int ID_File { get; set; }
        public string File_Name { get; set; }
        public string File_Type { get; set; }
        public string Source { get; set; }
        public float Version { get; set; }
        public DateTime Creation_Date { get; set; }
        public DateTime Modification_Date { get; set; }
        public bool Active { get; set; }
    }
}
