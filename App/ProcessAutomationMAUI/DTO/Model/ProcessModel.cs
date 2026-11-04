using CommunityToolkit.Mvvm.ComponentModel;
using ProcessAutomation.Common.DTO.Info;
using System.Collections.ObjectModel;

namespace ProcessAutomationMAUI.DTO.Model
{
    public partial class ProcessModel : ObservableObject
    {
        [ObservableProperty]
        private int number;

        [ObservableProperty]
        private string processName;

        [ObservableProperty]
        private bool enabled;

        [ObservableProperty]
        private DatabaseInfo processDatabase;

        [ObservableProperty]
        private ObservableCollection<LanguageSourceSettingInfo> languageSettingList;

        [ObservableProperty]
        private ObservableCollection<TaskModel> taskList;
    }
}
