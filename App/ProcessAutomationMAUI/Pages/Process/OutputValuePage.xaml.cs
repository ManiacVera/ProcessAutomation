using ProcessAutomationMAUI.ViewModel;

namespace ProcessAutomationMAUI.Pages.Process
{

    public partial class OutputValuePage : ContentPage
    {
        public OutputValuePage(OutputValueVM vm)
        {
            InitializeComponent();
            BindingContext = vm;
            vm.webView = OutputWebView;
        }
    }
}