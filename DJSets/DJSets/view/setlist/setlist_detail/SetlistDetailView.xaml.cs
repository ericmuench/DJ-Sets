using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DJSets.resources;
using DJSets.util.Extensions;
using DJSets.view.custom_views;
using DJSets.view.setlist.setlist_edit;
using DJSets.view.setlist.setlist_export;
using DJSets.view.song.song_choose;
using DJSets.view.song.song_detail;
using DJSets.viewmodel.setlist.setlist_detail;
using DJSets.viewmodel.setlist.setlist_edit;
using DJSets.viewmodel.setlist.setlist_export;
using DJSets.viewmodel.song.song_choose;
using DJSets.viewmodel.song.song_detail;

namespace DJSets.view.setlist.setlist_detail
{
    /// <summary>
    /// This class defines the code-behind logic for a SetlistDetailView
    /// </summary>
    public partial class SetlistDetailView : Page
    {
        #region Constructors
        public SetlistDetailView(SetlistDetailViewModel vm)
        {
            InitializeComponent();

            _viewModel = vm;
            DataContext = _viewModel;
            ConfigViewModelCommands();
        }
        #endregion

        #region Fields
        /// <summary>
        /// This field is the corresponding viewmodel for this view
        /// </summary>
        private readonly SetlistDetailViewModel _viewModel;

        #endregion

        /// <summary>
        /// This function handles the click-event for clicking the "Back"-Button.
        /// </summary>
        /// <param name="sender">the sender for the click-event which is the "Back"-Button in this case</param>
        /// <param name="e">click event args</param>
        private void OnBackButtonClicked(object sender, RoutedEventArgs e) => GoBack();

        /// <summary>
        /// This function is called every time this view is left by the user
        /// </summary>
        private void GoBack()
        {
            _viewModel.EnsureNoAudioPlaying();
            NavigationService?.GoBack();
        }

        /// <summary>
        /// This function handles the click-event for clicking the "Add"-Button.
        /// </summary>
        /// <param name="sender">the sender for the click-event which is the "Back"-Button in this case</param>
        /// <param name="e">click event args</param>
        private void OnAddSong(object sender, RoutedEventArgs e)
        {
            _viewModel.EnsureNoAudioPlaying();
            var title = Application.Current.GetResource<string>(StringResourceKeys.StrAddSong);
            new CustomDialogWindow(new SongChooseView(new SongChooseViewModel(_viewModel)),title, window =>
            {
                window.Height = 750;
            }).ShowDialog();
        }

        /// <summary>
        /// This function handles the edit-operation for a setlists title and description
        /// </summary>
        /// <param name="sender">The Sender of the event, usually Edit-Button</param>
        /// <param name="e">Event Args</param>
        private void OnEditSetlist(object sender, RoutedEventArgs e)
        {
            _viewModel.EnsureNoAudioPlaying();
            var title = Application.Current.GetResource<string>(StringResourceKeys.StrEditSet);
            new CustomDialogWindow(new SetlistEditView(new SetlistEditViewModel(_viewModel)),title).ShowDialog();
        }

        #region Help Functions
        /// <summary>
        /// This function adds further View-Logic to execution of the VM-Commands
        /// </summary>
        private void ConfigViewModelCommands()
        {
            //Delete Command
            _viewModel.DeleteCommand.OnDone(success =>
            {
                if (success)
                {
                    GoBack();
                    return;
                }
                this.ShowMessageBoxWithResources(StringResourceKeys.StrNote, StringResourceKeys.StrDbOperationFailed);
            });

            _viewModel.DeleteCommand.BeforeExecute(obj =>
            {
                _viewModel.EnsureNoAudioPlaying();
                MessageBoxResult msgRes = this.ShowMessageBoxWithResources(StringResourceKeys.StrNote,
                    StringResourceKeys.StrQuestionShouldDeleteSetlist, MessageBoxButton.YesNo);
                return msgRes == MessageBoxResult.Yes;
            });
        }
        #endregion

        /// <summary>
        /// This function reacts on double click on a certain SetlistPosition
        /// </summary>
        /// <param name="sender">Sender of the Event</param>
        /// <param name="e">Event Args</param>
        private void OnSetlistPositionLeftDoubleClicked(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                _viewModel.EnsureNoAudioPlaying();
                var song = _viewModel.SelectedSetlistPositionVm.GetElement().Song;
                var title = Application.Current.GetResource<string>(StringResourceKeys.StrEditSong);
                new CustomDialogWindow(new SongDetailView(new SongDetailViewModel(song)),title).ShowDialog();
            }
        }

        /// <summary>
        /// This function handles click events on the Share-Button
        /// </summary>
        /// <param name="sender">Sender of the Event</param>
        /// <param name="e">Event Args</param>
        private void OnShareSetlist(object sender, RoutedEventArgs e)
        {
            _viewModel.EnsureNoAudioPlaying();
            var title = Application.Current.GetResource<string>(StringResourceKeys.StrSetlistExport);
            new CustomDialogWindow(new SetlistExportView(new SetlistExportViewModel(_viewModel)),title)
                .ShowDialog();
            //got help from https://www.wpf-tutorial.com/dialogs/the-savefiledialog/
            /*var dialog = new SaveFileDialog().Apply(dia =>
            {
                var filterTitle = Application.Current.GetResource<string>(StringResourceKeys.StrTextFiles);
                dia.Filter = $"{filterTitle}|*.txt";
                
                dia.AddExtension = true;
                dia.FileName = $"{_viewModel.SetListTitle}.txt";
            });
            
            dialog.ShowDialog().NotNull(shouldSave =>
            {
                if (shouldSave)
                {
                    _viewModel.ExportSetlist(dialog.FileName, success =>
                    {
                        var app = Application.Current;
                        var title = app.GetResource<string>(StringResourceKeys.StrNote);
                        var msg = app.GetResource<string>(StringResourceKeys.StrExportSetlistFailed);

                        if (success)
                        {
                            msg = app.GetResource<string>(StringResourceKeys.StrTemplateSetlistExportSuccess)
                                .Replace("#",dialog.FileName);
                        }

                        var img = success ? MessageBoxImage.Information : MessageBoxImage.Error;

                        WpfUiExtensions.ShowMessageBox(title, msg, MessageBoxButton.OK, img);
                    });
                }
            });*/
        }
    }
}
