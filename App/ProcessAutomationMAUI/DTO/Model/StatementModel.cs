using CommunityToolkit.Mvvm.ComponentModel;
using ProcessAutomation.Common.DTO.Info;
using System.Collections.Generic;

namespace ProcessAutomationMAUI.DTO.Model
{
    public class StatementModel : ObservableObject
    {
        public int StatementId { get; set; }
        public string Statement { get; set; }
        public string Description { get; set; }
        public string Symbol { get; set; }
        public string Separator { get; set; }
        public string Format { get; set; }
        public List<FieldDataInfo> ParameterList { get; set; }

        public StatementModel()
        {

        }

        public StatementModel(StatementModel statementInfo)
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
