using System.ComponentModel;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ProcessAutomationMAUI.Base
{
    public partial class ViewModelBase : ObservableObject
    {
        [ObservableProperty]
        private bool isBusy;

        public ViewModelBase()
        {   
        }

        //public event PropertyChangedEventHandler PropertyChanged;

        //protected bool SetProperty<T>(ref T field, T newValue, [CallerMemberName] string propertyName = null)
        //{
        //    if (!EqualityComparer<T>.Default.Equals(field, newValue))
        //    {
        //        field = newValue;
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        //        return true;
        //    }
        //    return false;
        //}

        protected void ShowBusy()
        {   
            IsBusy = true;
        }

        protected void HideBusy()
        {
            IsBusy = false;
        }

        protected bool IsNotBusy()
        {
            return !IsBusy;
        }
    }
}
