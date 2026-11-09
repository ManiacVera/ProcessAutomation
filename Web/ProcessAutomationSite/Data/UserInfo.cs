using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ProcessAutomationSite.Data
{
    public class UserInfo
    {
        public int IdUser { get; set; }
        [Key]
        public string UserName { get; set; }
        public string Name { get; set; }
        public string FatherLastName { get; set; }
        public string MotherLastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Photo { get; set; }
        public DateTime DateBirth { get; set; }
    }
}
