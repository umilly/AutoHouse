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
    public partial class SensorEditor
    {
        public SensorEditor(ViewService viewService) : base(viewService)
        {
            InitializeComponent();
        }
    }
}