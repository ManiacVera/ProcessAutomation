using ProcessAutomationMAUI.DTO.Model;
using ProcessAutomationMAUI.ViewModel;

namespace ProcessAutomationMAUI.Pages.Process
{

    public partial class TaskPage : ContentPage
    {
        public TaskPage(TaskVM vm)
        {
            InitializeComponent();
            BindingContext = vm;
            vm.stepCollectionView = StepCollectionView;
        }

        protected override bool OnBackButtonPressed()
        {
            ((TaskVM)BindingContext).BackButtonPressedCommand.Execute(this);
            return base.OnBackButtonPressed();
        }

        private void ComponentPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            Picker picker = (Picker)sender;
            ((TaskVM)BindingContext).ComponentPickerSelectedCommand.Execute((StepModel)picker.BindingContext);
        }

        //private void ComponentListPicker_SelectedIndexChanged(object sender, EventArgs e)
        //{
        //    ((TaskVM)BindingContext).ComponentListPickerSelectedCommand.Execute(sender);
        //}

        private void ComponentListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            ((TaskVM)BindingContext).ComponentListViewItemTappedCommand.Execute((ComboBoxModel)e.Item);
        }
    }
}