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

        public int ID
        {
            get
            {
                return ViewModel.ID;
            }
            set
            {
                _viewService.ResetVM(this,value);
            }
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            ViewModel.SaveDB();
            MessageBox.Show("Устройвтва сохранены в базе");
        }

        private void FindClick(object sender, RoutedEventArgs e)
        {
            ViewModel.FindSensors();
            MessageBox.Show(ViewModel.MessageFind);
        }

        private void DeleteClick(object sender, RoutedEventArgs e)
        {
            ViewModel.Delete();
            _viewService.ResetVM(this,0);
        }
    }
}