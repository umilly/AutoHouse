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

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            var controllers=ViewModel.GetControllers();
            ViewModel.SaveDB();
        }

        private void FindClick(object sender, RoutedEventArgs e)
        {
            ViewModel.Find();
        }
    }
}