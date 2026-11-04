using ProcessAutomation.Framework.DTO.Table;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProcessAutomation.Common.DTO.Entity.Table
{
    public class USER_LOGIN : COMMON_FIELD
    {
        [Key]
        public int ID_User_Login { get; set; }
        public int ID_User { get; set; }
        public int ID_Token { get; set; }
        public int ID_Device { get; set; }
        public DateTime Login_Date { get; set; }
        public required string Login_Latitude { get; set; }
        public required string Login_Longitude { get; set; }
        public DateTime? Logout_Date { get; set; }
        public string? Logout_Latitude { get; set; }
        public string? Logout_Longitude { get; set; }

        [ForeignKey("ID_User")]
        public virtual CAT_USER? Cat_User { get; set; }

        [ForeignKey("ID_Device")]
        public virtual CAT_DEVICE? Cat_Device { get; set; }

        [ForeignKey("ID_Token")]
        public virtual TOKEN? Token { get; set; }
    }
}
