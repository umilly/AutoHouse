using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ViewTools
{
    public class IntToColorConverter : IValueConverter
    {
        private int badTimeout=2000;
        private Color _goodColor = Colors.LightGreen;
        private Color _badColor = Colors.LightCoral;
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value==null)
                return new SolidColorBrush(Colors.Transparent);
            var val = (int)value;
            if(val==0)
                return new SolidColorBrush(Colors.Transparent);
            Color currentMaxColor;
            if (val < 0)
            {
                val = -val;
                currentMaxColor = _badColor;
            }
            else
            {
                currentMaxColor = _goodColor;
            }
            float percent = val / (float)badTimeout;
            if (percent > 1)
                percent = 1;
            var c = new Color();
            c.A = (byte)(255-255*percent);
            c.R = currentMaxColor.R;
            c.G = currentMaxColor.G;
            c.B = currentMaxColor.B;
            return new SolidColorBrush(c);
            
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}