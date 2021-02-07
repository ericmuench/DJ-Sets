using System.Windows.Controls;
using DJSets.viewmodel.vm_util.titledescription;

namespace DJSets.view.view_util.title_description
{
    /// <summary>
    /// This class defines the code-behind logic for the TitleDescriptionView
    /// </summary>
    /// <remarks>This view should help provide a title and a description</remarks>
    public partial class TitleDescriptionView : Page
    {
        #region Constructors
        public TitleDescriptionView(TitleDescriptionViewModel vm = null)
        {
            InitializeComponent();
            ViewModel = vm ?? new TitleDescriptionViewModel();
            DataContext = ViewModel;
        }

        public TitleDescriptionView() : this(null) { }
        #endregion

        #region Fields
        /// <summary>
        /// This field defines the viewmodel for this view with its data
        /// </summary>
        public TitleDescriptionViewModel ViewModel;
        #endregion
    }
}
