using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Cache;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ViewModel;
using VMBase;

namespace View
{
    /// <summary>
    /// Interaction logic for UserControl1.xaml
    /// </summary>
    public partial class ClientSingleParameterView
    {
        public ClientSingleParameterView()
        {
            InitializeComponent();
        }
        
        public ClientSingleParameterView(ViewService viewService) : base(viewService)
        {
            InitializeComponent();
        }

        BitmapImage image = null;

        public bool ToggleValue
        {
            get { return ViewModel?.TogggleValue ?? false; }
            set
            {
                ViewModel.TogggleValue = value;
                OnPropertyChanged("RadioButton");
            }
        }
        public BitmapImage RadioButton
        {
            get
            {
                return ViewModel.TogggleValue
                    ? new BitmapImage(new Uri("pack://application:,,,/Resources/radioOn.png"))
                    : new BitmapImage(new Uri("pack://application:,,,/Resources/radioOff.png"));
            }
        }

        public BitmapImage Image1
        {
            get { return image; }
        }

        public override void OnVMSet()
        {
            base.OnVMSet();
            image = string.IsNullOrEmpty(ViewModel.ParamImage)
                ? null
                : new BitmapImage(new Uri(ViewModel.ParamImage));
            OnPropertyChanged("Image1");
        }
    }

    public class RadioButtonValueToImageConverter : IValueConverter
    {
        static BitmapImage on = new BitmapImage(new Uri("pack://application:,,,/Resources/radioOn.png"));
        static BitmapImage off = new BitmapImage(new Uri("pack://application:,,,/Resources/radioOff.png"));
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool) value)
                return @on;
            return off;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
