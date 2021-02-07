using System;
using System.Windows;
using System.Windows.Controls;
using DJSets.resources;
using DJSets.util.Extensions;

namespace DJSets.view.custom_views
{
    /// <summary>
    /// This class defines an Button with an Icon. The icon can be textual icon using Segoe Assets
    /// in a TextBlock or an SVG using an SvgViewbox.
    /// </summary>
    /// <remarks>
    /// Line 77-135 are inspired by the following tutorial
    /// https://www.tutorialspoint.com/wpf/wpf_dependency_properties.htm
    /// --> LOC do not count
    /// </remarks>
    public partial class IconedButton : Button
    {
        #region Constructors
        public IconedButton()
        {
            InitializeComponent();
            OnSVGModeChanged(IsInSvgMode);

            TextBlockIcon.Text = TextualIcon;
            SvgIcon.Source = IconUri;
        }
        #endregion

        #region Fields
        /// <summary>
        /// This field determines whether to use <see cref="IconUri"/> or <see cref="TextualIcon"/>
        /// for the icon
        /// </summary>
        private bool _isInSvgMode;

        /// <summary>
        /// This field exposes the value of <see cref="_isInSvgMode"/>
        /// </summary>
        public bool IsInSvgMode
        {
            get => _isInSvgMode;
            set
            {
                if (_isInSvgMode == value)
                {
                    return;
                }

                _isInSvgMode = value;
                OnSVGModeChanged(_isInSvgMode);
            }
        }

        
        //Textblock icon
        /// <summary>
        /// This field stores the Asset-String for the textual Icon
        /// </summary>
        private string _textualIcon = "&#xEA3B;";

        /// <summary>
        /// This field exposes the value of <see cref="_textualIcon"/>
        /// </summary>
        public string TextualIcon
        {
            get => _textualIcon;
            set
            {
                if (value == _textualIcon) return;

                _textualIcon = value;
                TextBlockIcon.Text = TextualIcon;
            }
        }

        //Icon
        public static readonly DependencyProperty IconUriProperty 
            = DependencyProperty.Register(nameof(IconUri),typeof(Uri),typeof(IconedButton),
                new PropertyMetadata(
                    new Uri($"{Application.Current.GetResource<string>(AppResourceKeys.AppBaseUri)}/resources/images/ICUnchecked.svg"),
                    OnIconUriChanged));

        /// <summary>
        /// This property provides the Uri value for the SVGBox for Binding via <see cref="IconUriProperty"/>
        /// </summary>
        /// <remarks>
        /// While using this field with a bound value in a ViewModel you should always use
        /// the while Uri including the Application-Base-Uri
        /// </remarks>
        public Uri IconUri
        {
            get => GetValue(IconUriProperty) as Uri;
            set => SetValue(IconUriProperty,value);
        }
        #endregion

        #region Help function
        /// <summary>
        /// This function calls <see cref="OnSetIconUri"/> for the DependencyObject. This function is necessary
        /// to enable Binding o this Control.
        /// </summary>
        /// <param name="d">The dependency Object, usually an IconedButton</param>
        /// <param name="args">DependencyPropertyChangedEventArgs. They e.g. contain the new binding value</param>
        private static void OnIconUriChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            d.CastedAs<IconedButton>(it =>
            {
                it.OnSetIconUri(args);
            });
        }

        /// <summary>
        /// This function sets the IconUri to <see cref="SvgIcon"/>
        /// </summary>
        /// <param name="args">EventsArgs that a bound property has changed</param>
        private void OnSetIconUri(DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue is Uri newUri)
            {
                SvgIcon.Source = newUri;
            }
        }

        /// <summary>
        /// This function changes the UI Components according to <see cref="IsInSvgMode"/>
        /// </summary>
        /// <param name="inSvgMode">Whether this IconedButton should display an SVG or not</param>
        private void OnSVGModeChanged(bool inSvgMode)
        {
            SvgIcon.Visibility = (inSvgMode) ? Visibility.Visible : Visibility.Collapsed;
            TextBlockIcon.Visibility = (inSvgMode) ? Visibility.Collapsed : Visibility.Visible;
        }
        #endregion
    }
}
