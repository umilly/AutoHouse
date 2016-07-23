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
    public partial class CommandEditor
    {
        public CommandEditor(ViewService viewService) : base(viewService)
        {
            InitializeComponent();
        }
    }
}