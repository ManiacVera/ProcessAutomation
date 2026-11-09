using CommunityToolkit.Mvvm.ComponentModel;

namespace ProcessAutomationMAUI.DTO.Model
{
    public partial class AccountModel : ObservableObject
    {
        [ObservableProperty]
        private string email;

        [ObservableProperty]
        private string oldPassword;

        [ObservableProperty]
        private string newPassword;

        [ObservableProperty]
        private string languageCode;
    }
}
