﻿using System.ComponentModel;
using System.Linq;
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
            _container.RegisterType<IWebServer, WebServer.WebServer>();
            var types = GetType().Assembly.GetTypes().Where(type => typeof(IView).IsAssignableFrom(type));
            _container.Use<IViewService>().FillTypes(types.ToArray());
            MainVM = new MainViewModel(_container);
            MainVM.InitSettings();
            InitializeComponent();
            CommandBindings.Add(new CommandBinding(ShowNextViewCommand.Instance, OnNextView));
            ShowDevicesClick(null,null);
            Closed += (sender, args) => ShutDown(false);
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
                if(pc!=null)
                    pc.Invoke(this, new PropertyChangedEventArgs("CurrentContent"));
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void SettingsMenuClick(object sender, RoutedEventArgs e)
        {
            CurrentContent = _container.Use<IViewService>().CreateView<SettingsView>(1);
            //OnNextView(null, null);
        }

        private void ControlMenuClick(object sender, RoutedEventArgs e)
        {
            
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            MainVM.SaveSettings();
        }

        private void LoadClick(object sender, RoutedEventArgs e)
        {
            MainVM.InitSettings();
        }

        private void ShowDevicesClick(object sender, RoutedEventArgs e)
        {
            CurrentContent=_container.Use<IViewService>().CreateView<DevicesNavigationView>();
            //OnNextView(null, null);
        }

        private void AddControllerClick(object sender, RoutedEventArgs e)
        {
            
            
        }

        private void ShowReactionsClick(object sender, RoutedEventArgs e)
        {
            CurrentContent = _container.Use<IViewService>().CreateView<ReactionNavigationView>();
            //OnNextView(null, null);
        }

        private void ShowParametersList(object sender, RoutedEventArgs e)
        {
            CurrentContent = _container.Use<IViewService>().CreateView<ParametersListView>();
            //OnNextView(null, null);
        }

        private void ShowZoneList(object sender, RoutedEventArgs e)
        {
            CurrentContent = _container.Use<IViewService>().CreateView<ZoneListView>();
            //OnNextView(null, null);
        }
        private void ExitClick(object sender, RoutedEventArgs e)
        {
            ShutDown();
        }

        private void ShutDown(bool needClose=true)
        {
            if(needClose)
                Close();
            Application.Current.Shutdown();
            _container.Use<ITimerSerivce>().Dispose();
        }

        private void ShowCategoryList(object sender, RoutedEventArgs e)
        {
            CurrentContent = _container.Use<IViewService>().CreateView<CategoryEditor>();
        }
    }
}
