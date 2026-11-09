using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAutomation.Common.DTO.Entity.Table
{
    public class CHAT
    {
        public CHAT()
        {
            Message_Collection = new HashSet<CHAT_MESSAGE>();
        }

        [Key]
        public int ID_Chat { get; set; }
        public required string Chat { get; set; }
        public string? Chat_Image { get; set; }
        public int ID_User { get; set; }
        public required DateTime Creation_Date { get; set; }
        public DateTime? Modification_Date { get; set; }
        public bool Active { get; set; }

        [ForeignKey("ID_User")]
        public required CAT_USER Cat_User { get; set; }

        public virtual ICollection<CHAT_MESSAGE> Message_Collection { get; set; }
    }
}
