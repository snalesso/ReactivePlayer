using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace ReactivePlayer.UI.Wpf.Converters
{
    public class StringEnumerableJoiner : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var strings = value as IEnumerable<string>;
            return string.Join(", ", strings);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}