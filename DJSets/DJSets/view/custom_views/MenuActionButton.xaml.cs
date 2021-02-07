using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using DJSets.Annotations;
using DJSets.resources;
using DJSets.util.Extensions;

namespace DJSets.view.custom_views
{
    /// <summary>
    /// This class contains the code behind logic for a MenuActionButton.
    /// </summary>
    /// <remarks> A MenuActionButton is a button with a big image and a caption used in Menus</remarks>
    public partial class MenuActionButton : Button, INotifyPropertyChanged
    {
        #region Constructors
        public MenuActionButton()
        {
            DataContext = this;
            InitializeComponent();
        }
        #endregion

        #region Fields
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
                    var baseUri = new UriBuilder(baseUriStr).Uri;
                    _iconUri = new Uri(baseUri, value);
                    OnPropertyChanged(nameof(IconUri));
                }
            }
        }
        #endregion

        #region INotifyPropertyChanged
        //property changed
        /// <inheritdoc cref="INotifyPropertyChanged.PropertyChanged" />
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

    }
}
