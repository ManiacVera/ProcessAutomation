using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.DTO.Type;
using System.Collections.Generic;

namespace ProcessAutomation.Common.DTO.Info
{
    public class TaskComponentInfo
    {
        //public ComponentStatementTypeEnum ComponentStatement { get; set; }
        public ITaskComponent? TaskComponent { get; set;}
        public IComponentService? ComponentService { get; set; }
        public IComponentDTO? ComponentDTO { get; set; }
        public IComponentSnippetSource? ComponentSnippetSource { get; set; }
        public ListInputFormatType? ComponentFieldList { get; set; }
        public ListInputFormatType? StepFieldList { get; set; }
    }
}
