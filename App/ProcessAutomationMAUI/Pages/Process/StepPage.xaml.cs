using ProcessAutomationMAUI.ViewModel;

namespace ProcessAutomationMAUI.Pages.Process
{
    
    public partial class StepPage : ContentPage
    {
        public StepPage(StepVM vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        protected override bool OnBackButtonPressed()
        {
            ((StepVM)BindingContext).BackButtonPressedCommand.Execute(this);
            return base.OnBackButtonPressed();
        }
    }
}