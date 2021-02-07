using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace DJSets.util.mvvm.converters
{
    /// <summary>
    /// This class converts an Boolean-Value into an VisibilityValue
    /// </summary>
    class BooleanToVisibiltyConverter : IValueConverter
    {
        #region Constructors
        public BooleanToVisibiltyConverter(bool shouldCollapse = false)
        {
            _defaultInvisibleVisibility = shouldCollapse ? Visibility.Collapsed : Visibility.Hidden;
        }
        #endregion

        #region Fields
        /// <summary>
        /// This field determines which Visibility to use when UI-element should not be visible
        /// </summary>
        private readonly Visibility _defaultInvisibleVisibility;

        #endregion
        #region Interface Functions for IValueConverter
        /// <see cref="IValueConverter.Convert"/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool shouldBeVisible)
            {
                return shouldBeVisible ? Visibility.Visible : _defaultInvisibleVisibility;
            }

            return value;
        }

        /// <see cref="IValueConverter.ConvertBack"/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility visibility)
            {
                return visibility == Visibility.Visible;
            }

            return false;
        }
        #endregion
    }
}
