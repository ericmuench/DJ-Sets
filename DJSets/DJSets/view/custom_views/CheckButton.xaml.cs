using System;
using System.Windows;
using System.Windows.Controls.Primitives;

namespace DJSets.view.custom_views
{
    /// <summary>
    /// This class defines a form of a ToggleButton with 2 different SVGs as Content depending on the checked-State
    /// </summary>
    public partial class CheckButton : ToggleButton
    {
        #region Constructors
        public CheckButton()
        {
            InitializeComponent();
            ViewboxImg.Source = IconUri;
        }
        #endregion

        #region Fields
        private readonly Uri _iconUriUnchecked = new Uri("pack://application:,,,/DJSets;component/resources/images/ICUnchecked.svg");
        private readonly Uri _iconUriChecked = new Uri("pack://application:,,,/DJSets;component/resources/images/ICChecked.svg");

        public Uri IconUri => (IsChecked ?? false) ? _iconUriChecked : _iconUriUnchecked;

        #endregion

        #region Events
        /// <summary>
        /// This Event causes the button to change to checked state and change the icon to checked
        /// </summary>
        /// <param name="e">Check-Event params</param>
        /// <inheritdoc cref="OnChecked"/>
        protected override void OnChecked(RoutedEventArgs e)
        {
            base.OnChecked(e);
            ViewboxImg.Source = IconUri;
        }

        /// <summary>
        /// This Event causes the button to change to checked state and change the icon to unchecked
        /// </summary>
        /// <param name="e">Check-Event params</param>
        /// <inheritdoc cref="OnChecked"/>
        protected override void OnUnchecked(RoutedEventArgs e)
        {
            base.OnUnchecked(e);
            ViewboxImg.Source = IconUri;
        }
        #endregion
    }
}
