using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

public class BoolToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var param = bool.Parse(parameter.ToString());
        var val = value as bool?;
        if (!val.HasValue)
            return new SolidColorBrush(Colors.Gray);
        if (val.Value^param)
            return new SolidColorBrush(Colors.DarkOliveGreen);
        else
            return new SolidColorBrush(Colors.LightCoral);

    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}