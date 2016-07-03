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
    public partial class ControllerEditorView
    {
        public ControllerEditorView(ViewService viewService) : base(viewService)
        {
            InitializeComponent();
        }
        private async void FindClick(object sender, RoutedEventArgs e)
        {
            await ViewModel.FindSensors();
            MessageBox.Show(ViewModel.MessageFind);
        }

        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            ViewModel.Delete();
            //_viewService.ResetVM(this,0);
        }
    }
}