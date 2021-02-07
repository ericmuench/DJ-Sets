using System;
using System.Globalization;
using System.Windows.Data;
using DJSets.model.export;

namespace DJSets.util.mvvm.converters
{
    /// <summary>
    /// This class can convert a SetlistExportType into a String and vice versa
    /// </summary>
    class SetlistExportTypeToStringConverter : IValueConverter
    {
        #region Interface Functions for IValueConverter
        /// <see cref="IValueConverter.Convert"/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is SetlistExportType type)
            {
                return type.FileExtensionName();
            }

            return value;
        }

        /// <see cref="IValueConverter.ConvertBack"/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string str)
            {
                return SetlistExportTypeUtils.GetByFileExtensionName(str);
            }

            return value;
        }
        #endregion
    }
}
