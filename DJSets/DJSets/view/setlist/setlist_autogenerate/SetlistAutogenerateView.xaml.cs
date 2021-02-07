using System.Windows;
using DJSets.resources;
using DJSets.util.Extensions;
using DJSets.util.mvvm.view;
using DJSets.viewmodel.setlist.setlist_autogenerate;

namespace DJSets.view.setlist.setlist_autogenerate
{
    /// <summary>
    /// This class defines code-behind logic for SetlistAutogenerateView.xaml
    /// </summary>
    public partial class SetlistAutogenerateView : HostedPage
    {
        #region Constructors
        public SetlistAutogenerateView(SetlistAutogenerateViewModel vm)
        {
            InitializeComponent();
            _viewModel = vm;
            _viewModel.SetTitleDescriptionSource(TitleDescriptionViewAutogenerate.ViewModel);
            ConfigViewModelCommands();
            ConfigViewModelBackgroundWorker();
            DataContext = _viewModel;

        }
        #endregion

        #region Fields
        /// <summary>
        /// This field defines the viewmodel for this View
        /// </summary>
        private readonly SetlistAutogenerateViewModel _viewModel;

        #endregion

        #region Help functions
        /// <summary>
        /// This function configures the commands of <see cref="_viewModel"/> and add further UI Callbacks
        /// </summary>
        private void ConfigViewModelCommands()
        {
            //Cancel Command
            _viewModel.CancelCommand.BeforeExecute(_ =>
            {
                if (!_viewModel.AutoGenerationBackgroundWorker.IsBusy)
                {
                    return true;
                }

                var dialogResult = this.ShowMessageBoxWithResources(
                    StringResourceKeys.StrNote,
                    StringResourceKeys.StrSureCancelSetlistAutogeneration,
                    MessageBoxButton.YesNo);
                return dialogResult == MessageBoxResult.Yes;

            });

            _viewModel.CancelCommand.OnDone(success =>
            {
                if (!success)
                {
                    this.ShowMessageBoxWithResources(StringResourceKeys.StrNote,
                        StringResourceKeys.StrCancelOoperationFailed,
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    _viewModel.CancelCommand.NotifyCanExecuteChanged();
                    return;
                }

                Host.DialogResult = true;
                Host.Close();
            });

            //Save Command
            _viewModel.SaveCommand.BeforeExecute(_ =>
            {
                if (_viewModel.SetlistPositionsFromAutogeneration.Count == 0)
                {
                    var dialogRes = this.ShowMessageBoxWithResources(
                        StringResourceKeys.StrNote,
                        StringResourceKeys.StrQuestionSaveGeneratedEmptySetAnyway,
                        MessageBoxButton.YesNo);

                    return dialogRes == MessageBoxResult.Yes;
                }
                
                
                if (_viewModel.SetlistWillBeShorterThanExpected())
                {
                    var dialogRes = this.ShowMessageBoxWithResources(
                        StringResourceKeys.StrNote,
                        StringResourceKeys.StrQuestionSaveGeneratedShorterSet,
                        MessageBoxButton.YesNo);

                    return dialogRes == MessageBoxResult.Yes;
                }

                SaveLoadingIndicator.Visibility = Visibility.Visible;
                return true;
            });

            _viewModel.SaveCommand.OnDone(success =>
            {
                SaveLoadingIndicator.Visibility = Visibility.Collapsed;
                if (!success)
                {
                    this.ShowMessageBoxWithResources(
                        StringResourceKeys.StrNote,
                        StringResourceKeys.StrDbOperationFailed,
                        MessageBoxButton.OK);
                    return;
                }

                Host.DialogResult = true;
                Host.Close();
            });
        }

        /// <summary>
        /// This function configures the BackgroundWorker of <see cref="_viewModel"/> by adding further UI Callbacks
        /// </summary>
        private void ConfigViewModelBackgroundWorker()
        {
            _viewModel.AutoGenerationBackgroundWorker.ProgressChanged += (sender, args) =>
            {
                ProgressBarAutoGenerate.Value = args.ProgressPercentage;
            };
        }

        #endregion
    }
}
