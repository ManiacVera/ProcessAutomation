using System.Collections.ObjectModel;
using System.Text;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.Devices;
using ProcessAutomation.Api.Adapter.Service;
using ProcessAutomation.Common.DTO.Constants;
using ProcessAutomation.Common.DTO.Info;
using ProcessAutomation.Common.Service;
using ProcessAutomation.Framework.DTO.Info;
using ProcessAutomation.FrameworkExpress.Helper;
using ProcessAutomationMAUI.Base;
using ProcessAutomationMAUI.DataAccess;
using ProcessAutomationMAUI.DTO.Model;

namespace ProcessAutomationMAUI.ViewModel
{

    [QueryProperty("TaskData", "task")]
    public partial class ProcessVM : ViewModelBase
    {
        [ObservableProperty]
        private ObservableCollection<string> processFileList;

        [ObservableProperty]
        private string processFile;

        [ObservableProperty]
        private TaskModel taskData;

        [ObservableProperty]
        private ObservableCollection<TaskModel> taskList;

        [ObservableProperty]
        private ProcessModel processData;

        private string userCode = "VERA_USER";
        private readonly IFileSaver fileSaver;
        private readonly ProcessApiAdapter apiAdapter;
        private readonly StoreDbContext dbContext;

        public ProcessVM(IFileSaver fileSaver, StoreDbContext dbContext, ProcessApiAdapter apiAdapter)
        {
            ShowBusy();

            this.fileSaver = fileSaver;
            this.dbContext = dbContext;
            this.apiAdapter = apiAdapter;

            this.ProcessData = new ProcessModel();
            TaskList = new ObservableCollection<TaskModel>();
            ReloadFileList();

            HideBusy();
        }

        private void LoadTables()
        {
            dbContext.LoadTable<CAT_COMPONENT>((dbSet) =>
            {
                List<ComboBoxInfo> comboBoxList = apiAdapter.RetrieveComponent();
                if (comboBoxList?.Count > 0)
                {
                    dbSet.AddRange(comboBoxList.Where(f => !string.IsNullOrWhiteSpace(f.Text)).Select(item => new CAT_COMPONENT()
                    {
                        Component = item.Text,
                        Component_Code = item.Value,
                        Creation_Date = DateTime.Now,
                        Modification_Date = DateTime.Now,
                        Active = true
                    }).ToList());

                    dbContext.SaveChanges();

                    return true;
                }
                else
                {
                    return false;
                }
            });

            dbContext.LoadTable<CAT_ENCAPSULATED_PARTITION_TYPE>((dbSet) =>
            {
                List<ComboBoxInfo> comboBoxList = apiAdapter.RetrieveEncapsulatedPartitionType();
                if (comboBoxList?.Count > 0)
                {
                    dbSet.AddRange(comboBoxList.Where(f => !string.IsNullOrWhiteSpace(f.Text)).Select(item => new CAT_ENCAPSULATED_PARTITION_TYPE()
                    {
                        Encapsulated_Partition_Type = item.Text,
                        Encapsulated_Partition_Type_Code = item.Value,
                        Creation_Date = DateTime.Now,
                        Modification_Date = DateTime.Now,
                        Active = true
                    }).ToList());

                    dbContext.SaveChanges();

                    return true;
                }
                else
                {
                    return false;
                }
            });

            dbContext.LoadTable<CAT_VALUE_TYPE>((dbSet) =>
            {
                List<ComboBoxInfo> comboBoxList = apiAdapter.RetrieveValueType();
                if (comboBoxList?.Count > 0)
                {
                    dbSet.AddRange(comboBoxList.Where(f => !string.IsNullOrWhiteSpace(f.Text)).Select(item => new CAT_VALUE_TYPE()
                    {
                        Value_Type = item.Text,
                        Value_Type_Code = item.Value,
                        Creation_Date = DateTime.Now,
                        Modification_Date = DateTime.Now,
                        Active = true
                    }).ToList());

                    dbContext.SaveChanges();

                    return true;
                }
                else
                {
                    return false;
                }
            });

            dbContext.LoadTable<CAT_VALUE_OUTPUT_TYPE>((dbSet) =>
            {
                List<ComboBoxInfo> comboBoxList = apiAdapter.RetrieveValueOutputType();
                if (comboBoxList?.Count > 0)
                {
                    dbSet.AddRange(comboBoxList.Where(f => !string.IsNullOrWhiteSpace(f.Text)).Select(item => new CAT_VALUE_OUTPUT_TYPE()
                    {
                        Value_Output_Type = item.Text,
                        Value_Output_Type_Code = item.Value,
                        Creation_Date = DateTime.Now,
                        Modification_Date = DateTime.Now,
                        Active = true
                    }).ToList());

                    dbContext.SaveChanges();

                    return true;
                }
                else
                {
                    return false;
                }
            });

            dbContext.LoadTable<CAT_DECLARATION_TYPE>((dbSet) =>
            {
                List<ComboBoxInfo> comboBoxList = apiAdapter.RetrieveDeclarationType();
                if (comboBoxList?.Count > 0)
                {
                    dbSet.AddRange(comboBoxList.Where(f => !string.IsNullOrWhiteSpace(f.Text)).Select(item => new CAT_DECLARATION_TYPE()
                    {
                        Declaration_Type = item.Text,
                        Declaration_Type_Code = item.Value,
                        Creation_Date = DateTime.Now,
                        Modification_Date = DateTime.Now,
                        Active = true
                    }).ToList());

                    dbContext.SaveChanges();

                    return true;
                }
                else
                {
                    return false;
                }
            });

            //dbContext.LoadTable<CAT_RESULT_PARAMETER>((dbSet) =>
            //{
            //    List<OutputResultOptionInfo> valueOptionList = apiAdapter.RetrieveOutputValueOption(new Dictionary<string, object>());
            //    if (valueOptionList?.Count > 0)
            //    {
            //        dbSet.AddRange(valueOptionList.Where(f => !string.IsNullOrWhiteSpace(f.ResultParameter)).Select(item => new CAT_RESULT_PARAMETER()
            //        {
            //            Result_Parameter = item.ResultParameter,
            //            Value = item.Value,
            //            Control_Type = string.Empty,
            //            Creation_Date = DateTime.Now,
            //            Modification_Date = DateTime.Now,
            //            Active = true
            //        }).ToList());

            //        dbContext.SaveChanges();

            //       return false;
            //    }
            //    else
            //    {
            //        return false;
            //    }
            //});
        }

        private bool IsValidTask(TaskModel task)
        {
            bool isValid = false;

            if (task != null)
            {
                isValid = !string.IsNullOrWhiteSpace(task.Name) && task.StepList?.Count > 0 && task.ComponentList?.Count > 0
                    && task.Number > 0;
            }

            return isValid;
        }

        private bool ValidateProcess()
        {
            string message = string.Empty;
            if (processData == null)
            {
                message = string.Format("{0}{1}", "El proceso no puede ser null", Environment.NewLine);
            }
            else
            {
                if (processData.Number <= 0)
                {
                    message += string.Format("{0}{1}", "El número de proceso debe de ser mayor de 0", Environment.NewLine);
                }

                if (string.IsNullOrWhiteSpace(processData.ProcessName))
                {
                    message += string.Format("{0}{1}", "Falta capturar el nombre del proceso", Environment.NewLine);
                }

                if (processData.TaskList == null || processData.TaskList.Count == 0)
                {
                    message += string.Format("{0}{1}", "Falta capturar tareas", Environment.NewLine);
                }
            }

            if (message != string.Empty)
            {
                Shell.Current.DisplayAlertAsync("PROCESS AUTOMATION", message.TrimEnd(), "ACEPTAR");
            }

            return (message == string.Empty);
        }

        private void ReloadFileList()
        {
            string processFile = this.processFile;

            List<string> processList = apiAdapter.RetrieveProcessFileList(" ", Path.Combine(userCode, CommonConstants.DEFAULT_FOLDER));
            if (processList?.Count > 0)
            {
                ProcessFileList = processList.ToObservableCollection();
            }

            this.ProcessFile = processFile;
        }

        private ProcessDataInfo CreateProcessDataInfo()
        {
            ProcessDataInfo processInfo = new ProcessDataInfo();
            processInfo.ProcessData = processData.ToTransformObject<ProcessInfo>();

            if (!string.IsNullOrEmpty(ProcessFile))
            {
                processInfo.ProcessFile = Path.GetFileName(ProcessFile);
            }
            else
            {
                processInfo.ProcessFile = processData.ProcessName;
            }

            processInfo.UserCode = userCode;
            return processInfo;
        }

        [RelayCommand]
        void Appearing()
        {
            try
            {
                LoadTables();

                if (IsValidTask(TaskData))
                {
                    TaskModel taskFounded = TaskList.FirstOrDefault(f => f.Number == TaskData.Number);
                    if (taskFounded != null)
                    {
                        taskFounded = CloneHelper.Clone(TaskData);
                    }
                    else
                    {
                        TaskList.Add(TaskData);
                    }

                    ProcessData.TaskList = taskList;
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
        private void ProcessOpen()
        {
            if (!string.IsNullOrEmpty(ProcessFile))
            {
                ProcessInfo process = apiAdapter.OpenProcess(processFile, userCode, " ");
                if (process != null)
                {
                    try
                    {
                        ProcessData.ProcessName = process.ProcessName;
                        ProcessData.Number = process.Number;
                        ProcessData.ProcessDatabase = process.ProcessDatabase;
                        ProcessData.LanguageSettingList = process.LanguageSettingList?.ToObservableCollection();
                        ProcessData.TaskList = process.TaskList.ToTransformObject<ObservableCollection<TaskModel>>();

                        TaskList = ProcessData.TaskList?.ToObservableCollection();
                    }
                    catch (Exception)
                    {
                        Shell.Current.DisplayAlertAsync("PROCESS AUTOMATION", "File open error", "ACCEPT");
                    }
                }
            }
        }

        [RelayCommand]
        private async Task ProcessSave()
        {
            ShowBusy();
            await Task.Delay(100);
            if (ValidateProcess())
            {
                ProcessDataInfo processInfo = CreateProcessDataInfo();
                //string processJson = processInfo.ToJsonSerializer();
                string result = apiAdapter.SaveProcess(processInfo);
                if(!string.IsNullOrEmpty(result))
                {
                    ReloadFileList();
                }
            }
            HideBusy();
        }

        [RelayCommand]
        private async Task ProcessBuild(CancellationToken cancellationToken)
        {
            base.ShowBusy();
            await Task.Delay(100);

            if (ValidateProcess())
            {
                try
                {
                    ProcessDataInfo processInfo = CreateProcessDataInfo();
                    string zipBase64 = apiAdapter.BuildProcess(processInfo);

                    if (!string.IsNullOrEmpty(zipBase64))
                    {
                        ReloadFileList();
                        using var stream = new MemoryStream(Encoding.Default.GetBytes(Utilities.DecodeBase64(zipBase64)));
                        if (DeviceInfo.Platform == DevicePlatform.MacCatalyst && DeviceInfo.Version < new Version(14, 0))
                        {
                            await Toast.Make("Saving files is not supported on this macOS version. Please update to macOS 14.0 or later.").Show(cancellationToken);
                        }
#if !IOS && !MACCATALYST
                        else
                        {
                            var fileSaverResult = await fileSaver.SaveAsync($"{ProcessData.ProcessName}.zip", stream, cancellationToken);
                            fileSaverResult.EnsureSuccess();
                            await Toast.Make($"File is saved: {fileSaverResult.FilePath}").Show(cancellationToken);
                        }
#endif
                    }
                }
                catch(Exception ex)
                {
                    await Toast.Make($"File is not saved, {ex.Message}").Show(cancellationToken);
                }
            }
            base.HideBusy();
        }

        [RelayCommand]
        private async Task TaskAdd()
        {
            ShowBusy();
            await Task.Delay(100);

            int taskNumber = TaskList.Count == 0 ? 1 : TaskList.Max(m => m.Number) + 1;
            await Shell.Current.GoToAsync("TaskPage", true, new Dictionary<string, object>
            {
                ["task"] = new TaskModel { Number = taskNumber, Delay = 0, Enabled = true }
            });
            HideBusy();
        }

        [RelayCommand]
        private async Task TaskEdit(TaskModel task) 
        {
            ShowBusy();
            await Task.Delay(100);
            await Shell.Current.GoToAsync("TaskPage", true, new Dictionary<string, object>
            {
                ["task"] = task
            });
            HideBusy();
        }

        [RelayCommand]
        private async Task TaskDelete(TaskModel task)
        {
            bool result = await Shell.Current.DisplayAlertAsync("PROCESS AUTOMATION", "¿Estas seguro de borrar la tarea?", accept: "Aceptar", cancel: "Cancelar");

            if (result)
            {
                await Task.Run(() =>
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        TaskList.Remove(task);
                    });
                });
            }
        }
    }
}
