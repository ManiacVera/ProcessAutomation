using System.ComponentModel.DataAnnotations;

namespace ProcessAutomation.Framework.DTO.Table
{
    public class COMMON_CATALOG : COMMON_FIELD
    {
        [Required]
        public bool Active { get; set; }
    }
}
