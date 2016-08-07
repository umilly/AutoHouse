using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace ViewTools
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var param = parameter!=null&& bool.Parse(parameter.ToString());
            var val = value as bool?;
            if (!val.HasValue)
                return new SolidColorBrush(Colors.Transparent);
            if (val.Value ^ param)
                return new SolidColorBrush(Colors.LightGreen);
            else
                return new SolidColorBrush(Colors.LightCoral);

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}