using System;
using System.Globalization;
using System.Windows.Data;
using DJSets.model.autogeneration;

namespace DJSets.util.mvvm.converters
{
    /// <summary>
    /// This class defines the convert-logic between a string and a GeneralSpecificGenerationMode
    /// </summary>
    public class GeneralSpecificGenerationModeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is GeneralSpecificGenerationMode mode)
            {
                return mode.GetTitle();
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return GeneralSpecificGenerationModeUtils.GeneralSpecificGenerationModesDefault();
            }

            return GeneralSpecificGenerationModeUtils.GetFromString(value.ToString());
        }
    }
}
