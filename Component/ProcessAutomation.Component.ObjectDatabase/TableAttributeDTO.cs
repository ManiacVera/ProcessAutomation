using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessAutomation.Component
{
    public class TableAttributeDTO
	{
        public string ImportOption { get; set; }
        public string Table { get; set; }
        public string RenameTable { get; set; }
        public string Scheme { get; set; }
        public string Columns { get; set; }
        public string PrimaryKeyName { get; set; }
        public string PrimaryKeyColumns { get; set; }
        //public string ForeignKeys { get; set; }
        //public string Indexes { get; set; }
        //public string UniqueConstraints { get; set; }
        //public string AlterTypes { get; set; }        
        public bool RemoveDuplicate { get; set; }
    }
}
