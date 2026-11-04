using ProcessAutomation.Common.DTO.Contract;
using ProcessAutomation.Common.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessAutomation.Component
{
    public class IfSnippetSource : ComponentSnippetSourceBase, IComponentSnippetSource
    {
        protected override void AddMethodList()
        {
            AddSnippetMethod(IfConstants.VALIDATE_CONDITION_ALIAS_METHOD, IfConstants.VALIDATE_CONDITION_METHOD);
        }

        public static bool ValidateCondition(string value1, string value2, string dataType, string conditionType)
        {
            bool resultValue = false;
            string conditionFormat = null;
            DataTable resultDataTable = new DataTable();

            if (dataType == "NUMBER")
            {
                switch (conditionType)
                {
                    //EQUAL, DOES NOT EQUAL, GREATER THAN, GREATER THAN OR EQUAL TO, BEGIN WITH, END WITH, CUSTOM (SQL EXPRESSION, IS DEFAULT)
                    case "=":
                        conditionFormat = "{0} = {1}";
                        break;
                    case "<>":
                        conditionFormat = "{0} <> {1}";
                        break;
                    case ">":
                        conditionFormat = "{0} > {1}";
                        break;
                    case ">=":
                        conditionFormat = "{0} >= {1}";
                        break;
                    case "<":
                        conditionFormat = "{0} < {1}";
                        break;
                    case "<=":
                        conditionFormat = "{0} <= {1}";
                        break;
                    default:
                        conditionFormat = "{0} = {1}";
                        break;
                }

                if (conditionFormat != null)
                {
                    resultValue = (bool)resultDataTable.Compute(string.Format(conditionFormat, value1, value2), string.Empty);
                }
            }
            else
            {
                switch (conditionType)
                {
                    //EQUAL, DOES NOT EQUAL, GREATER THAN, GREATER THAN OR EQUAL TO, BEGIN WITH, END WITH, CUSTOM (SQL EXPRESSION, IS DEFAULT)
                    case "=":
                        conditionFormat = "'{0}' = '{1}'";
                        break;
                    case "<>":
                        conditionFormat = "'{0}' <> '{1}'";
                        break;
                    case "CONTAINS":   // "C":
                        conditionFormat = "'{0}' LIKE '%{1}%'";
                        break;
                    case "BEGIN WITH":  // "Az...":
                        conditionFormat = "'{0}' LIKE '{1}%'";
                        break;
                    case "END WITH":    // "...Az":
                        conditionFormat = "'{0}' LIKE '%{1}'";
                        break;
                    default:
                        conditionFormat = "'{0}' = '{1}'";
                        break;
                }

                if (conditionFormat != null)
                {
                    resultValue = (bool)resultDataTable.Compute(string.Format(conditionFormat, value1, value2), string.Empty);
                }
            }

            return resultValue;
        }
    }
}
