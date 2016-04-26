using System.Windows;
using ViewModel;
using VMBase;

namespace View
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class SecondUserView : CustomViewBase<SecondRoleVM>
    {
        public SecondUserView(ViewService viewService) : base(viewService)
        {
            InitializeComponent();
        }

        private void LoadClick(object sender, RoutedEventArgs e)
        {
            ViewModel.LoadData();
        }
    }
}
