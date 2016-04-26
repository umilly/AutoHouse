using System.Windows;
using Facade;
using ViewModel;
using VMBase;
using MessageBox = System.Windows.MessageBox;

namespace View
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class ContolView : CustomViewBase<ConrolPanelVM>
    {
        public ContolView(ViewService viewService) : base(viewService)
        {
            InitializeComponent();
        }

        public void Invalidate()
        {
            
        }

        private void AllOnClick(object sender, RoutedEventArgs e)
        {
            ViewModel.SwitchAll(true);

        }

        private void AllOffClick(object sender, RoutedEventArgs e)
        {
            ViewModel.SwitchAll(false);
        }
    }
}