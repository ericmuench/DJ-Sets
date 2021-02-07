using System.Windows;
using DJSets.resources;
using DJSets.util.Extensions;
using DJSets.util.mvvm.view;
using DJSets.viewmodel.setlist.setlist_edit;

namespace DJSets.view.setlist.setlist_edit
{
    /// <summary>
    /// This class defines code-behind logic for SetlistEditView
    /// </summary>
    public partial class SetlistEditView : HostedPage
    {
        #region Constructors
        public SetlistEditView(SetlistEditViewModel vm = null)
        {
            InitializeComponent();
            _viewModel = vm;
            _viewModel?.SetTitleDescriptionSource(TitleDescriptionView.ViewModel);
            DataContext = _viewModel;
            ConfigViewModelCommands();

        }

        public SetlistEditView() : this(null) { }
        #endregion

        #region Fields
        /// <summary>
        /// This field provides viewmodel data for this view
        /// </summary>
        private readonly SetlistEditViewModel _viewModel;

        #endregion

        #region Help Functions
        /// <summary>
        /// This function configures the commands in viewmodel
        /// </summary>
        void ConfigViewModelCommands()
        {
            _viewModel.SaveCommand.BeforeExecute(_ =>
            {
                SaveLoadingIndicator.Visibility = Visibility.Visible;
                return true;
            });
            _viewModel.SaveCommand.OnDone(success =>
            {
                SaveLoadingIndicator.Visibility = Visibility.Collapsed;
                if (success)
                {
                    Host.DialogResult = true;
                    Host.Close();
                    return;
                }

                this.ShowMessageBoxWithResources(StringResourceKeys.StrNote, StringResourceKeys.StrDbOperationFailed);
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
