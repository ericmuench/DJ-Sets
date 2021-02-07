using System;
using System.Globalization;
using System.Windows.Data;
using DJSets.model.musickeys;

namespace DJSets.util.mvvm.converters
{
    /// <summary>
    /// This class defines the convert-logic between a string and a music key
    /// </summary>
    public class MusicKeysToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MusicKeys key)
            {
                return key.GetTitle();
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return MusicKeysUtils.MusicKeysDefault();
            }

            return MusicKeysUtils.GetFromString(value.ToString());
        }
    }
}
