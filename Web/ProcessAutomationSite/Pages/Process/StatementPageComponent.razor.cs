using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using ProcessAutomation.Common.DTO.Constants;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.Service;
using ProcessAutomation.Framework.DTO.Info;
using ProcessAutomation.Framework.Manager;
using ProcessAutomationSite.Data;
using ProcessAutomationSite.DTO.Constant;
using ProcessAutomationSite.Service;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace ProcessAutomationSite.Pages.Process
{
    public partial class StatementPageComponent : ComponentBase
    {
        #pragma warning disable CS0414
        private string display = "block";
        private string errorMessage = null;
        private bool isNew = false;
        private bool shouldRender = false;
        #pragma warning restore CS0414

        private const string language = "es";
        private string variable = null;
        private string sentenceResult = null;
        private StatementInfo statementInfo = null;
        private List<StatementInfo> statementList = null;
        private List<ComboBoxInfo> statementTypeList = null;
        private List<StatementInfo> catStatementList = null;

        public class DynamicGrid
        {
            public int Index { get; set; }
            public string Key { get; set; }
            public List<FieldDataInfo> FieldList { get; set; }
        }

        private Dictionary<string, List<DynamicGrid>> DictionaryGrid { get; set; }

        [Inject]
        StateManagement stateManagement { get; set; }

        [CascadingParameter(Name = ProcessAutomationSiteConstant.GLOBAL_DATA)]
        public GlobalDataInfo GlobalData { get; set; }

        [Inject]
        NavigationManager navigation { get; set; }

        [Inject]
        protected IJSRuntime JsRuntime { get; set; }

        public StatementPageComponent()
        {
            statementInfo = new StatementInfo();
        }

        protected override void OnInitialized()
        {
            statementList = new List<StatementInfo>();
            LoadStatementType();
            LoadStatementJson();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
        }

        public object ShallowCopy(object o)
        {
            return o?.GetType().GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic)?.Invoke(o, null);
        }

        private void LoadPage()
        {
            //if (GlobalData.ProcessData?.TaskList != null)
            //{
            //}
            //else
            //{
            //}
        }

        private void LoadStatementJson()
        {
            string location = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            string jsonFile = Utilities.GetResourceFileFromDll(Path.Combine(location, CommonConstants.PACKAGE_FOLDER, CommonConstants.COMMONS_DLL), CommonConstants.STATEMENT_FORMAT_JSON_FILE);
            if (!string.IsNullOrEmpty(jsonFile))
            {
                catStatementList = jsonFile.DeserializerObject<List<StatementInfo>>();
            }
        }

        private void AcceptStatement()
        {
            shouldRender = true;
            if (statementInfo.StatementId == 0)
            {
                NewStatement();
            }
            else if (statementInfo != null)
            {
                statementInfo.Format = BuildFormat(statementInfo);
            }

            UpdateSentenceResult();
            statementInfo = new StatementInfo();
            stateManagement.NotifyStateChanged();            
            //navigation.NavigateTo($"/TaskPage");
        }

        private void UpdateSentenceResult()
        {
            sentenceResult = null;
            if (statementList != null)
            {
                string joinSenctences = string.Empty;
                if (statementList.Count > 0)
                {
                    foreach (StatementInfo statement in statementList)
                    {
                        if (!string.IsNullOrWhiteSpace(statement.Format))
                        {
                            joinSenctences += string.Format(" {0} ", statement.Format);
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(variable) && !string.IsNullOrWhiteSpace(joinSenctences))
                {
                    sentenceResult = string.Format("[@{0}][@{{{1}}}]", variable, joinSenctences);
                }
                else if (!string.IsNullOrWhiteSpace(variable))
                {
                    sentenceResult = string.Format("[@{0}]", variable);
                }
                else if (!string.IsNullOrWhiteSpace(joinSenctences))
                {
                    sentenceResult = string.Format("[@{{{0}}}]", joinSenctences);
                }
            }
        }

        private async static void CopyToClipboard(IJSRuntime js, string elementId)
        {
            await js.InvokeAsync<object>("CopyToClipboardWitAlert", elementId);
        }

        private string BuildFormat(StatementInfo statementInfo)
        {
            string format = string.Empty;
            if (statementInfo?.ParameterList != null)
            {
                string parameterValue = string.Empty;
                string fieldValue = string.Empty;
                foreach (FieldDataInfo fieldData in statementInfo.ParameterList)
                {
                    fieldValue = string.Empty;

                    if (fieldData.ControlType == "CHECKBOX")
                    {
                        fieldValue = fieldData.ValueBool ? bool.TrueString : bool.FalseString;
                    }
                    else if (fieldData.Value != null)
                    {
                        fieldValue = fieldData.Value.ToString();
                    }

                    if (fieldData.ControlType == CommonConstants.GRID)
                    {
                        if (fieldData.DefaultValue != null)
                        {
                            List<DynamicGrid> dynamicGridList = null;
                            try
                            {
                                dynamicGridList = GetDynamicGrid(fieldData.Field, null);
                            }
                            catch { }

                            if (dynamicGridList?.Count > 0)
                            {
                                foreach (DynamicGrid dynamicGrid in dynamicGridList)
                                {
                                    if (dynamicGrid?.FieldList.Count > 0)
                                    {
                                        string gridParameterValue = string.Empty;
                                        foreach (FieldDataInfo fieldGridData in dynamicGrid.FieldList)
                                        {
                                            fieldValue = string.Empty;
                                            if (fieldGridData.ExchangeSeparator &&
                                                statementInfo.Separator?.Trim() == fieldGridData?.Value.Trim())
                                            {
                                                fieldValue = statementInfo.Separator;
                                            }
                                            else if (fieldGridData.ControlType == "CHECKBOX")
                                            {
                                                fieldValue = fieldGridData.ValueBool ? bool.TrueString : bool.FalseString;
                                            }
                                            else if (fieldGridData.Value != null)
                                            {
                                                fieldValue = fieldGridData.Value.ToString();
                                            }

                                            if (gridParameterValue == string.Empty)
                                            {
                                                gridParameterValue = fieldValue;
                                            }
                                            else
                                            {
                                                gridParameterValue += fieldValue;
                                            }
                                        }

                                        if (parameterValue == string.Empty)
                                        {
                                            parameterValue = gridParameterValue;
                                        }
                                        else
                                        {
                                            parameterValue += string.Format("{0}{1}", statementInfo.Separator, gridParameterValue);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (parameterValue == string.Empty)
                        {
                            parameterValue = fieldValue;
                        }
                        else
                        {
                            parameterValue += string.Format("{0}{1}", statementInfo.Separator, fieldValue);
                        }
                    }
                }

                format = string.Format("{0}[{1}]{2}", statementInfo.Symbol, parameterValue, statementInfo.Separator);
            }

            return format;
        }

        private void NewStatement()
        {
            shouldRender = true;
            if (statementInfo != null)
            {
                statementInfo.Format = BuildFormat(statementInfo);
            }

            if (!string.IsNullOrWhiteSpace(statementInfo.Format) && statementInfo.StatementId == 0)
            {
                statementInfo.StatementId = 1;
                if (statementList.Count > 0)
                {
                    statementInfo.StatementId = statementList.Max(m => m.StatementId) + 1;
                }
                statementList.Add((StatementInfo)ShallowCopy(statementInfo));
            }
        }

        private void CancelStatement()
        {
            shouldRender = true;
            statementInfo = null;
            statementInfo = new StatementInfo();
            stateManagement.NotifyStateChanged();
            //navigation.NavigateTo($"/TaskPage");
        }

        private void EditStatement(StatementInfo statementInfo)
        {
            if (statementInfo.StatementId > 0)
            {
                this.statementInfo = statementInfo;
            }
        }

        private void DeleteStatement(StatementInfo statementInfo)
        {
            if (statementInfo != null)
            {
                statementList.Remove(statementInfo);
                UpdateSentenceResult();
            }
        }

        private void LoadStatementType()
        {
            statementTypeList = StepBC.RetrieveStatementTypeList();
        }

        private void OnStatementSelect()
        {
            //GlobalData.ProcessFileName = processFile;
            if (!string.IsNullOrEmpty(statementInfo.Statement))
            {
                StatementInfo statement = catStatementList.FirstOrDefault(f => f.Statement == statementInfo.Statement);
                if (statement != null)
                {
                    statementInfo = (StatementInfo)ShallowCopy(statement);
                    shouldRender = true;
                    stateManagement.NotifyStateChanged();
                }
            }            
        }

        private bool ConvertToBool(string value)
        {
            bool result = false;

            if (bool.TryParse(value, out result))
            {
            }

            return result;
        }

        private List<ComboBoxInfo> LoadComboValue(object optionValue, bool insertEmpty)
        {
            List<ComboBoxInfo> comboBoxList = null;

            if (optionValue != null && !string.IsNullOrWhiteSpace(optionValue.ToString()))
            { 
                comboBoxList = new List<ComboBoxInfo>();

                try
                {
                    comboBoxList = optionValue.ToString().DeserializerObject<List<ComboBoxInfo>>();
                }
                catch { }

                if (comboBoxList?.Count == 0)
                {
                    foreach (string item in optionValue.ToString().DeserializerObject<string[]>())
                    {
                        comboBoxList.Add(new ComboBoxInfo(item, item));
                    }
                }

                if (insertEmpty)
                {
                    comboBoxList.Insert(0, new ComboBoxInfo(string.Empty, string.Empty));
                }
            }

            return comboBoxList;
        }

        private List<DynamicGrid> GetDynamicGrid(string field, dynamic value = null)
        {
            List<DynamicGrid> dynamicGridList = new List<DynamicGrid>();

            if (!string.IsNullOrWhiteSpace(field))
            {
                if (DictionaryGrid == null || DictionaryGrid.Count == 0)
                {
                    DictionaryGrid = new Dictionary<string, List<DynamicGrid>>();
                }

                if (DictionaryGrid.ContainsKey(field))
                {
                    dynamicGridList = DictionaryGrid[field];
                }
                else
                {
                    if (value != null)
                    {
                        int newIndex = 1;
                        if (dynamicGridList.Count > 0)
                        {
                            newIndex = dynamicGridList.Max(m => m.Index) + 1;
                        }

                        List<FieldDataInfo> fieldList = CreateValueForDataList(value.ToString());
                        dynamicGridList.Add(new DynamicGrid()
                        {
                            Index = newIndex,
                            Key = string.Format("{0}_{1}", field, newIndex),
                            FieldList = fieldList
                        });

                        DictionaryGrid.Add(field, dynamicGridList);
                    }
                }
            }

            return dynamicGridList;
        }

        private List<FieldDataInfo> CreateValueForDataList(string value)
        {
            List<FieldDataInfo> fieldList = new List<FieldDataInfo>();

            if (!string.IsNullOrWhiteSpace(value))
            {
                try
                {
                    fieldList = value.DeserializerObject<List<FieldDataInfo>>();
                }
                catch{}
            }

            return fieldList;
        }

        protected void NewParameter(FieldDataInfo fieldDataInfo)
        {
            if (fieldDataInfo != null && !string.IsNullOrEmpty(fieldDataInfo.Field))
            {
                List<DynamicGrid> dynamicGridList = GetDynamicGrid(fieldDataInfo.Field);
                if (dynamicGridList != null)
                {
                    int newIndex = 1;
                    if (dynamicGridList.Count > 0)
                    {
                        newIndex = dynamicGridList.Max(m => m.Index) + 1;
                    }

                    List<FieldDataInfo> fieldList = CreateValueForDataList(fieldDataInfo.DefaultValue.ToString());
                    dynamicGridList.Add(new DynamicGrid()
                    {
                        Index = newIndex,
                        Key = string.Format("{0}_{1}", fieldDataInfo.Field, newIndex),
                        FieldList = fieldList
                    });

                    shouldRender = true;
                    stateManagement.NotifyStateChanged();
                }
            }
        }

        private void DeleteParameter(string field, DynamicGrid dynamicGrid)
        {
            List<DynamicGrid> dynamicGridList = GetDynamicGrid(field);
            if (dynamicGridList != null)
            {
                dynamicGridList.Remove(dynamicGrid);
            }
        }       

        protected override void OnParametersSet()
        {
        }

        protected override bool ShouldRender()
        {
            return base.ShouldRender();
        }

    }
}
