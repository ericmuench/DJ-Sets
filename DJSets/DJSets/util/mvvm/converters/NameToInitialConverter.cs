using System;
using System.Globalization;
using System.Windows.Data;

namespace DJSets.util.mvvm.converters
{
    /// <summary>
    /// This class converts a Name to its initial character
    /// </summary>
    /// <remarks>THIS CLASS WAS COPIED FROM PETER RILL: LOCs do not count in this file</remarks>
    public class NameToInitialConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string s && s.Length > 0)
            {
                var x = s.Substring(0, 1).ToUpper();
                return x;
            }
                
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //convert back is not implemented
            return null;
        }
    }
}