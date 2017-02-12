using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
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
using Color = System.Drawing.Color;
using Page = ViewModel.Page;


namespace client
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ClientMainWindow : Window, INotifyPropertyChanged
    {
        private ContentControl _currentConent;
        private readonly IServiceContainer _container = new Container();
        public ClientMainViewModel MainVM { get; set; }
        public ClientMainWindow()
        {
            _container.RegisterType<IViewService, ViewService>();
            var types = GetType().Assembly.GetTypes().Where(type => typeof(IView).IsAssignableFrom(type));
            _container.Use<IViewService>().FillTypes(types.ToArray());
            MainVM = new ClientMainViewModel(_container);
            MainVM.InitSettings();
            InitializeComponent();
            CommandBindings.Add(new CommandBinding(ShowNextViewCommand.Instance, OnNextView));
            HomeClick(null,null);
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

        

        private void OptionsClick(object sender, RoutedEventArgs e)
        {
            MainVM.Page = Page.Settings;
            MainVM.Page = Page.Settings;
            CurrentContent = _container.Use<IViewService>().CreateView<ClienOptions>(1);
        }

        private void HomeClick(object sender, RoutedEventArgs e)
        {
            MainVM.Page = Page.Main;
            CurrentContent = _container.Use<IViewService>().CreateView<ClientModesView>();
        }

        private void ClimaxClick(object sender, RoutedEventArgs e)
        {
            SetParameters(Page.Climax);
        }

        private void SetParameters(Page page)
        {
            MainVM.Page = page;
            var view = _container.Use<IViewService>().CreateView<ClientParametersView>(1);
            view.ViewModel.Page = page;
            CurrentContent = view;
        }

        private void LightClick(object sender, RoutedEventArgs e)
        {
            SetParameters(Page.Light);
        }

        private void RemoteClick(object sender, RoutedEventArgs e)
        {
            MainVM.Page = Page.Remote;
        }
    }

    public class BoolToBrushConverter : IValueConverter
    {
        private static SolidColorBrush orange =
            new SolidColorBrush(new System.Windows.Media.Color() {R = Color.DarkOrange.R, G = Color.DarkOrange.G, B = Color.DarkOrange.B, A = Color.DarkOrange.A });
        private static SolidColorBrush blue =
            new SolidColorBrush(new System.Windows.Media.Color() { R = Color.DodgerBlue.R, G = Color.DodgerBlue.G, B = Color.DodgerBlue.B, A = Color.DodgerBlue.A });
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var color= (bool)value?orange:blue;
            return color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

