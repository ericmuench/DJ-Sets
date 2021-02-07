using System;
using System.Globalization;
using System.Windows.Data;
using DJSets.model.autogeneration;

namespace DJSets.util.mvvm.converters
{
    /// <summary>
    /// This class defines the convert-logic between a string and a MovementGenerationMode
    /// </summary>
    public class MovementGenerationModeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is MovementGenerationMode mode)
            {
                return mode.GetTitle();
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return MovementGenerationModeUtils.MovementGenerationModesDefault();
            }

            return MovementGenerationModeUtils.GetFromString(value.ToString());
        }
    }
}
