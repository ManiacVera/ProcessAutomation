using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAutomation.Common.DTO.Entity.Table
{
    public class CHAT_USER_MESSAGE
    {
        [Key]
        public int ID_Chat_User_Message { get; set; }
        public int ID_User { get; set; }
        public int ID_Chat_Message { get; set; }
        public bool Is_New { get; set; }
        public bool Is_Readed { get; set; }
        public required DateTime Creation_Date { get; set; }
        public DateTime? Modification_Date { get; set; }

        [ForeignKey("ID_Chat_Message")]
        public required CHAT_MESSAGE Chat_Message { get; set; }

        [ForeignKey("ID_User")]
        public required CAT_USER Cat_User { get; set; }
    }
}
