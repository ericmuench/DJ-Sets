using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using DJSets.util.Extensions;
using DJSets.view.custom_views;
using DJSets.view.options.options_menu;
using DJSets.view.setlist.setlist_menu;
using DJSets.view.song.song_menu;
using DJSets.viewmodel.main_menu;
using DJSets.viewmodel.options;
using DJSets.viewmodel.setlist.setlist_menu;
using DJSets.viewmodel.song.song_menu;

namespace DJSets.view.main_menu
{
    /// <summary>
    /// This class defines the logic of the MainMenu of this application embedded in the MainWindow if user is in MainWindow
    /// </summary>
    public partial class MainMenuView : Page
    {
        #region Constructors
        public MainMenuView()
        {
            InitializeComponent();
            //init Menu Views
            InitMenuViews();

            //Configure Viewmodel
            _mainMenuViewModel = new MainMenuViewModel(MainMenuState.Setlists, (state) =>
            {
                _menuViews[state].NotNull(it => MainMenuNavigationFrame.Navigate(it));
            });
            SelectCorrespondingNavigationButton(_mainMenuViewModel.MainMenuState);

            
        }
        #endregion

        #region Fields
        //view children fields
        /// <summary>
        /// This field defines the mapping between MenuState and corresponding views
        /// </summary>
        private readonly Dictionary<MainMenuState, Page> _menuViews = new Dictionary<MainMenuState, Page>();

        //viewmodel
        /// <summary>
        /// This field defines the ViewModel for the MainMenu that holds the MainMenuState
        /// </summary>
        private readonly MainMenuViewModel _mainMenuViewModel;
        #endregion


        #region Functions
        /// <summary>
        /// This function handles click events on a MenuNavigationButton
        /// </summary>
        /// <param name="sender">The clicked Element</param>
        /// <param name="e">The corresponding EventArgs wit this</param>
        private void OnNavBtnClicked(object sender, RoutedEventArgs e)
        {
            sender.CastedAs<MenuNavigationButton>(btn =>
            {
                SelectSingleNavigationButton(btn);
                _mainMenuViewModel.MainMenuState = btn.MenuState;
            });
        }

        /// <summary>
        /// This help-function Selects the correct MenuNavigationButton the belongs to a certain view in main menu according to the current MainMenuState
        /// </summary>
        /// <param name="state">The current Main Menu State</param>
        private void SelectCorrespondingNavigationButton(MainMenuState state)
        {
            switch (state)
            {
                case MainMenuState.Setlists: 
                    SelectSingleNavigationButton(NavBtnSetlists);
                    break;
                case MainMenuState.Songs:
                    SelectSingleNavigationButton(NavBtnSongs);
                    break;
                case MainMenuState.Options:
                    SelectSingleNavigationButton(NavBtnOptions);
                    break;
            }
        }

        /// <summary>
        /// This help-function selects the given Button and sets all other Buttons to be in a unselected state
        /// </summary>
        /// <param name="btn">The MenuNavigationButton which needs to be selected</param>
        private void SelectSingleNavigationButton(MenuNavigationButton btn)
        {
            NavBtnOptions.IsSelected = false;
            NavBtnSetlists.IsSelected = false;
            NavBtnSongs.IsSelected = false;
            btn.ToggleSelectedState();
        }
        #endregion

        #region Help Functions

        /// <summary>
        /// This function initializes the <see cref="_menuViews"/> Property by adding the states for the MainMenu with its corresponding views
        /// </summary>
        private void InitMenuViews()
        {
            _menuViews.Add(MainMenuState.Setlists, new SetlistMenuView(new SetlistMenuViewModel(), this));
            _menuViews.Add(MainMenuState.Songs, new SongMenuView(new SongMenuViewModel()));
            _menuViews.Add(MainMenuState.Options, new OptionsMenuView(new OptionsViewModel()));
        }

        #endregion
    }
}
