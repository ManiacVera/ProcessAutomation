using ProcessAutomationMAUI.ViewModel;

namespace ProcessAutomationMAUI.Pages.Process
{

    public partial class StatementPage : ContentPage
    {
        public StatementPage(StatementVM vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}