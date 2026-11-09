using System.Collections.Generic;

namespace ProcessAutomation.Common.DTO.Info
{
    public class StatementInfo
    {
        public int StatementId { get; set; }
        public string? Statement { get; set; }
        public string? Description { get; set; }
        public string? Symbol { get; set; }
        public string? Separator { get; set; }
        public string? Format { get; set; }
        public List<FieldDataInfo>? ParameterList { get; set; }

        public StatementInfo()
        {

        }

        public StatementInfo(StatementInfo statementInfo)
        {
            StatementId = statementInfo.StatementId;
            Statement = statementInfo.Statement;
            Description = statementInfo.Description;
            Separator = statementInfo.Separator;
            Format = statementInfo.Format;
            ParameterList = statementInfo.ParameterList;
        }
    }
}
