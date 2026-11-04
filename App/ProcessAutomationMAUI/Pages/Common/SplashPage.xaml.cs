using ProcessAutomationMAUI.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessAutomationMAUI.Pages.Common
{
    
    public partial class SplashPage : ContentPage
    {
        public SplashPage(SplashVM vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}