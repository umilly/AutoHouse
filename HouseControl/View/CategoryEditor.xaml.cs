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
    public partial class CategoryEditor
    {
        public CategoryEditor(ViewService viewService) : base(viewService)
        {
            InitializeComponent();
        }

        private void AddCategoryClick(object sender, RoutedEventArgs e)
        {
            ViewModel.CreateCategory();
        }
    }
}