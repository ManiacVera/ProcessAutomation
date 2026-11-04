using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAutomation.Common.DTO.Entity.Table
{
    public class CHAT_MESSAGE
    {
        public CHAT_MESSAGE()
        {
        }

        [Key]
        public int ID_Chat_Message { get; set; }
        public required string Message { get; set; }
        public bool Is_Deleted { get; set; }
        public bool Is_All_Readed { get; set; }
        public int ID_User_Sending { get; set; }
        public int ID_Chat { get; set; }
        public required DateTime Creation_Date { get; set; }
        public DateTime? Modification_Date { get; set; }

        [ForeignKey("ID_User_Sending")]
        public required CAT_USER Cat_User_Sending { get; set; }

        [ForeignKey("ID_Chat")]
        public required CHAT Chat { get; set; }

    }
}
