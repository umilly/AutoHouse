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

        private void AddBoolClick(object sender, RoutedEventArgs e)
        {
            ViewModel.AddParam(ParameterTypeValue.Bool);
        }

        private void AddInt(object sender, RoutedEventArgs e)
        {
            ViewModel.AddParam(ParameterTypeValue.Int);
        }

        private void AddReal(object sender, RoutedEventArgs e)
        {
            ViewModel.AddParam(ParameterTypeValue.Double);
        }

        private void AddString(object sender, RoutedEventArgs e)
        {
            ViewModel.AddParam(ParameterTypeValue.String);
        }

        private void DeleteParams(object sender, RoutedEventArgs e)
        {
            ViewModel.DeleteParams();
        }
    }
}