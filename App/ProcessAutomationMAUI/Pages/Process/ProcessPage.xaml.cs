using ProcessAutomationMAUI.ViewModel;

namespace ProcessAutomationMAUI.Pages.Process
{
    //[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProcessPage : ContentPage
    {
        public ProcessPage(ProcessVM vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        //protected override void OnAppearing()
        //{
        //    base.OnAppearing();
        //}

        //protected override void OnDisappearing()
        //{
        //    base.OnDisappearing();
        //}
    }
}