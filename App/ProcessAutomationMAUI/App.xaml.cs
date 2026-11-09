using Microsoft.Maui.Controls.Handlers.Items;

namespace ProcessAutomationMAUI
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            CollectionViewHandler.Mapper.AppendToMapping("HeaderAndFooterFix", (_, collectionView) =>
            {
                collectionView.AddLogicalChild(collectionView.Header as Element);
                collectionView.AddLogicalChild(collectionView.Footer as Element);
            });
        }

        protected override Window CreateWindow(IActivationState activationState)
        {
            return new Window(new AppShell());
        }
    }
}
