using ProcessAutomationMAUI.Base;
using ProcessAutomationMAUI.ViewModel;

namespace ProcessAutomationMAUI.Pages.Process
{
    
    public partial class InputValuePage : ContentPage
    {
        public InputValuePage(InputValueVM vm)
        {
            InitializeComponent();
            BindingContext = vm;
            vm.webView = inputWebView;
        }
    }
}