using System.Windows;
using Facade;
using Model;
using ViewModel;
using VMBase;
using MessageBox = System.Windows.MessageBox;

namespace View
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class CustomDeviceEditor
    {
        public CustomDeviceEditor(ViewService viewService) : base(viewService)
        {
            InitializeComponent();
        }

        private void AddParamClick(object sender, RoutedEventArgs e)
        {
            ViewModel.AddParam();
        }
    

        private void DeleteParams(object sender, RoutedEventArgs e)
        {
            ViewModel.DeleteParams();
        }
    }
}