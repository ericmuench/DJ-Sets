using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using DJSets.Annotations;
using DJSets.util.extensions;

namespace DJSets.view.custom_views
{
    /// <summary>
    /// This class defines the functionality for a SearchBar with a button to clear the text of it
    /// </summary>
    public partial class SearchBar : UserControl, INotifyPropertyChanged
    {
        #region Constructors
        public SearchBar()
        {
            InitializeComponent();
            DataContext = this;
        }
        #endregion

        #region Events
        /// <summary>
        /// This delegate function provides the function for the event of text changes in Textbox
        /// </summary>
        /// <param name="searchQuery">The current contenr of the Search-Textbox</param>
        public delegate void OnSearchQueryChangedEventHandler(string searchQuery);
        /// <summary>
        /// This event allows to react on Changes in the Search-Textbox
        /// </summary>
        public event OnSearchQueryChangedEventHandler OnSearchQueryChanged = query => { };
        #endregion

        /// <summary>
        /// This field stores the Hint for the Searchbar
        /// </summary>
        private string _hint = string.Empty;
        /// <summary>
        /// This Property provides the hint value for the view and allows bindings to it
        /// </summary>
        public string Hint
        {
            get => _hint;
            set
            {
                if (value != _hint)
                {
                    _hint = value;
                    OnPropertyChanged(nameof(Hint));
                }
            }
        }


        /// <summary>
        /// This Property checks whether the Clear-Button should be clickable
        /// </summary>
        public bool ClearBtnEnabled
        {
            get => !SearchBox.Text.IsEmpty();
        }

        #region OnPropertyChanged
        /// <inheritdoc cref="INotifyPropertyChanged"/>>
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        /// <summary>
        /// This function reacts on click-events of the Clear-Button
        /// </summary>
        /// <param name="sender">The sender of the event (Clear Button in this case)</param>
        /// <param name="e">the click event args</param>
        private void BtnClearClicked(object sender, RoutedEventArgs e)
        {
            SearchBox.Text = string.Empty;
        }

        /// <summary>
        /// This function reacts on text-change Events in the Search-Textbox with calling the <see cref="OnSearchQueryChanged"/> Event and updating some view-variables/>
        /// </summary>
        /// <param name="sender">Sender of the event (Search-Textbox in this case)</param>
        /// <param name="e">EventArgs for a Textchange Event</param>
        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            OnSearchQueryChanged(SearchBox.Text);
            OnPropertyChanged(nameof(ClearBtnEnabled));
        }
    }
}
