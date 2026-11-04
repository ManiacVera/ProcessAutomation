using ProcessAutomationMAUI.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessAutomationMAUI.Pages.Security
{
    
    public partial class AccountPage : ContentPage
    {
        public AccountPage(AccountVM vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}