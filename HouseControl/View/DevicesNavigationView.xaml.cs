using System;
using System.Windows;
using System.Windows.Controls;
using Facade;
using ViewModel;
using VMBase;
using MessageBox = System.Windows.MessageBox;

namespace View
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class DevicesNavigationView : CustomViewBase<DevicesEditorVM>
    {
        public DevicesNavigationView(ViewService viewService) : base(viewService)
        {
            InitializeComponent();
            DeviceTreeView.SelectedItemChanged+=DeviceTreeViewOnSelectedItemChanged;
        }

        private void DeviceTreeViewOnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> routedPropertyChangedEventArgs)
        {
            var value=DeviceTreeView.SelectedValue;
            Editor= _viewService.CreateView(value as IViewModel);
            OnPropertyChanged(()=>Editor);
        }

        public IView Editor { get; set; }
    }
}