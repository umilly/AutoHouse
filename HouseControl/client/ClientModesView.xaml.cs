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
    public partial class ClientModesView
    {
        public ClientModesView()
        {
            InitializeComponent();
        }

        public ClientModesView(ViewService viewService) : base(viewService)
        {
            InitializeComponent();
        }

        BitmapImage image = null;

        public override void OnVMSet()
        {
            base.OnVMSet();
            ViewModel.AskModes();
            if(image==null)
                image=new BitmapImage(new Uri(ViewModel.ImagePath, UriKind.Relative));
            ImageBox.Source = image; //new BitmapImage(new Uri(ViewModel.ImagePath,UriKind.RelativeOrAbsolute));
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
