using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Media;
using DJSets.Annotations;
using DJSets.resources;
using DJSets.util.Extensions;
using DJSets.viewmodel.main_menu;

namespace DJSets.view.custom_views
{
    /// <summary>
    /// This class contains the code behind logic for a MenuNavigationButton.
    /// </summary>
    /// <remarks>
    /// A MenuNavigationButton is a button in the Side-Menu-Panel that contains an internal Selected-State, an icon
    /// a text and a hover color.
    /// </remarks>
    public partial class MenuNavigationButton : Button, INotifyPropertyChanged
    {
        #region Constructors
        public MenuNavigationButton()
        {
            DataContext = this;
            InitializeComponent();
        }
        #endregion

        #region Fields
        //Selected state
        /// <summary>
        /// This property stores the seleted-state of the Button.
        /// </summary>
        /// <remarks>
        /// The Selected state also has impact on the hover color as well as the background. Further, it declares
        /// if the menu associated with the button is selected or not.
        /// </remarks>
        private bool _isSelected = true;
        ///<summary>
        /// This property provides the Selected State stored in <see cref="_isSelected"/> for Binding in xaml.
        /// Additionally it provides a Setter triggering the <see cref="OnPropertyChanged"/> function if the value has changed
        /// </summary>
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (value != _isSelected)
                {
                    _isSelected = value;
                    NotifySelectedStateChanged();
                }

            }
        }

        //menu state 
        /// <summary>
        /// This property stores the <see cref="MenuState"/> of the MainMenu that the Button is representing
        /// </summary>
        private MainMenuState _menuState = MainMenuState.Setlists;
        ///<summary>
        /// This property provides the  MenuState stored in <see cref="_menuState"/> for Binding in xaml.
        /// Additionally it provides a Setter triggering the <see cref="OnPropertyChanged"/> function if the value has changed
        /// </summary>
        public MainMenuState MenuState
        {
            get => _menuState;
            set
            {
                if (value != _menuState)
                {
                    _menuState = value;
                    OnPropertyChanged(nameof(MenuState));
                }
            }
        }

        //Hover color
        ///<summary>
        /// This property provides the Hover color for Binding in xaml.
        /// Additionally it provides a Setter triggering the <see cref="OnPropertyChanged"/> function if the value has changed
        /// </summary>
        public SolidColorBrush HoverColor
        {
            get
            {
                var defaultVal = new SolidColorBrush(Color.FromRgb(128, 128, 128));
                return (IsSelected)
                    ? this.GetResource(ColorResourceKeys.BackgroundColorAccentHover, defaultVal)
                    : this.GetResource(ColorResourceKeys.BackgroundColorPrimaryDarkHover, defaultVal);
            }

        }

        //Click Ripple color
        ///<summary>
        /// This property provides the color of the ripple effect when the button is clicked for Binding in xaml.
        /// Additionally it provides a Setter triggering the <see cref="OnPropertyChanged"/> function if the value has changed
        /// </summary>
        public SolidColorBrush ClickRippleColor
        {
            get
            {
                var defaultVal = new SolidColorBrush(Color.FromRgb(128, 128, 128));
                return (IsSelected)
                    ? this.GetResource(ColorResourceKeys.BackgroundColorAccentClick, defaultVal)
                    : this.GetResource(ColorResourceKeys.BackgroundColorPrimaryDarkClick, defaultVal);
            }

        }

        //Text
        /// <summary>
        /// This property stores the Text of the Button/Label inside the button
        /// </summary>
        private string _text = "Button";
        ///<summary>
        /// This property provides the text stored in <see cref="_text"/> for Binding in xaml.
        /// Additionally it provides a Setter triggering the <see cref="OnPropertyChanged"/> function if the value has changed
        /// </summary>
        public string Text
        {
            get => _text;
            set
            {
                if (_text != value)
                {
                    _text = value;
                    OnPropertyChanged(nameof(Text));
                }
            }
        }

        //Icon
        /// <summary>
        /// This property stores the uri for the icon inside the button
        /// </summary>
        private Uri _iconUri = new Uri("pack://application:,,,/DJSets;component/resources/images/ICUnchecked.svg");
        /// <summary>
        /// This property provides the stored value defined in <see cref="_iconUri"/>.
        /// Additionally it provides a Setter triggering the <see cref="OnPropertyChanged"/> function if the value has changed
        /// </summary>
        /// <remarks>IMPORTANT: When setting this uri it is necessary to access the baseURI of this application, defined in App.xaml/></remarks>
        ///
        public Uri IconUri
        {
            get => _iconUri;
            set
            {
                var baseUriStr = this.GetResource<string>(AppResourceKeys.AppBaseUri);
                if (_iconUri != value && baseUriStr != null)
                {
                    var bUri = new UriBuilder(baseUriStr).Uri;
                    _iconUri = new Uri(bUri, value);
                    OnPropertyChanged(nameof(IconUri));
                }
            }
        }
        #endregion

        #region INotifyPropertyChanged
        /// <summary>
        /// I Notify Property changed events + Property changed functions
        /// </summary>
        private void NotifySelectedStateChanged()
        {
            OnPropertyChanged(nameof(IsSelected));
            OnPropertyChanged(nameof(HoverColor));
            OnPropertyChanged(nameof(ClickRippleColor));
        }

        /// <inheritdoc cref="INotifyPropertyChanged.PropertyChanged" />
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion


        #region Functions
        /// <summary>
        /// This function toggles the selected-state of the Button
        /// </summary>
        public void ToggleSelectedState() => IsSelected = !IsSelected;
        #endregion

    }
}
