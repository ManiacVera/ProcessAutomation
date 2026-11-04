using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessAutomation.Common.DTO.Info
{
    public class ProcessLogInfo
    {
        public string? ProcessName { get; set; }
        public string? TaskName { get; set; }
        public int StepNumber { get; set; }
        public string? StepName { get; set; }
        public string? Message { get; set; }
    }
}
