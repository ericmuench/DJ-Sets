using System.Windows;
using DJSets.util.mvvm.view;
using DJSets.viewmodel.setlist.setlist_setup;

namespace DJSets.view.setlist.setlist_setup
{
    /// <summary>
    /// This class defines the code behind logic for SetlistSetupView
    /// </summary>
    public partial class SetlistSetupView : HostedPage
    {
        #region Constructors
        public SetlistSetupView(SetlistSetupViewModel vm = null)
        {
            InitializeComponent();
            _viewModel = vm;
            _viewModel?.SetTitleDescriptionSource(TitleDescriptionView.ViewModel);
            DataContext = _viewModel;
            ConfigViewModelCommands();

        }

        public SetlistSetupView() : this(null) {}
        #endregion

        #region Fields
        /// <summary>
        /// This field provides viewmodel data for this view
        /// </summary>
        private readonly SetlistSetupViewModel _viewModel;

        #endregion

        #region Help Functions
        /// <summary>
        /// This function configures the commands in viewmodel
        /// </summary>
        void ConfigViewModelCommands()
        {
            _viewModel.CreateCommand.BeforeExecute(_ =>
            {
                CreateLoadingIndicator.Visibility = Visibility.Visible;
                return true;
            });
            _viewModel.CreateCommand.OnDone(_ =>
            {
                CreateLoadingIndicator.Visibility = Visibility.Collapsed;
                Host.DialogResult = true;
                Host.Close();
            });

            _viewModel.CancelCommand.OnDone(_ =>
            {
                Host.DialogResult = false;
                Host.Close();
            });
        }

        #endregion
    }
}
