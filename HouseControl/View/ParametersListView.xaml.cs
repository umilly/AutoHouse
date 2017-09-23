using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VMBase;

namespace View
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class ParametersListView 
    {
        public ParametersListView()
        {
            InitializeComponent();
        }

        public ParametersListView(ViewService viewService) : base(viewService)
        {
            InitializeComponent();
        }

        private void AddParamsClick(object sender, RoutedEventArgs e)
        {
            ViewModel.CreateParams();
        }

        private void ClearFilter(object sender, RoutedEventArgs e)
        {
            ViewModel.ClearFilter();
        }
    }
}
