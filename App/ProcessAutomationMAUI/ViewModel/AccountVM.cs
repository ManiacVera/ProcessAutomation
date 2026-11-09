using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ProcessAutomationMAUI.Base;
using ProcessAutomationMAUI.DTO.Model;
using System.Collections.ObjectModel;

namespace ProcessAutomationMAUI.ViewModel
{
    public partial class AccountVM : ViewModelBase
    {
        [ObservableProperty]
        private AccountModel accountData;

        [ObservableProperty]
        private ObservableCollection<LanguageModel> languageList;

        [ObservableProperty]
        private LanguageModel languageSelected;

        public AccountVM()
        {
            accountData = new AccountModel();
            LanguageList = LanguageManager.GetLanguageList().ToObservableCollection();
            string languageCode = LanguageManager.GetLanguageCode();

            if (languageCode != null)
            {
                LanguageModel languageModel = languageList.FirstOrDefault(f =>  f.LanguageCode == languageCode);
                if (languageModel != null)
                {
                    languageSelected = languageModel;
                }
                else
                {
                    languageSelected = languageList.FirstOrDefault(f => f.LanguageCode == "en-US");
                }
            }
            else
            {
                languageSelected = languageList.FirstOrDefault(f => f.LanguageCode == "en-US");
            }

        }

        [RelayCommand]
        private void Save()
        {
            if (languageSelected == null || languageSelected.LanguageCode != accountData.LanguageCode)
            {
                AccountData.LanguageCode = languageSelected.LanguageCode;
                LanguageManager.SetLanguage(accountData.LanguageCode);
            }
        }

        [RelayCommand]
        private void ChangePassword()
        { 
        }
    }
}
