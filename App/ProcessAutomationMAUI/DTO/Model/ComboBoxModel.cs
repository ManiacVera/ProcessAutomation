using CommunityToolkit.Mvvm.ComponentModel;

namespace ProcessAutomationMAUI.DTO.Model
{
    public partial class ComboBoxModel : ObservableObject
    {

        [ObservableProperty]
        private string text;

        [ObservableProperty]
        private string value;

        public ComboBoxModel()
        {

        }

        public ComboBoxModel(string text, string value)
        {
            this.Text = text;
            this.Value = value;
        }
    }
}
