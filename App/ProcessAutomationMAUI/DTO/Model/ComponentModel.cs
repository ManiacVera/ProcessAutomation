using CommunityToolkit.Mvvm.ComponentModel;
using ProcessAutomation.Common.DTO.Contract;

namespace ProcessAutomationMAUI.DTO.Model
{
    public partial class ComponentModel : ObservableObject
    {
        [ObservableProperty]
        private string taskReference;

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private string description;

        [ObservableProperty]
        private string componentType;

        [ObservableProperty]
        private string value;

        //[ObservableProperty]
        //private dynamic dynamicValue;

        //[ObservableProperty]
        //private IConfigDTO valueDTO;

        [ObservableProperty]
        private string valueType;

        [ObservableProperty]
        private string valueFormat;
    }
}
