using System.Windows;
using DJSets.resources;
using DJSets.util.Extensions;
using DJSets.util.mvvm.view;
using DJSets.viewmodel.song.song_detail;
using Microsoft.Win32;

namespace DJSets.view.song.song_detail
{
    /// <summary>
    /// This view shows details of a song
    /// </summary>
    public partial class SongDetailView : HostedPage
    {

        #region Constructors
        /// <summary>
        /// This constructor creates a SongDetailView with reference to parent and a viewmodel
        /// </summary>
        /// <param name="vm">Corresponding ViewModel</param>
        /// <param name="parent">Hosted Window</param>
        public SongDetailView(SongDetailViewModel vm, Window parent = null) : base(parent)
        {
            InitializeComponent();
            //configuring + assigning viewmodel
            _viewModel = vm;
            DataContext = _viewModel;
            ConfigViewModelCommands();
        }
        #endregion

        #region Fields
        /// <summary>
        /// This field is the corresponding viewmodel for this view
        /// </summary>
        private readonly SongDetailViewModel _viewModel;
        #endregion

        #region Help Functions
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

                this.ShowMessageBoxWithResources(StringResourceKeys.StrNote, 
                    StringResourceKeys.StrDbOperationFailed);

            });

            _viewModel.CancelCommand.OnDone(_ =>
            {
                Host.DialogResult = false;
                Host.Close();
            });

            _viewModel.DeleteCommand.OnDone(_ =>
            {
                Host.DialogResult = true;
                Host.Close();
            });

            _viewModel.DeleteCommand.BeforeExecute(obj =>
            {
                MessageBoxResult msgRes = this.ShowMessageBoxWithResources(StringResourceKeys.StrNote,
                    StringResourceKeys.StrQuestionShouldDeleteSong, MessageBoxButton.YesNo);
                return msgRes == MessageBoxResult.Yes;
            });
        }

        #endregion

        private void OnChooseMusicFile(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog().Apply(it =>
            {
                it.Multiselect = false;

                var filterTitle = Application.Current.GetResource<string>(StringResourceKeys.StrMusicFiles);
                it.Filter = $"{filterTitle}|*.mp3;*.wav;*.ogg;*.flac;*.wma;*.acc";
            });

            var result = dialog.ShowDialog();
            if (result != null && result == true)
            {
                _viewModel.FilePath = dialog.FileName;
            }
        }
    }
}
