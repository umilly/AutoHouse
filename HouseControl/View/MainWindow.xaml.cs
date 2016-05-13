using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Facade;
using View;
using ViewModel;
using VMBase;
using Xceed.Wpf.Toolkit;

namespace WpfApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        private ContentControl _currentConent;
        private readonly IServiceContainer _container=new Container();
        private MainViewModel MainVM { get; set; }
        public MainWindow()
        {
            _container.RegisterType<IViewService,ViewService>();
            MainVM = new MainViewModel(_container);
            MainVM.InitSettings();
            InitializeComponent();
            CommandBindings.Add(new CommandBinding(ShowNextViewCommand.Instance, OnNextView));
            
            OpenControlView();
        }

        private void OpenControlView()
        {
            var control = _container.Use<IViewService>().CreateView<ContolView>(1);
            control.ViewModel.FillRelays();
            CurrentContent = control;
        }

        private void OnNextView(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            CurrentContent = _container.Use<IViewService>().NextView as ContentControl;
        }

        public ContentControl CurrentContent
        {
            get { return _currentConent; }
            private set
            {
                _currentConent = value;
                var pc = PropertyChanged;
                if(pc!=null)
                    pc.Invoke(this, new PropertyChangedEventArgs("CurrentContent"));
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void SettingsMenuClick(object sender, RoutedEventArgs e)
        {
            var settings = _container.Use<IViewService>().CreateView<SettingsView>(1);
            CurrentContent = settings;
        }

        private void ControlMenuClick(object sender, RoutedEventArgs e)
        {
            OpenControlView();
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            MainVM.SaveSettings();
        }

        private void LoadClick(object sender, RoutedEventArgs e)
        {
            MainVM.InitSettings();
        }

        private void ShowControllersClick(object sender, RoutedEventArgs e)
        {
            
        }

        private void AddControllerClick(object sender, RoutedEventArgs e)
        {
            
            var controller = _container.Use<IViewService>().CreateView<ControllerEditorView>(MainVM.GetOrCreateControllerVm());
            CurrentContent = controller;
        }
    }
}
