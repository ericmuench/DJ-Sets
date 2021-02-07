using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DJSets.model.entityframework;
using DJSets.resources;
using DJSets.util.Extensions;
using DJSets.util.mvvm;
using DJSets.view.custom_views;
using DJSets.view.song.song_detail;
using DJSets.viewmodel.song.song_detail;
using DJSets.viewmodel.song.song_item;
using DJSets.viewmodel.song.song_menu;

namespace DJSets.view.song.song_menu
{
    /// <summary>
    /// This class defines the UI-Logic for the Song Menu
    /// </summary>
    public partial class SongMenuView : Page
    {
        #region Constructors
        public SongMenuView(SongMenuViewModel vm)
        {
            InitializeComponent();

            SongMenuViewModel = vm;
            DataContext = SongMenuViewModel;
            SongMenuViewModel.OnLoadingFinished = () =>
            {
                SongList.UnselectAll();
                SongMenuViewModel.SelectedSongVms.Clear();
            };
            SongMenuViewModel.LoadData();
        }

        public SongMenuView() : this(new SongMenuViewModel()){}
        #endregion

        #region Fields
        /// <summary>
        /// This field defines the viewmodel for the SongMenuView
        /// </summary>
        public readonly SongMenuViewModel SongMenuViewModel;
        #endregion

        #region Functions
        /// <summary>
        /// This function is called when the text of the searchbar changes
        /// </summary>
        /// <param name="searchquery">current text in the searchbar</param>
        private void OnSearchbarTextChanged(string searchquery) 
            => SongMenuViewModel.FilterText = searchquery;

        /// <summary>
        /// This function is called when the button for adding a song is clicked
        /// </summary>
        /// <param name="sender">Sender of the click-Event (Song-Add-Button in this case)</param>
        /// <param name="e">Click-Event-Eventargs</param>
        private void OnBtnAddSongClicked(object sender, RoutedEventArgs e) => OpenSongDetailWindow();

        /// <summary>
        /// This function opens a new Window dealing with Song-Operations like add, update or delete
        /// </summary>
        /// <param name="container">A Container that can emit a Song, e.g. a Viewmodel</param>
        private void OpenSongDetailWindow(IElementContainer<Song> container = null)
        {
            var title = this.GetResource<string>(StringResourceKeys.StrManageSong);
            new CustomDialogWindow(new SongDetailView(new SongDetailViewModel(container)), title, window =>
            {
                window.Height = 500;
            }).ShowDialog();
        }
        

        /// <summary>
        /// This function reacts to the event of double-clicking an element of <see cref="SongList"/>
        /// </summary>
        /// <param name="sender">sender of the event, usually an item of <see cref="SongList"/></param>
        /// <param name="e">Event Args</param>
        private void OnSongItemLeftDoubleClicked(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                SongMenuViewModel.SelectedSongVms.NotNull(osvms =>
                {
                    if(osvms.Count == 1)
                    {
                        OpenSongDetailWindow(osvms[0]);
                    }
                    
                });
            }
        }

        /// <summary>
        /// This function removes all selected items of <see cref="SongList"/> when right clicking an element.
        /// </summary>
        /// <param name="sender">sender of the event, usually an item of <see cref="SongList"/></param>
        /// <param name="e">Event Args</param>
        private void OnSongItemRightClicked(object sender, MouseButtonEventArgs e) => SongList.UnselectAll();

        #endregion

        /// <summary>
        /// This function reacts on Selection Changes in <see cref="SongList"/>
        /// </summary>
        /// <param name="sender">sender of the event, usually <see cref="SongList"/></param>
        /// <param name="e">Event Args</param>
        private void OnSongListSelectedItemChanged(object sender, SelectionChangedEventArgs e)
        {
            SongMenuViewModel.SelectedSongVms.Clear();
            foreach (OverviewSongItemViewModel item in SongList.SelectedItems)
            {
                SongMenuViewModel.SelectedSongVms.Add(item);
            }
            BtnEditSong.IsEnabled = SongMenuViewModel.SelectedSongVms.Count == 1;
        }

        /// <summary>
        /// This function reacts to the event of clicking the <see cref="BtnEditSong"/>-Button
        /// </summary>
        /// <param name="sender">Sender, usually <see cref="BtnEditSong"/></param>
        /// <param name="e">Event Args</param>
        private void OnButtonEditSongClicked(object sender, RoutedEventArgs e)
        {
            if (SongMenuViewModel.SelectedSongVms.Count == 1)
            {
                OpenSongDetailWindow(SongMenuViewModel.SelectedSongVms[0]);
            }
        }

        /// <summary>
        /// This function reacts when user left-clicks the background of this page
        /// </summary>
        /// <param name="sender">Sender, usually the Background, meaning the whole Grid-Layout</param>
        /// <param name="e">Event Args</param>
        private void OnBackgroundLeftClicked(object sender, MouseButtonEventArgs e) => SongList.UnselectAll();
    }
}
