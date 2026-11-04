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

namespace ProcessAutomationMAUI.ViewModel
{
    [QueryProperty("ComponentData", "component")]
    public partial class ComponentVM : ViewModelBase, IValueVM
    {
        [ObservableProperty]
        ComponentModel componentData;

        [ObservableProperty]
        private List<ComboBoxInfo> componentTypeList;

        [ObservableProperty]
        private List<ComboBoxInfo> typeValueTypeList;

        //private readonly ProcessApiAdapter apiAdapter;
        private readonly StoreDbContext dbContext;

        private object inputValueObject;
        private object outputValueObject;

        public ComponentVM(StoreDbContext dbContext)
        {
            this.dbContext = dbContext;
            //this.apiAdapter = apiAdapter;

            ComponentTypeList = dbContext.CAT_COMPONENT.Select(s => new ComboBoxInfo(s.Component, s.Component_Code)).ToList();
            ComponentTypeList.Insert(0, new ComboBoxInfo("", ""));

            TypeValueTypeList = dbContext.CAT_VALUE_TYPE.Select(s => new ComboBoxInfo(s.Value_Type, s.Value_Type_Code)).ToList();
            TypeValueTypeList.Insert(0, new ComboBoxInfo("", ""));

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
                if (componentData == null)
                {
                    componentData = new ComponentModel();
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
            if (WeakReferenceMessenger.Default.IsRegistered<ValueMessage>(this))
            {
                WeakReferenceMessenger.Default.Cleanup();
                WeakReferenceMessenger.Default.Unregister<ValueMessage>(this);
            }
        }

        [RelayCommand(CanExecute = nameof(IsNotBusy))]
        private async void ConfigValueByFormat(ComponentModel component)
        {
            ShowBusy();
            await Task.Delay(100);

            outputValueObject = null;
            inputValueObject = component;
            ComponentInfo componentInfo = component.ToTransformObject<ComponentInfo>();

            await Shell.Current.Navigation.PushModalAsync(new InputValuePage(new InputValueVM(
                this,
                new BuildInputFormatDataInfo
                {
                    Value = component.Value != null ? Utilities.EncodeBase64(component.Value) : null,
                    ValueAction = ValueFormatEnum.COMPONENT.ToString(),
                    Component = component.Name,
                    Description = component.Description,
                    VariableList = null,
                    Language = CultureInfo.CurrentCulture.Name,
                    FormatFile = CommonConstants.INPUT_FIELDS_FILE,
                    ComponentData = componentInfo,
                    RootComponentFolder = null,
                    EnabledOptions = false
                }, dbContext)));

            HideBusy();
        }

        [RelayCommand]
        private async Task Accept()
        {
            UnregisterMessenger();

            await Shell.Current.GoToAsync("..", true,
                        new Dictionary<string, object>
                        {
                            { "component", componentData}
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
            //        if (r.GetType() == typeof(ComponentVM))
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
