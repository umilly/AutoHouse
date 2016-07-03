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
            if (string.IsNullOrEmpty(ViewModel.IP)|| ViewModel.Port<=0)
            {
                MessageBox.Show("Укажите IP и порт");
                return;
            }
            await ViewModel.FindSensors();
            MessageBox.Show(ViewModel.MessageFind);
        }
    }
}