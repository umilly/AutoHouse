using System.Collections.Generic;
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
    public partial class TemplateEditor
    {
        public TemplateEditor(ViewService viewService) : base(viewService)
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

        private void AddDevicePair(object sender, RoutedEventArgs e)
        {
            ViewModel.AddDevicePairs();
        }

        private void DeleteDevicePairs(object sender, RoutedEventArgs e)
        {
            ViewModel.DeleteDevicePairs();
        }
    }

    
}