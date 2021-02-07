using System.Windows;
using DJSets.resources;
using DJSets.util.Extensions;
using DJSets.util.mvvm.view;
using DJSets.viewmodel.song.song_choose;

namespace DJSets.view.song.song_choose
{
    /// <summary>
    /// This class defines code-behind logic for the SongChooseView
    /// </summary>
    public partial class SongChooseView : HostedPage
    {
        public SongChooseView(SongChooseViewModel vm)
        {
            InitializeComponent();
            
            _viewModel = vm;
            _viewModel.SongMenuViewModel = SongMenuView.SongMenuViewModel;
            DataContext = _viewModel;
            SongMenuView.SongList.SelectionChanged += (sender, args) =>
            {
                _viewModel.NotifySongSelectionChanged();
            };

            ConfigViewModelCommands();
        }

        #region Fields

        #region ViewModel
        /// <summary>
        /// This field defines a ViewModel for this View
        /// </summary>
        private readonly SongChooseViewModel _viewModel;
        #endregion

        #endregion

        #region Help Functions
        /// <summary>
        /// This function configures the Commands in <see cref="_viewModel"/> and adds further UI-Callbacks
        /// </summary>
        private void ConfigViewModelCommands()
        {
            //Cancel Command
            _viewModel.CancelCommand.OnDone(_ =>
            {
                Host.DialogResult = false;
                Host.Close();
            });

            //Choose Command
            _viewModel.ChooseCommand.BeforeExecute(_ =>
            {
                ChooseLoadingIndicator.Visibility = Visibility.Visible;
                return true;
            });
            _viewModel.ChooseCommand.OnDone(success =>
            {
                ChooseLoadingIndicator.Visibility = Visibility.Collapsed;
                if (success)
                {
                    Host.DialogResult = true;
                    Host.Close();
                    return;
                }

                this.ShowMessageBoxWithResources(StringResourceKeys.StrNote, StringResourceKeys.StrDbOperationFailed);
            });
        }

        #endregion
    }
}
