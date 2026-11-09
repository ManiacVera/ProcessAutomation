using CommunityToolkit.Mvvm.ComponentModel;
using ProcessAutomation.Common.DTO.Info;

namespace ProcessAutomationMAUI.DTO.Model
{
    public partial class StepModel : ObservableObject
    {
        [ObservableProperty]
        private int encapsulatedId;

        [ObservableProperty]
        private string encapsulatedPartitionType;

        [ObservableProperty]
        private int number;

        [ObservableProperty]
        private string description;

        [ObservableProperty]
        private string variable;

        [ObservableProperty]
        private bool isPublicVariable;

        [ObservableProperty]
        private string component;

        //[ObservableProperty]
        //private ComboBoxModel componentItem;

        [ObservableProperty]
        private int componentIndex;

        [ObservableProperty]
        private string value;

        [ObservableProperty]
        private string inputValueType;
        
        [ObservableProperty]
        private string valueAction;

        [ObservableProperty]
        private bool applyFormula;

        [ObservableProperty]
        private string outputValueType;

        [ObservableProperty]
        private string outputResultOption;

        [ObservableProperty] 
        private string valueTypeDeclaration;

        [ObservableProperty]
        private int delay;

        [ObservableProperty]
        private bool required;

        [ObservableProperty]
        private bool async;

        [ObservableProperty]
        private bool log;

        [ObservableProperty]
        private bool disable;

        [ObservableProperty]
        private List<ValueOptionInfo> valueOptionList;

        [ObservableProperty]
        private List<object> parameterList;

        [ObservableProperty]
        private bool isBeingDragged;

        [ObservableProperty]
        private bool isBeingDraggedOver;
    }
}
