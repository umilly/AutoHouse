using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using ViewModel;
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

        private void CollectionViewSource_Filter(object sender, FilterEventArgs e)
        {
            e.Accepted = (e.Item is ParameterFilterVM)|| ViewModel.Filter.IsMatch((ParameterViewModel) e.Item);
        }

        private void DataGrid_OnBeginningEdit(object sender, DataGridBeginningEditEventArgs e)
        {
            e.Cancel = !(e.Row.DataContext as IPublicParam).IsEditable;
        }
    }
    public class ParamsSource : ObservableCollection<IPublicParam>
    {
    }
}
