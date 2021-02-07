using System.Linq;
using System.Windows;
using DJSets.resources;
using DJSets.util.Extensions;
using DJSets.util.mvvm.view;
using DJSets.viewmodel.filescan;

namespace DJSets.view.filescan
{
    /// <summary>
    /// This class defines code-behind logic for MusicFileScanView.xaml
    /// </summary>
    public partial class MusicFileScanView : HostedPage
    {
        #region Constructors
        public MusicFileScanView(MusicFileScanViewModel vm)
        {
            InitializeComponent();

            _viewModel = vm;
            DataContext = _viewModel;
            ConfigViewModelCommands();
            ConfigViewModelBackgroundWorker();

            _viewModel.OnScanError = _ =>
            {
                this.ShowMessageBoxWithResources(
                    StringResourceKeys.StrNote,
                    StringResourceKeys.StrFileScanFailed,
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);

                Host.DialogResult = false;
                Host.Close();
            };
            _viewModel.BackgroundWorker.RunWorkerAsync();
        }
        #endregion

        #region _viewmodel
        /// <summary>
        /// This field defines a VM for this view
        /// </summary>
        private readonly MusicFileScanViewModel _viewModel;

        #endregion

        #region Help Functions
        /// <summary>
        /// This function assigns further UI Callbacks to the commands of <see cref="_viewModel"/>
        /// </summary>
        private void ConfigViewModelCommands() => _viewModel.NotNull(vm =>
        {
            //SaveCommand
            vm.SaveCommand.BeforeExecute(_ =>
            {
                var willExe = true;
                if (_viewModel.ShouldOverrideSongDb)
                {
                    var dialogRes = this.ShowMessageBoxWithResources(
                        StringResourceKeys.StrNote,
                        StringResourceKeys.StrSureToOverrideDb,
                        MessageBoxButton.YesNo);
                    willExe = dialogRes == MessageBoxResult.Yes;
                }

                if (willExe)
                {
                    SaveLoadingIndicator.Visibility = Visibility.Visible;
                }

                return willExe;
            });
            vm.SaveCommand.OnDone(success =>
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

            //Cancel Command
            vm.CancelCommand.BeforeExecute(_ =>
            {
                if (_viewModel.BackgroundWorker.IsBusy)
                {
                    var dialogResult = this.ShowMessageBoxWithResources(
                        StringResourceKeys.StrNote,
                        StringResourceKeys.StrSureCancelScanProcess,
                        MessageBoxButton.YesNo);
                    return dialogResult == MessageBoxResult.Yes;
                }

                return true;

            });

            vm.CancelCommand.OnDone(success =>
            {
                if (!success)
                {
                    this.ShowMessageBoxWithResources(StringResourceKeys.StrNote,
                        StringResourceKeys.StrCancelOoperationFailed,
                        MessageBoxButton.OK,
                        MessageBoxImage.Error);
                    vm.CancelCommand.NotifyCanExecuteChanged();
                    return;
                }

                Host.DialogResult = true;
                Host.Close();
            });
        });

        /// <summary>
        /// This function configures the BackgroundWorker of <see cref="_viewModel"/> by applying further UI Actions
        /// </summary>
        private void ConfigViewModelBackgroundWorker()
        {
            _viewModel.BackgroundWorker.ProgressChanged += (sender, args) =>
            {
                ProgressBarScan.Value = args.ProgressPercentage;
                TxtBlkStatus.Text = $"{args.ProgressPercentage} %";
            };
            _viewModel.BackgroundWorker.RunWorkerCompleted += (sender, args) =>
            {
                _viewModel?.ScannedSongs.NotNull(scSongs =>
                {
                    var header = Application
                        .Current
                        .GetResource<string>(StringResourceKeys.StrTemplateScannedSongsCount)
                        .Replace("#", scSongs.Count.ToString());
                    var songs = string.Join("\n", scSongs.Select(it => $"{it.Artist} - {it.Title} ({it.FilePath})"));
                    TxtBlkStatus.Text = $"{header}\n\n{songs}";
                });
            };
        }
        #endregion
    }
}
