using ProcessAutomationMAUI.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessAutomationMAUI.Pages.Security
{
    
    public partial class LoginPage : ContentPage
    {
        public LoginPage(LoginVM vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}