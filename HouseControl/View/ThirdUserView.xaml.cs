using System.Threading;
using System.Windows;
using ViewModel;
using VMBase;

namespace View
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class ThirdUserView : CustomViewBase<ThirdRoleVM>
    {
        private bool _token;

        private Thread _firstThread;
        private Thread _secondThread;


        public ThirdUserView(ViewService viewService) : base(viewService)
        {
            InitializeComponent();
            _token = false;
            Dispatcher.ShutdownStarted += Dispatcher_ShutdownStarted;
        }

        private void Dispatcher_ShutdownStarted(object sender, System.EventArgs e)
        {
            _token = false;
            _firstThread.Join();
            _secondThread.Join();
        }

        private void ActionClick(object sender, RoutedEventArgs e)
        {
            _token = !_token;
            if (!_token) return;

            _firstThread = new Thread(FirstFunc);
            _secondThread = new Thread(SecondFunc);
            _firstThread.Start();
            _secondThread.Start();
        }

        private void FirstFunc()
        {
            while (_token)
            {
                ViewModel.IncreaseFirstField();
                Thread.Sleep(1000);
            }
        }
        private void SecondFunc()
        {
            while (_token)
            {
                ViewModel.IncreaseSecondField();
                Thread.Sleep(1000);
            }
        }
    }
}
