using System;
using System.Collections.Generic;
using System.Linq;
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
    public partial class ClientParametersView
    {
        public  ClientParametersView()
        {
            InitializeComponent();
        }

        public  ClientParametersView(ViewService viewService) : base(viewService)
        {
            InitializeComponent();
        }

        public List<ClientParameterContainerView> ParameterViewList
        {
            get { return ViewModel?.Parameters.Select(a => _viewService.CreateView<ClientParameterContainerView>(a)).ToList(); }
        }

        public override void OnVMSet()
        {
            base.OnVMSet();
            ViewModel.AskParams(()=>OnPropertyChanged("ParameterViewList"));
            OnPropertyChanged("ParameterViewList");
        }
    }
}
