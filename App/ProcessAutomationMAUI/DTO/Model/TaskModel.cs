using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;

namespace ProcessAutomationMAUI.DTO.Model
{
    public class TaskModel : ObservableObject
    {
        public int Number {get;set;}
        public string Name {get;set;}
        public int Delay { get; set; }
        public bool Enabled {get;set;}
        public List<StepModel> StepList{get;set;}
        public List<ComponentModel> ComponentList { get; set; }
    }
}
