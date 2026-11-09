using ProcessAutomation.Common.DTO.Contract;
using System.Collections.Generic;

namespace ProcessAutomation.Common.DTO.Info
{
    public class RunProcessInfo
    {
        public int EncapsulatedId { get; private set; }
        public string EncapsulatedPartitionType { get; private set; }
        public string Variable { get; private set; }
        public bool IsPublicVariable { get; private set; }
        public string ProcessName { get; private set; }
        public bool ProcessEnabled { get; private set; }
        public int StepNumber { get; private set; }
        public string StepDescription { get; private set; }        
        public int TaskNumber { get; private set; }
        public string TaskName { get; private set; }
        public int TaskDelay { get; set; }
        public bool TaskEnabled { get; private set; }
        public int TaskLegacyNumber { get; private set; }
        public int StepLegacyNumber { get; private set; }
        public string? ComponentValue { get; private set; }
        /// <summary>
        /// Step variable value unchanged from step  
        /// </summary>
        public string Value { get; private set; }
        /// <summary>
        /// This is the user input value
        /// </summary>
        public string InputValue { get; set; }
        public string InputValueType { get; private set; }
        public string ValueAction { get; private set; }
        public bool ApplyFormula { get; private set; }
        public string OutputValueType { get; private set; }
        public string OutputResultOption { get; private set; }
        //public List<ValueOptionInfo> OutputValueOption { get; private set; }
        public string ValueTypeDeclaration { get; private set; }
        public int StepDelay { get; private set; }
        public bool Required { get; private set; }
        public bool Async { get; private set; }
        public bool Log { get; private set; }
        public bool Disable { get; private set; }
        public List<ValueOptionInfo> ValueOptionList { get; private set; }
        public List<object> ParameterList { get; set; }      
        public ComponentInfo? ComponentInfo { get; private set; }
        public ITaskComponent ComponentInstance { get; private set; }
        public IComponentService ComponentService { get; private set; }
        public IComponentDTO ComponentDTO { get; private set; }
        public IComponentSnippetSource ComponentSnippetSource { get; private set; }

        public RunProcessInfo(int encapsulatedId, string encapsulatedPartitionType, string value, string inputValue, string processName, bool processEnabled, int stepNumber, int taskNumber, string taskName, int taskDelay, bool taskEnabled,
            int taskLegacyNumber, int stepLegacyNumber, string stepDescription, string variable, bool isPublicVariable, string inputValueType,
            string valueAction, bool applyFormula, string outputValueType, string outputResultOption, string valueTypeDeclaration, int stepDelay, bool required, bool async,
            List<ValueOptionInfo> valueOptionList, List<object> parameterList, bool log, bool disable, ComponentInfo componentInfo,
            ITaskComponent componentInstance, IComponentService componentService, IComponentDTO componentDTO, IComponentSnippetSource componentSnippetSource)
        {
            this.EncapsulatedId = encapsulatedId;
            this.EncapsulatedPartitionType = encapsulatedPartitionType;
            this.Value = value;
            this.InputValue = inputValue;
            this.ProcessName = processName;
            this.ProcessEnabled = processEnabled;
            this.StepNumber = stepNumber;
            this.TaskNumber = taskNumber;
            this.TaskName = taskName;
            this.TaskDelay = taskDelay;
            this.TaskEnabled = taskEnabled;
            this.TaskLegacyNumber = taskLegacyNumber;
            this.StepLegacyNumber = stepLegacyNumber;
            this.StepDescription = stepDescription;
            this.Variable = variable;
            this.IsPublicVariable = isPublicVariable;
            this.InputValueType = inputValueType;
            this.ValueAction = valueAction;
            this.ApplyFormula = applyFormula;
            this.OutputValueType = outputValueType;
            this.OutputResultOption = outputResultOption;
            this.ValueTypeDeclaration = valueTypeDeclaration;
            this.StepDelay = stepDelay;
            this.Required = required;
            this.Async = async;
            this.ValueOptionList = valueOptionList;
            this.ParameterList = parameterList;
            //this.Event = eventName;
            this.Log = log;
            this.Disable = disable;
            //this.EventList = eventList;
            this.ComponentValue = componentInfo?.Value;
            this.ComponentInfo = componentInfo;
            this.ComponentInstance = componentInstance;
            this.ComponentService = componentService;
            this.ComponentDTO = componentDTO;
            this.ComponentSnippetSource = componentSnippetSource;
        }
    }
}
