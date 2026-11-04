using CommunityToolkit.Mvvm.ComponentModel;

namespace ProcessAutomationMAUI.DTO.Model
{
    public partial class LanguageModel : ObservableObject
    {
        [ObservableProperty]
        private string language;

        [ObservableProperty]
        private string languageCode;
    }
}
