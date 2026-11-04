using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.EntityFrameworkCore;
using ProcessAutomation.Api.Adapter.Service;
using ProcessAutomation.Common.DTO.Constants;
using ProcessAutomation.Common.DTO.Enum;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Framework.DTO.Info;
using ProcessAutomationMAUI.Base;
using ProcessAutomationMAUI.DataAccess;
using ProcessAutomationMAUI.DTO.Model;
using ProcessAutomationMAUI.Messages;
using ProcessAutomationMAUI.Pages.Process;
using System.Globalization;
using System.Text.Json;

namespace ProcessAutomationMAUI.ViewModel
{
    [QueryProperty("StepData", "step")]
    [QueryProperty("ComponentListData", "componentList")]
    [QueryProperty("VariableListData", "variableList")]
    public partial class StepVM : ViewModelBase, IValueVM
    {
        [ObservableProperty]
        StepModel stepData;

        [ObservableProperty]
        private List<ComponentModel> componentListData;

        [ObservableProperty]
        private List<string> variableListData;

        [ObservableProperty]
        private List<ComboBoxInfo> componentTypeList;

        [ObservableProperty]
        private List<ComboBoxInfo> encapsulatedList;

        [ObservableProperty]
        private List<ComboBoxInfo> typeValueTypeList;

        [ObservableProperty]
        private List<ComboBoxInfo> valueOutputTypeList;

        [ObservableProperty]
        private List<ComboBoxInfo> typeValueOptionList;

        [ObservableProperty]
        private List<ComboBoxModel> componentList;

        private readonly ProcessApiAdapter apiAdapter;
        private readonly StoreDbContext dbContext;
        
        private object inputValueObject;
        private object outputValueObject;

        public StepVM(StoreDbContext dbContext, ProcessApiAdapter apiAdapter)
        {
            this.dbContext = dbContext;
            this.apiAdapter = apiAdapter;

            ComponentTypeList = dbContext.CAT_COMPONENT.Select(s => new ComboBoxInfo(s.Component, s.Component_Code)).ToList();
            ComponentTypeList.Insert(0, new ComboBoxInfo("", ""));

            EncapsulatedList = dbContext.CAT_ENCAPSULATED_PARTITION_TYPE.Select(s => new ComboBoxInfo(s.Encapsulated_Partition_Type, s.Encapsulated_Partition_Type_Code)).ToList();
            EncapsulatedList.Insert(0, new ComboBoxInfo("", ""));

            TypeValueTypeList = dbContext.CAT_VALUE_TYPE.Select(s => new ComboBoxInfo(s.Value_Type, s.Value_Type_Code)).ToList();
            TypeValueTypeList.Insert(0, new ComboBoxInfo("", ""));

            ValueOutputTypeList = dbContext.CAT_VALUE_OUTPUT_TYPE.Select(s => new ComboBoxInfo(s.Value_Output_Type, s.Value_Output_Type_Code)).ToList();
            ValueOutputTypeList.Insert(0, new ComboBoxInfo("", ""));

            TypeValueOptionList = dbContext.CAT_DECLARATION_TYPE.Select(s => new ComboBoxInfo(s.Declaration_Type, s.Declaration_Type_Code)).ToList();
            TypeValueOptionList.Insert(0, new ComboBoxInfo("", ""));

            RegisterMessenger();
        }

        public void ValueMessageReceived(string value)
        {
            if (inputValueObject != null)
            {
                if (inputValueObject.GetType() == typeof(StepModel))
                {
                    ((StepModel)inputValueObject).Value = value;
                }
                else if (inputValueObject.GetType() == typeof(ComponentModel))
                {
                    ((ComponentModel)inputValueObject).Value = value;
                }

            }
            else if (outputValueObject?.GetType() == typeof(StepModel))
            {
                ((StepModel)outputValueObject).OutputResultOption = value;
            }

            outputValueObject = null;
            inputValueObject = null;
        }

        [RelayCommand]
        void Appearing()
        {
            try
            {
                if (stepData == null)
                {   
                    stepData = new StepModel();
                }

                if (componentList == null || componentList.Count == 0)
                {
                    ComponentList = ComponentListData?.Select(s => new ComboBoxModel(s.Name, s.Name)).ToList();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        [RelayCommand]
        void Disappearing()
        {
            try
            {
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
        }

        [RelayCommand]
        void BackButtonPressed()
        {
            UnregisterMessenger();
        }

        [RelayCommand(CanExecute = nameof(IsNotBusy))]
        private async void ValueByFormat(StepModel step)
        {
            ShowBusy();
            await Task.Delay(100);

            outputValueObject = null;
            inputValueObject = step;
            ComponentModel component = componentListData.FirstOrDefault(s => s.Name == step.Component);

            if (component != null)
            {
                ComponentInfo componentInfo = component.ToTransformObject<ComponentInfo>();
                await Shell.Current.Navigation.PushModalAsync(new InputValuePage(new InputValueVM(
                    this,
                    new BuildInputFormatDataInfo
                    {
                        Value = step.Value != null ? Utilities.EncodeBase64(step.Value) : null,
                        ValueAction = ValueFormatEnum.STEP.ToString(),
                        Component = step.Component,
                        Description = step.Description,
                        VariableList = variableListData,
                        Language = CultureInfo.CurrentCulture.Name,
                        FormatFile = CommonConstants.INPUT_FIELDS_FILE,
                        ComponentData = componentInfo,
                        RootComponentFolder = null,
                        EnabledOptions = true
                    }, dbContext)));
            }

            HideBusy();
        }

        [RelayCommand(CanExecute = nameof(IsNotBusy))]
        private async void OptionValueByFormat(StepModel step)
        {
            ShowBusy();
            await Task.Delay(100);

            inputValueObject = null;
            outputValueObject = step;
            Dictionary<string, object> outputDictionary = null;
            List<OutputResultOptionInfo> valueOptionList = null;
            string value = null;

            if (step.OutputResultOption != null)
            {
                outputDictionary = JsonSerializer.Deserialize<Dictionary<string, object>>(step.OutputResultOption);
            }

            if (outputDictionary == null)
            {
                outputDictionary = new Dictionary<string, object>();
            }

            valueOptionList = apiAdapter.RetrieveOutputValueOption(outputDictionary);
            value = JsonSerializer.Serialize(valueOptionList);

            await Shell.Current.Navigation.PushModalAsync(new OutputValuePage(new OutputValueVM(
                this,
                new BuildOutputFormatDataInfo
                {
                    Value = value != null ? value : null,
                    Token = "TOKEN",
                    Component = step.Component,
                    Description = step.Description,
                    Language = CultureInfo.CurrentCulture.Name,
                    EnabledOptions = false
                })));

            HideBusy();
        }

        [RelayCommand]
        private async Task Accept()
        {
            UnregisterMessenger();

            await Shell.Current.GoToAsync("..", true,
                        new Dictionary<string, object>
                        {
                            { "step", StepData}
                        });
        }

        [RelayCommand]
        private async Task Cancel()
        {
            UnregisterMessenger();

            await Shell.Current.GoToAsync("..", true);
        }

        private void RegisterMessenger()
        {
            //if (!WeakReferenceMessenger.Default.IsRegistered<ValueMessage>(this))
            //{
            //    WeakReferenceMessenger.Default.Register<ValueMessage>(this, (r, m) =>
            //    {
            //        if (r.GetType() == typeof(StepVM))
            //        {
            //            ValueMessageReceived(m.Value);
            //        }
            //    });
            //}
            //else
            //{
            //    WeakReferenceMessenger.Default.Cleanup();
            //}
        }

        private void UnregisterMessenger()
        {
            //if (WeakReferenceMessenger.Default.IsRegistered<ValueMessage>(this))
            //{
            //    WeakReferenceMessenger.Default.Cleanup();
            //    WeakReferenceMessenger.Default.Unregister<ValueMessage>(this);
            //}
        }
    }
}
