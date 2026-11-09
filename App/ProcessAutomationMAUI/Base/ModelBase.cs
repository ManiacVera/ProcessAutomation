using CommunityToolkit.Mvvm.ComponentModel;

namespace ProcessAutomationMAUI.Base
{
    public abstract class ModelBase<MODEL, DTO> : ObservableObject
    {
        public abstract MODEL ConvertModel(DTO dto);
        public abstract DTO ConvertDTO(MODEL model);
    }
}
