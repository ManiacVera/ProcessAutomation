using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using ProcessAutomation.Api.Adapter.Service;
using ProcessAutomation.Common.DTO.Constants;
using ProcessAutomation.Common.DTO.Enum;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.Service;
using ProcessAutomation.Framework.DTO.Info;
using ProcessAutomationMAUI.Base;
using ProcessAutomationMAUI.DataAccess;
using ProcessAutomationMAUI.DTO.Model;
using ProcessAutomationMAUI.Messages;
using ProcessAutomationMAUI.Pages.Process;

namespace ProcessAutomationMAUI.ViewModel
{
    [QueryProperty("ProcessData", "process")]
    [QueryProperty("TaskData", "task")]
    [QueryProperty("StepData", "step")]
    [QueryProperty("ComponentData", "component")]
    public partial class TaskVM : ViewModelBase, IValueVM
    {
        [ObservableProperty]
        public ProcessModel processData;

        [ObservableProperty]
        public TaskModel taskData;

        [ObservableProperty]
        public StepModel stepData;

        [ObservableProperty]
        public ComponentModel componentData;

        [ObservableProperty]
        public ObservableCollection<StepModel> stepList;

        [ObservableProperty]
        public ObservableCollection<ComponentModel> taskComponentList;

        [ObservableProperty]
        private ObservableCollection<ComboBoxModel> componentList;

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
        private bool visibleOptionStep;

        [ObservableProperty]
        private bool visibleOptionComponent;

        [ObservableProperty]
        private bool visibleOptionStatement;

        [ObservableProperty]
        private bool isOpenComponentList;

        [ObservableProperty]
        private ComboBoxModel componentItem;

        private StepModel currentStep;
        public CollectionView stepCollectionView;
        private object inputValueObject;
        private object outputValueObject;
        private StepModel itemBeingDragged;
        private readonly StoreDbContext dbContext;
        private readonly ProcessApiAdapter apiAdapter;

        public TaskVM(StoreDbContext dbContext, ProcessApiAdapter apiAdapter)
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

            VisibleOptionStep = true;
            VisibleOptionComponent = false;
            VisibleOptionStatement = false;

            RegisterMessenger();
        }

        //private void StepList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        //{
        //    if (e.OldItems != null)
        //        foreach (var item in e.OldItems)
        //            if (item != null && item is INotifyPropertyChanged i)
        //                i.PropertyChanged -= StepInfo_PropertyChanged;

        //    if (e.NewItems != null)
        //    {
        //        foreach (var item in e.NewItems)
        //        {
        //            if (item != null && item is INotifyPropertyChanged itemProperty)
        //            {
        //                itemProperty.PropertyChanged -= StepInfo_PropertyChanged;
        //                itemProperty.PropertyChanged += StepInfo_PropertyChanged;
        //            }
        //        }
        //    }
        //}

        //private void StepInfo_PropertyChanged(object sender, PropertyChangedEventArgs e)
        //{
        //    if (e.PropertyName == "MyProperty")
        //    { 
        //    }
        //}

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

        private bool ValidateTask(TaskModel task)
        {
            string message = string.Empty;
            if (task == null)
            {
                message = string.Format("{0}{1}", "La tarea no puede ser null", Environment.NewLine);
            }
            else
            {
                if (task.Number <= 0)
                {
                    message += string.Format("{0}{1}", "El número de tarea debe de ser mayor de 0", Environment.NewLine);
                }

                if (string.IsNullOrWhiteSpace(task.Name))
                {
                    message += string.Format("{0}{1}", "Falta capturar el nombre de la tarea", Environment.NewLine);
                }

                if (task.StepList == null || task.StepList.Count == 0)
                {
                    message += string.Format("{0}{1}", "Falta capturar pasos", Environment.NewLine);
                }

                if (task.ComponentList == null || task.ComponentList.Count == 0)
                {
                    message += string.Format("{0}{1}", "Falta capturar componentes", Environment.NewLine);
                }
            }

            if (message != string.Empty)
            {
                Shell.Current.DisplayAlertAsync("PROCESS AUTOMATION", message.TrimEnd(), "ACEPTAR");
            }

            return (message == string.Empty);
        }

        private void LoadComponentList()
        {
            if (TaskComponentList?.Count > 0)
            {
                //List<ComboBoxModel> componentSelectedList = StepList.Select(s => s.ComponentItem).ToList();
                List<string> componentSelectedList = StepList.Select(s => s.Component).ToList();

                ComponentList = TaskComponentList.Select(s => new ComboBoxModel(s.Name, s.Name)).ToObservableCollection();
                ComponentList.Insert(0, new ComboBoxModel("", ""));

                //if (ComponentList == null)
                //{
                //    ComponentList = TaskComponentList.Select(s => new ComboBoxInfo(s.Name, s.Name)).ToObservableCollection();
                //    ComponentList.Insert(0, new ComboBoxInfo("", ""));
                //}
                //else
                //{
                //    var newComponentList = TaskComponentList.Where(f => !ComponentList.Any(a => a.Text == f.Name));
                //    foreach (var newComponent in newComponentList)
                //    {
                //        ComponentList.Add(new ComboBoxInfo(newComponent.Name, newComponent.Name));
                //    }

                //    var allComponentList = TaskComponentList.Select(s => new ComboBoxInfo(s.Name, s.Name));
                //    var removeComponentList = ComponentList.Where(f => !allComponentList.Any(a => a.Text == f.Text || string.IsNullOrWhiteSpace(a.Text))).ToList();
                //    foreach (ComboBoxInfo componentRemove in removeComponentList)
                //    {
                //        ComponentList.Remove(componentRemove);
                //    }

                //    ComponentList.Insert(0, new ComboBoxInfo("", ""));
                //}


                //List<ComboBoxModel> componentListAux = ComponentList.ToList();
                //for (int countCombo = 0; countCombo <= StepList.Count - 1; countCombo++)
                //{
                //    if (componentSelectedList[countCombo] != null)
                //    {
                //        StepList[countCombo].ComponentIndex = -1;
                //        StepList[countCombo].Component = null;

                //        StepList[countCombo].Component = componentSelectedList[countCombo];
                //        StepList[countCombo].ComponentIndex = componentListAux.FindIndex(i => i.Text == componentSelectedList[countCombo]);
                //        //StepList[countCombo].ComponentItem = null;
                //        //StepList[countCombo].ComponentItem = componentListAux.FirstOrDefault(i => i.Text == componentSelectedList[countCombo].Text);
                //    }
                //}
            }
        }

        private void ReorderStepList()
        {
            int stepCount = 1;
            foreach (StepModel model in StepList)
            {
                model.Number = stepCount++;
            }

            //List<string> componentSelectedList = StepList.Select(s => s.Component).ToList();
            //ComponentList = TaskComponentList.Select(s => new ComboBoxModel(s.Name, s.Name)).ToObservableCollection();
            //ComponentList.Insert(0, new ComboBoxModel("", ""));

            //List<ComboBoxModel> componentListAux = ComponentList.ToList();
            //for (int countCombo = 0; countCombo <= StepList.Count - 1; countCombo++)
            //{
            //    if (componentSelectedList[countCombo] != null)
            //    {
            //        StepList[countCombo].ComponentIndex = -1;
            //        StepList[countCombo].Component = null;

            //        StepList[countCombo].Component = componentSelectedList[countCombo];
            //        StepList[countCombo].ComponentIndex = componentListAux.FindIndex(i => i.Text == componentSelectedList[countCombo]);
            //    }
            //}
        }

        private List<string> RetrieveStepVariableList(int stepNumber)
        {
            List<string> variableList = new List<string>();

            if (processData != null)
            {
                variableList.AddRange(taskData.StepList
                    .Where(f => f.Number < stepNumber && !string.IsNullOrWhiteSpace(f.Variable))
                    .OrderBy(o => o.Variable)
                    .Select(step => step.Variable).ToList());

                variableList.AddRange(processData.TaskList.Where(task => task.Number < taskData.Number)
                    .OrderByDescending(o => o.Number)
                    .SelectMany(s => s.StepList.Select(sl => new { Variable = sl.Variable, TaskName = s.Name, IsPublicVariable = sl.IsPublicVariable }))
                    .Where(f => f.IsPublicVariable && !string.IsNullOrWhiteSpace(f.Variable))
                    .Select(step => string.Format("{0},{1}", step.Variable, step.TaskName)).ToList());
            }


            return variableList.Distinct().ToList();
        }

        [RelayCommand]
        void Appearing()
        {
            try
            {
                //MainThread.BeginInvokeOnMainThread(() => 
                //{
                if (StepList == null)
                {
                    if (TaskData.StepList == null)
                    {
                        TaskData.StepList = new List<StepModel>();
                    }

                    if (TaskData.ComponentList == null)
                    {
                        TaskData.ComponentList = new List<ComponentModel>();
                    }

                    StepList = TaskData.StepList.ToObservableCollection();
                    TaskComponentList = TaskData.ComponentList.ToObservableCollection();

                    LoadComponentList();

                    //StepList.CollectionChanged -= StepList_CollectionChanged;
                    //StepList.CollectionChanged += StepList_CollectionChanged;
                }

                if (stepData != null)
                {
                    if (StepList == null)
                    {
                        StepList = new ObservableCollection<StepModel>();
                    }

                    StepModel step = StepList.FirstOrDefault(f => f.Number == stepData.Number);
                    if (step != null)
                    {
                        step = stepData;
                    }
                    else
                    {
                        StepList.Add(stepData);
                    }
                    StepData = null;
                }

                if (componentData != null)
                {
                    if (taskComponentList == null)
                    {
                        TaskComponentList = new ObservableCollection<ComponentModel>();
                    }

                    int componentIndex = TaskComponentList.IndexOf(componentData);
                    if (componentIndex != -1)
                    {
                        TaskComponentList[componentIndex] = componentData;
                    }
                    else
                    {
                        TaskComponentList.Add(componentData);
                    }
                    ComponentData = null;
                }

                ComponentData = null;
                StepData = null;

                //});
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

        [RelayCommand]
        private async void ComponentPickerSelected(StepModel step)
        {
            if (step.ComponentIndex != -1)
            {
                step.Component = componentList[step.ComponentIndex].Text;
            }
            else
            {
                step.Component = null;
            }

            await Task.CompletedTask;
        }

        [RelayCommand]
        private void IsVisibleOption(string option)
        {
            VisibleOptionStep = false;
            VisibleOptionComponent = false;
            VisibleOptionStatement = false;

            switch (option) 
            {
                case "STEP":
                    {
                        LoadComponentList();
                        VisibleOptionStep = true;
                    }
                    break;

                case "COMPONENT":
                    VisibleOptionComponent = true;
                    break;

                case "STATEMENT":
                    VisibleOptionStatement = true;
                    break;
            }
        }

        [RelayCommand]
        private async void AcceptTask()
        {
            ShowBusy();
            await Task.Delay(100);

            UnregisterMessenger();

            TaskData.StepList = StepList.ToList();
            TaskData.ComponentList = TaskComponentList.ToList();

            if (ValidateTask(TaskData))
            {
                await Shell.Current.GoToAsync("..", true,
                    new Dictionary<string, object>
                    {
                    { "task", TaskData}
                    });
            }

            HideBusy();
        }

        [RelayCommand]
        private async void CancelTask()
        {
            UnregisterMessenger();

            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private async Task StepAdd()
        {
            int stepNumber = StepList?.Count == 0 ? 1 : StepList.Max(m => m.Number) + 1;
            
            if (DeviceInfo.Idiom == DeviceIdiom.Desktop)
            {
                StepList.Add(new StepModel
                {
                    Number = stepNumber,
                    Required = true,
                });
            }
            else
            {
                ShowBusy();
                await Task.Delay(100);
                await Shell.Current.GoToAsync("StepPage", true, new Dictionary<string, object>
                {
                    ["step"] = new StepModel { Number = stepNumber },
                    ["componentList"] = taskComponentList?.ToList(),
                    ["variableList"] = RetrieveStepVariableList(stepNumber)
                });
                HideBusy();
            }
        }

        [RelayCommand]
        private async Task StepEdit(StepModel step)
        {
            ShowBusy();
            await Task.Delay(100);

            await Shell.Current.GoToAsync("StepPage", true, new Dictionary<string, object>
            {
                ["step"] = step,
                ["componentList"] = taskComponentList?.ToList(),
                ["variableList"] = RetrieveStepVariableList(step.Number)
            });
            HideBusy();
        }

        [RelayCommand]
        private async void StepDelete(StepModel step)
        {
            bool result = await Shell.Current.DisplayAlertAsync("PROCESS AUTOMATION", "¿Estas seguro de borrar el paso?", accept: "Aceptar", cancel: "Cancelar");

            if (result)
            {
                await Task.Run(() =>
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        StepList.Remove(step);
                    });
                });
            }
        }

        [RelayCommand]
        private async void ComponentAdd()
        {
            if (DeviceInfo.Idiom == DeviceIdiom.Desktop)
            {
                TaskComponentList.Add(new ComponentModel());
            }
            else
            {
                ShowBusy();
                await Task.Delay(100);

                await Shell.Current.GoToAsync("ComponentPage", true, new Dictionary<string, object>
                {
                    ["component"] = new ComponentModel()
                });
                HideBusy();
            }
        }

        [RelayCommand]
        private async void ComponentEdit(ComponentModel component)
        {
            ShowBusy();
            await Task.Delay(100);
            await Shell.Current.GoToAsync("ComponentPage", true, new Dictionary<string, object>
            {
                ["component"] = component
            });
            HideBusy();
        }

        [RelayCommand]
        private async void ComponentDelete(ComponentModel component)
        {
            bool result = await Shell.Current.DisplayAlertAsync("PROCESS AUTOMATION", "¿Estas seguro de borrar el componente?", accept: "Aceptar", cancel: "Cancelar");

            if (result)
            {
                await Task.Run(() =>
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        TaskComponentList.Remove(component);
                    });
                });
            }
        }

        [RelayCommand(CanExecute = nameof(IsNotBusy))]
        private async void ValueByFormat(StepModel step)
        {
            ShowBusy();
            await Task.Delay(100);

            outputValueObject = null;
            inputValueObject = step;
            //ComponentModel component = taskComponentList.FirstOrDefault(s => s.Name == step.ComponentItem?.Text);
            ComponentModel component = taskComponentList.FirstOrDefault(s => s.Name == step.Component);

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
                        VariableList = RetrieveStepVariableList(step.Number),
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

            //valueOptionList = apiAdapter.RetrieveOutputValueOption(outputDictionary);
            valueOptionList = StepExpressBC.RetrieveOutputValueOptionList(outputDictionary);
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
        private void ItemDragged(StepModel step)
        {
            Debug.WriteLine($"ItemDragged : {step?.Description}");
            step.IsBeingDragged = true;
            itemBeingDragged = step;
        }

        [RelayCommand]
        private void ItemDragLeave(StepModel step)
        {
            Debug.WriteLine($"ItemDragLeave : {step?.Description}");
            step.IsBeingDraggedOver = false;
        }

        [RelayCommand]
        private void ItemDraggedOver(StepModel step)
        {
            Debug.WriteLine($"ItemDraggedOver : {step?.Description}");
            if (step == itemBeingDragged)
            {
                step.IsBeingDragged = false;
            }
            step.IsBeingDraggedOver = step != itemBeingDragged;
        }

        [RelayCommand]
        private void ItemDropped(StepModel step)
        {
            try
            {
                var itemToMove = itemBeingDragged;
                var itemToInsertBefore = step;
                if (itemToMove == null || itemToInsertBefore == null || itemToMove == itemToInsertBefore)
                    return;
                int insertAtIndex = StepList.IndexOf(itemToInsertBefore);
                if (insertAtIndex >= 0 && insertAtIndex < StepList.Count)
                {
                    StepList.Remove(itemToMove);
                    StepList.Insert(insertAtIndex, itemToMove);
                    itemToMove.IsBeingDragged = false;
                    itemToInsertBefore.IsBeingDraggedOver = false;

                    ReorderStepList();
                }
                Debug.WriteLine($"ItemDropped: [{itemToMove?.Description}] => [{itemToInsertBefore?.Description}], target index = [{insertAtIndex}]");
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        [RelayCommand]
        private async Task ComponentTap(StepModel step)
        {
            IsOpenComponentList = true;
            await Task.Run(() =>
            {
                ComponentItem = componentList.FirstOrDefault(f => f.Text == step.Component);
                currentStep = step;
            });
        }

        //[RelayCommand]
        //private async Task ComponentListPickerSelected(object sender)
        //{
        //    if (currentStep != null)
        //    {
        //        await Task.Run(() =>
        //        {
        //            currentStep.ComponentIndex = componentIndex;
        //            currentStep.Component = componentList[componentIndex].Text;
        //        });
        //    }
        //    IsOpenComponentList = false;
        //}

        [RelayCommand]
        private async Task ComponentListViewItemTapped(ComboBoxModel combo)
        {
            if (currentStep != null)
            {
                await Task.Run(() =>
                {
                    currentStep.ComponentIndex = ComponentList.IndexOf(combo);
                    currentStep.Component = combo.Text;
                });
            }
            ComponentItem = null;
            IsOpenComponentList = false;
        }

        private void RegisterMessenger()
        {
            //if (!WeakReferenceMessenger.Default.IsRegistered<ValueMessage>(this))
            //{
            //    WeakReferenceMessenger.Default.Register<ValueMessage>(this, (r, m) =>
            //    {
            //        if (r.GetType() == typeof(TaskVM))
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
