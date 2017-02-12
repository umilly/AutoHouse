using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
    public partial class ClientParameterContainerView
    {
        public ClientParameterContainerView()
        {
            InitializeComponent();
        }
        public List<ClientSingleParameterView> ParameterViewList
        {
            get { return ViewModel?.Chain.Select(a => _viewService.CreateView<ClientSingleParameterView>(a)).ToList(); }
        }
        public ClientParameterContainerView(ViewService viewService) : base(viewService)
        {
            InitializeComponent();
        }
        
        public override void OnVMSet()
        {
            base.OnVMSet();
            OnPropertyChanged("ParameterViewList");
        }
    }
}
