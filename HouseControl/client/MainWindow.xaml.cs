using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using Facade;
using View;
using ViewModel;
using VMBase;


namespace client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ClientMainWindow : Window, INotifyPropertyChanged
    {
        private ContentControl _currentConent;
        private readonly IServiceContainer _container = new Container();
        private ClientMainViewModel MainVM { get; set; }
        public ClientMainWindow()
        {
            _container.RegisterType<IViewService, ViewService>();
            var types = GetType().Assembly.GetTypes().Where(type => typeof(IView).IsAssignableFrom(type));
            _container.Use<IViewService>().FillTypes(types.ToArray());
            MainVM = new ClientMainViewModel(_container);
            MainVM.InitSettings();
            InitializeComponent();
            CommandBindings.Add(new CommandBinding(ShowNextViewCommand.Instance, OnNextView));
        }



        private void OnNextView(object sender, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            if ((CurrentContent != null) && (CurrentContent is IView))
            {
                (CurrentContent as IView).OnClose();
            }
            CurrentContent = _container.Use<IViewService>().NextView as ContentControl;
        }

        public ContentControl CurrentContent
        {
            get { return _currentConent; }
            private set
            {
                _currentConent = value;
                var pc = PropertyChanged;
                if (pc != null)
                    pc.Invoke(this, new PropertyChangedEventArgs("CurrentContent"));
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void HomeClick(object sender, RoutedEventArgs e)
        {
            CurrentContent = _container.Use<IViewService>().CreateView<ClientModesView>();
        }

        private void OptionsClick(object sender, RoutedEventArgs e)
        {
            CurrentContent = _container.Use<IViewService>().CreateView<ClienOptions>(1);
        }

        private void CommonParams(object sender, RoutedEventArgs e)
        {
            CurrentContent = _container.Use<IViewService>().CreateView<ClientParametersView>(1);
        }
    }
}
