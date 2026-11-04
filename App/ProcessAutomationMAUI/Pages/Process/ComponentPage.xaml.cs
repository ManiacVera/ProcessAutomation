using ProcessAutomationMAUI.ViewModel;

namespace ProcessAutomationMAUI.Pages.Process
{
    
    public partial class ComponentPage : ContentPage
    {
        public ComponentPage(ComponentVM vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        protected override bool OnBackButtonPressed()
        {
            ((ComponentVM)BindingContext).BackButtonPressedCommand.Execute(this);
            return base.OnBackButtonPressed();
        }
    }
}